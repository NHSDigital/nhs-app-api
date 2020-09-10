using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.SessionManager;
using NHSOnline.Backend.PfsApi.Areas.Session;
using NHSOnline.Backend.PfsApi.GpSession;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Http;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public sealed class UnauthorisedGpSystemHttpRequestExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly ILogger<UnauthorisedGpSystemHttpRequestExceptionFilterAttribute> _logger;
        private readonly IUserSessionService _userSessionService;
        private readonly ISessionCacheService _sessionCacheService;
        private readonly IErrorReferenceGenerator _errorReferenceGenerator;
        private readonly ISessionErrorResultBuilder _sessionErrorResultBuilder;

        public UnauthorisedGpSystemHttpRequestExceptionFilterAttribute(
            ILogger<UnauthorisedGpSystemHttpRequestExceptionFilterAttribute> logger,
            IUserSessionService userSessionService,
            ISessionCacheService sessionCacheService,
            IErrorReferenceGenerator errorReferenceGenerator,
            ISessionErrorResultBuilder sessionErrorResultBuilder)
        {
            _logger = logger;
            _userSessionService = userSessionService;
            _sessionCacheService = sessionCacheService;
            _errorReferenceGenerator = errorReferenceGenerator;
            _sessionErrorResultBuilder = sessionErrorResultBuilder;
        }

        public override async Task OnExceptionAsync(ExceptionContext context)
        {
            if (context.ExceptionHandled)
            {
                return;
            }

            if (!(context.Exception is UnauthorisedGpSystemHttpRequestException))
            {
                return;
            }

            await ClearDownGpUserSessionIfPresent(context);

            _logger.LogDebug(context.Exception, $"{nameof(UnauthorisedGpSystemHttpRequestException)} handled");

            context.ExceptionHandled = true;
        }

        [SuppressMessage("ReSharper", "CA1031", Justification = "Exception filter should not throw an exception")]
        private async Task ClearDownGpUserSessionIfPresent(ExceptionContext context)
        {
            UserSession userSession = null;

            _userSessionService.GetUserSession<UserSession>()
                .IfSome(s => userSession = s);

            var p9UserSession = userSession as P9UserSession;

            if (p9UserSession is null)
            {
                context.Result = new StatusCodeResult(StatusCodes.Status401Unauthorized);

                var sessionType = userSession is null ? "user with no session" : "P5 user";
                _logger.LogWarning($"{ nameof(UnauthorisedGpSystemHttpRequestException) } was caught - " +
                                   $"returning { nameof(StatusCodes.Status401Unauthorized) } for { sessionType }");

                return;
            }

            try
            {
                _logger.LogInformation(
                    "Attempting to clear down GP User Session for P9 user after receiving an unauthorised " +
                    "response from GP System");

                var clearDownVisitor = new GpUserSessionClearDownVisitor(
                    _logger,
                    _errorReferenceGenerator,
                    _sessionCacheService,
                    p9UserSession);

                await clearDownVisitor.Visit(p9UserSession.GpUserSession);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to clear down P9 GP User Session");
            }
            finally
            {
                context.Result = _sessionErrorResultBuilder.BuildResult(new ErrorTypes.GPSessionUnavailable());

                _logger.LogWarning($"{ nameof(UnauthorisedGpSystemHttpRequestException) } was caught - " +
                                   $"returning { Constants.CustomHttpStatusCodes.Status599GpSessionUnavailable } for " +
                                   "P9 user");
            }
        }
    }
}
