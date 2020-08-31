using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.SessionManager;
using NHSOnline.Backend.PfsApi.CitizenId;
using NHSOnline.Backend.PfsApi.GpSession;
using NHSOnline.Backend.ServiceJourneyRulesApi.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.Session
{
    public sealed class P9UserSessionCreator
    {
        private readonly ISessionCacheService _sessionCacheService;
        private readonly IAuditor _auditor;
        private readonly ILogger<P9UserSessionCreator> _logger;
        private readonly IGpSessionCreator _gpSessionCreator;
        private readonly IIm1CacheService _im1CacheService;

        public P9UserSessionCreator(
            ISessionCacheService sessionCacheService,
            IAuditor auditor,
            ILogger<P9UserSessionCreator> logger,
            IGpSessionCreator gpSessionCreator,
            IIm1CacheService im1CacheService)
        {
            _sessionCacheService = sessionCacheService;
            _auditor = auditor;
            _gpSessionCreator = gpSessionCreator;
            _logger = logger;
            _im1CacheService = im1CacheService;
        }

        public async Task<CreateUserSessionResult> Create(
            CitizenIdSessionResult citizenIdSessionResult,
            ServiceJourneyRulesResponse serviceJourneyRules,
            string csrfToken)
        {
            return await _auditor.Audit()
                .AccessToken(citizenIdSessionResult.Session.AccessToken)
                .NhsNumber(citizenIdSessionResult.NhsNumber)
                .Supplier(serviceJourneyRules.Journeys.Supplier)
                .Operation(AuditingOperations.GpSessionCreate)
                .Details("Attempting to create Session")
                .Execute(() => CreateP9UserSession(
                    citizenIdSessionResult,
                    serviceJourneyRules.Journeys.Supplier,
                    citizenIdSessionResult.Session.OdsCode,
                    csrfToken));
        }

        private async Task<CreateUserSessionResult> CreateP9UserSession(
            CitizenIdSessionResult citizenIdSessionResult,
            Supplier supplier,
            string odsCode,
            string csrfToken)
        {
            if (supplier == Supplier.Unknown)
            {
                return new CreateUserSessionResult.Failure(
                    new ErrorTypes.LoginOdsCodeNotFoundOrNotSupported(),
                    $"Failed to determine the GP system based on ODS code '{odsCode}'");
            }

            var gpUserSession = await _gpSessionCreator.CreateGpSession(citizenIdSessionResult, supplier);

            var userSession = await CreateP9UserSession(citizenIdSessionResult, gpUserSession, csrfToken);

            await DeleteConnectionTokenFromCache(citizenIdSessionResult.Im1ConnectionToken);

            return CreateUserSessionResult.Succeeded(userSession);
        }

        private async Task<P9UserSession> CreateP9UserSession(CitizenIdSessionResult citizenIdSessionResult,
            GpUserSession gpUserSession, string csrfToken)
        {
            var userSession = new P9UserSession(
                csrfToken,
                citizenIdSessionResult.NhsNumber,
                citizenIdSessionResult.Session,
                gpUserSession,
                citizenIdSessionResult.Im1ConnectionToken);

            var sessionId = await _sessionCacheService.CreateUserSession(userSession);
            _logger.LogDebug($"Created Session Id: '{sessionId}'");
            return userSession;
        }

        private async Task DeleteConnectionTokenFromCache(string im1ConnectionToken)
        {

            _logger.LogInformation("DeleteConnectionTokenFromCache");

            if (im1ConnectionToken is null)
            {
                _logger.LogInformation("Im1 connection token is null.");
                return;
            }

            if (Guid.TryParse(im1ConnectionToken, out _))
            {
                return;
            }

            var tokenObject = JObject.Parse(im1ConnectionToken);

            if (tokenObject.TryGetValue(Im1CacheService.Im1ConnectionTokenCacheKeyPropertyName,
                StringComparison.Ordinal,
                out var cacheKeyToken))
            {
                var cacheKey = cacheKeyToken.ToString();
                if (!string.IsNullOrEmpty(cacheKey))
                {
                    await _im1CacheService.DeleteIm1ConnectionToken(cacheKey);
                }
            }
        }
    }
}