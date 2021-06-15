using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.GpSession
{
    public class GpSessionRecreateVisitor : IGpUserSessionVisitor<Task<GpSessionRecreateResult>>
    {
        private readonly ILogger _logger;
        private readonly IGpSessionCreator _gpSessionCreator;
        private readonly P9UserSession _p9UserSession;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GpSessionRecreateVisitor(
            ILogger logger,
            IGpSessionCreator gpSessionCreator,
            P9UserSession p9UserSession,
            IHttpContextAccessor httpContextAccessor)
        {
            _gpSessionCreator = gpSessionCreator;
            _p9UserSession = p9UserSession;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<GpSessionRecreateResult> Visit(NullGpSession nullGpSession)
        {
            _logger.LogInformation("Invalid GP session detected, attempting to recreate GP user session");

            return await _gpSessionCreator.RecreateGpSession(_p9UserSession, nullGpSession.SessionSupplier);
        }

        public Task<GpSessionRecreateResult> Visit(GpUserSession gpSession)
        {
            _logger.LogInformation("Valid GP session present");

            return Task.FromResult<GpSessionRecreateResult>(new GpSessionRecreateResult.SessionStillValidResult());
        }

        public async Task<GpSessionRecreateResult> Visit(OnDemandGpSession gpSession)
        {
            if (string.Equals("/v1/patient/configuration", _httpContextAccessor.HttpContext?.Request.Path, StringComparison.Ordinal))
            {
                _logger.LogInformation("OnDemandGpSession, valid GP session not yet needed for request /v1/patient/configuration");
                return await Task.FromResult<GpSessionRecreateResult>(new GpSessionRecreateResult.SessionNotRequiredResult());
            }

            if (string.IsNullOrEmpty(_p9UserSession.Im1ConnectionToken))
            {
                _logger.LogInformation($"OnDemandGpSession, empty Im1ConnectionToken detected when requesting " +
                                       $"{_httpContextAccessor.HttpContext?.Request.Path}");
                return await Task.FromResult<GpSessionRecreateResult>(new GpSessionRecreateResult.Im1ConnectionTokenEmptyResult());
            }

            _logger.LogInformation(
                "OnDemandGpSession, attempting to recreate a " +
                $"{gpSession.SessionSupplier} GpUserSession for {_httpContextAccessor.HttpContext?.Request.Path}");
            var result = await _gpSessionCreator.RecreateGpSession(_p9UserSession, gpSession.SessionSupplier);

            _httpContextAccessor.HttpContext?.Response.Headers.Add(Constants.HttpHeaders.Im1MessagingEnabled, _p9UserSession.GpUserSession.Im1MessagingEnabled.ToString());
            _httpContextAccessor.HttpContext?.Response.Headers.Add(Constants.HttpHeaders.GpSessionCreated, "true");

            return result;
        }
    }
}
