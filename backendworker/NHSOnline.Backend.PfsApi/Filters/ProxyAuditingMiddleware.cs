using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.AspNet;
using NHSOnline.Backend.Support.Session;
using static NHSOnline.Backend.Support.Constants.HttpHeaders;

namespace NHSOnline.Backend.PfsApi.Filters
{
    public class ProxyAuditingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ProxyAuditingMiddleware> _logger;

        private readonly List<string> _patientIdHeaderNotExpectedForPaths = new List<string> {
            @"/v1/patient/configuration", @"/v1/patient/journey-configuration"
        };

        public ProxyAuditingMiddleware(RequestDelegate next, ILogger<ProxyAuditingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var userSessionOption = context.RequestServices.GetRequiredService<IUserSessionService>().GetUserSession<P9UserSession>();

            userSessionOption.IfSome(userSession =>
            {

                if (userSession.GpUserSession is null)
                {
                    return;
                }

                var gpSystem = context.RequestServices.GetRequiredService<IGpSystemFactory>().CreateGpSystem(userSession.GpUserSession.Supplier);

                if (gpSystem.SupportsLinkedAccounts && TryParsePatientId(context, out var patientId))
                {
                    var linkedAccountsService = gpSystem.GetLinkedAccountsService();
                    var gpSessionDetails = new GpLinkedAccountModel(userSession.GpUserSession, patientId);
                    var result = linkedAccountsService.GetProxyAuditData(gpSessionDetails);

                    context.SetLinkedAccountAuditInfo(result);
                }
            });

            await _next(context);
        }

        private bool TryParsePatientId(HttpContext context, out Guid patientId)
        {
            string patientIdHeader = context.Request.Headers[PatientId];
            if (patientIdHeader == null)
            {
                // These two requests path are being called in the Startup before the patient id has been set
                if (!_patientIdHeaderNotExpectedForPaths.Contains(context.Request.Path))
                {
                    _logger.LogWarning("{PatientIdHeaderName} Header not found", PatientId);
                }
                patientId = default;
                return false;
            }

            if (Guid.TryParse(patientIdHeader, out patientId))
            {
                return true;
            }

            _logger.LogError("Could not parse Guid from {PatientIdHeaderName} Header value: {PatientIdHeader}", PatientId, patientIdHeader);
            return false;
        }
    }
}