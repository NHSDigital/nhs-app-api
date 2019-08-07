using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.Support.AspNet;
using static NHSOnline.Backend.Support.Constants.HttpHeaders;

namespace NHSOnline.Backend.PfsApi.Filters
{
    public class ProxyAuditingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IGpSystemFactory _gpSystemFactory;
        private readonly ILogger<ProxyAuditingMiddleware> _logger;

        public ProxyAuditingMiddleware(RequestDelegate next, IGpSystemFactory gpSystemFactory, ILogger<ProxyAuditingMiddleware> logger)
        {
            _next = next;
            _gpSystemFactory = gpSystemFactory;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var userSession = context.GetUserSession();

            if (userSession != null)
            {
                var gpSystem = _gpSystemFactory.CreateGpSystem(userSession.GpUserSession.Supplier);

                if (gpSystem.SupportsLinkedAccounts && TryParsePatientId(context, out var patientId))
                {
                    var linkedAccountsService = gpSystem.GetLinkedAccountsService();
                    var result = linkedAccountsService.GetProxyAuditData(userSession.GpUserSession, patientId);

                    context.SetLinkedAccountAuditInfo(result);
                }
            }

            await _next(context);
        }

        private bool TryParsePatientId(HttpContext context, out Guid patientId)
        {
            string patientIdHeader = context.Request.Headers[PatientId];
            if (patientIdHeader == null)
            {
                _logger.LogWarning("{PatientIdHeaderName} Header not found", PatientId);
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