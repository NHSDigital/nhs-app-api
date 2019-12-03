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

                if (gpSystem.SupportsLinkedAccounts)
                {
                    try
                    {
                        var patientId = Guid.Parse(context.Request.Headers[PatientId]);
                        var linkedAccountsService = gpSystem.GetLinkedAccountsService();
                        var result = linkedAccountsService.GetProxyAuditData(userSession.GpUserSession, patientId);

                        context.SetLinkedAccountAuditInfo(result);
                    }
                    catch (ArgumentNullException e)
                    {
                        _logger.LogWarning(e, "PatientId Header not found");
                    }
                    catch (FormatException e)
                    {
                        _logger.LogError(e, "Could not parse Guid from PatientId Header value");
                    }
                }
            }

            await _next(context);
        }
    }
}