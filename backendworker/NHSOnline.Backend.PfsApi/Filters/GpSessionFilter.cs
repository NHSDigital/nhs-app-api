using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.PfsApi.Areas.Session;
using NHSOnline.Backend.PfsApi.GpSession;
using NHSOnline.Backend.PfsApi.Session;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Http;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.Filters
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "ASP.NET Filter")]
    public class GpSessionFilter :  IAsyncActionFilter
    {
        private readonly ILogger<GpSessionFilter> _logger;
        private readonly IUserSessionService _userSessionService;
        private readonly IGpSessionCreator _gpSessionCreator;
        private readonly IGpSystemFactory _gpSystemFactory;
        private readonly ISessionErrorResultBuilder _errorResultBuilder;

        public GpSessionFilter(
            ILogger<GpSessionFilter> logger,
            IUserSessionService userSessionService,
            IGpSessionCreator gpSessionCreator,
            IGpSystemFactory gpSystemFactory,
            ISessionErrorResultBuilder errorResultBuilder)
        {
            _logger = logger;
            _userSessionService = userSessionService;
            _gpSessionCreator = gpSessionCreator;
            _gpSystemFactory = gpSystemFactory;
            _errorResultBuilder = errorResultBuilder;
            _userSessionService = userSessionService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var gpSessionParameters = context.ActionDescriptor
                .Parameters
                .OfType<ControllerParameterDescriptor>()
                .Where(d => d.HasAttribute<GpSessionAttribute>())
                .ToList();

            if (!gpSessionParameters.Any())
            {
                await next();

                return;
            }

            _logger.LogInformation($"Injecting {nameof(GpUserSession)} into controller method parameters");

            var sessionOption = _userSessionService.GetUserSession<P9UserSession>();

            await sessionOption.IfSome(async session =>
            {
                var injectionResult = await InjectGpSessionParameters(
                    context,
                    session,
                    gpSessionParameters);

                if (injectionResult)
                {
                    await next();
                }
            }).IfNone(() =>
            {
                _logger.LogError("Attempted to inject GP Session parameter for non P9 User Session");

                context.Result = new UnauthorizedResult();

                return Task.CompletedTask;
            });
        }

        private async Task<bool> InjectGpSessionParameters(
            ActionExecutingContext context,
            P9UserSession p9Session,
            IList<ControllerParameterDescriptor> gpSessionParameters)
        {
            if (!ValidateSessionAndParameters(context, gpSessionParameters))
            {
                _logger.LogError("Session or parameters invalid, rejecting request");

                return false;
            }

            var sessionValid = await EnsureGpSessionIsValid(context, p9Session);

            if (!sessionValid)
            {
                _logger.LogError("GP session invalid, rejecting request");

                return false;
            }

            _logger.LogInformation("GP session valid, injecting value into method parameter(s)");

            gpSessionParameters.ForEach(p => context.ActionArguments[p.Name] = p9Session.GpUserSession);

            return true;
        }

        private bool ValidateSessionAndParameters(
            ActionExecutingContext context,
            IList<ControllerParameterDescriptor> gpSessionParameters)
        {
            var invalidParameters = gpSessionParameters.Where(p =>
                    p.ParameterType != typeof(GpUserSession))
                .Select(p => p.Name)
                .ToList();

            if (invalidParameters.Any())
            {
                _logger.LogInformation(
                    $"Parameter(s) {string.Join(", ", invalidParameters)} require a GP session " +
                    $"but are not of type {nameof(GpUserSession)}");
                context.Result = new StatusCodeResult(StatusCodes.Status500InternalServerError);

                return false;
            }

            return true;
        }

        private async Task<bool> EnsureGpSessionIsValid(ActionExecutingContext context, P9UserSession session)
        {
            var recreateVisitor = new GpSessionRecreateVisitor(_logger, _gpSessionCreator, session);
            var recreateResult = await session.GpUserSession.Accept(recreateVisitor);

            var filterRecreateRulesVisitor = new FilterSessionRecreateResultVisitor(this, context, session);

            return await recreateResult.Accept(filterRecreateRulesVisitor);
        }

        private class FilterSessionRecreateResultVisitor : IGpSessionRecreateResultVisitor<Task<bool>>
        {
            private readonly GpSessionFilter _gpSessionFilter;
            private readonly ActionExecutingContext _context;
            private readonly P9UserSession _p9UserSession;

            public FilterSessionRecreateResultVisitor(
                GpSessionFilter gpSessionFilter,
                ActionExecutingContext context,
                P9UserSession p9UserSession)
            {
                _gpSessionFilter = gpSessionFilter;
                _context = context;
                _p9UserSession = p9UserSession;
            }

            public async Task<bool> Visit(GpSessionRecreateResult.RecreatedResult createRecreatedResult)
            {
                _gpSessionFilter._logger.LogInformation(
                    $"Successfully recreated gp session for user from ODS code: {_p9UserSession.OdsCode}");

                await InjectPatientIdParameterValues();

                return true;
            }

            public Task<bool> Visit(GpSessionRecreateResult.SessionStillValidResult createRecreatedResult)
            {
                _gpSessionFilter._logger.LogInformation(
                    $"GP session still valid for user from ODS code: {_p9UserSession.OdsCode}");

                return Task.FromResult(true);
            }

            public Task<bool> Visit(GpSessionRecreateResult.ErrorResult errorResult)
            {
                _gpSessionFilter._logger.LogError(
                    "Authentication failure when recreating gp session for " +
                    $"user from ODS code: {_p9UserSession.OdsCode}");

                _context.Result = _gpSessionFilter._errorResultBuilder.BuildResult(errorResult.ErrorType);

                return Task.FromResult(false);
            }

            private async Task InjectPatientIdParameterValues()
            {
                var gpSession = _p9UserSession.GpUserSession;
                var gpSessionPatientId = gpSession.Id;

                var patientIdParameters = _context.ActionDescriptor
                    .GetParametersWithAttributeAndType<FromHeaderAttribute, Guid>()
                    .Where(kvp => kvp.Value.Name == Constants.HttpHeaders.PatientId)
                    .Select(kvp => kvp.Key)
                    .ToList();

                if (!_p9UserSession.GpUserSession.HasLinkedAccounts)
                {
                    // no linked accounts, so just inject the current patient ID
                    patientIdParameters.ForEach(p => _context.ActionArguments[p] = gpSessionPatientId);
                    return;
                }

                // inject current patient ID for parameters which are not set to the ID of a known linked account
                var linkedAccounts = await _gpSessionFilter._gpSystemFactory.CreateGpSystem(gpSession.Supplier)
                    .GetLinkedAccountsService()
                    .GetLinkedAccounts(gpSession);
                var linkedAccountIds = linkedAccounts.Accept(new LinkedAccountsGetIdsVisitor());

                foreach (var parameterName in patientIdParameters)
                {
                    if (!_context.ActionArguments.ContainsKey(parameterName))
                    {
                        _context.ActionArguments[parameterName] = gpSessionPatientId;
                    }
                    else if (_context.ActionArguments[parameterName] is Guid patientIdGuid
                             && !linkedAccountIds.Contains(patientIdGuid))
                    {
                        _context.ActionArguments[parameterName] = gpSessionPatientId;
                    }
                }
            }
        }
    }
}
