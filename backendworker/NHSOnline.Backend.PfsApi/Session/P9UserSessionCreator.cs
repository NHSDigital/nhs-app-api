using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.Session;
using NHSOnline.Backend.GpSystems.SessionManager;
using NHSOnline.Backend.PfsApi.Areas.Session;
using NHSOnline.Backend.PfsApi.CitizenId;
using NHSOnline.Backend.ServiceJourneyRulesApi.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.Session
{
    internal sealed class P9UserSessionCreator
    {
        private readonly ISessionCacheService _sessionCacheService;
        private readonly IAuditor _auditor;
        private readonly ILogger<UserSessionManager> _logger;
        private readonly IGpSystemFactory _gpSystemFactory;
        private readonly IGpSessionManager _gpSessionManager;
        private readonly IIm1CacheService _im1CacheService;

        public P9UserSessionCreator(
            ISessionCacheService sessionCacheService,
            IAuditor auditor,
            ILogger<UserSessionManager> logger,
            IGpSystemFactory gpSystemFactory,
            IGpSessionManager gpSessionManager,
            IIm1CacheService im1CacheService)
        {
            _sessionCacheService = sessionCacheService;
            _auditor = auditor;
            _logger = logger;
            _gpSystemFactory = gpSystemFactory;
            _gpSessionManager = gpSessionManager;
            _im1CacheService = im1CacheService;
        }

        public async Task<CreateUserSessionResult> Create(
            ServiceJourneyRulesResponse serviceJourneyRules,
            CitizenIdSessionResult citizenIdSessionResult,
            string csrfToken)
        {
            if (SupplierIsUnknown(serviceJourneyRules))
            {
                return FailedToDetermineGpSystem(citizenIdSessionResult);
            }

            var gpSystem = FetchGpSystem(serviceJourneyRules);

            await AuditAttemptToCreateSession(citizenIdSessionResult, gpSystem);

            if (IsInvalidIm1ConnectionToken(gpSystem, citizenIdSessionResult.Im1ConnectionToken))
            {
                return await FailedToValidateIm1Connection(citizenIdSessionResult, gpSystem);
            }

            var gpSessionCreateResult = await _gpSessionManager.CreateSession(new GpSessionCreateArgs(gpSystem, citizenIdSessionResult));
            if (!(gpSessionCreateResult is GpSessionCreateResult.Success gpSessionCreateSuccess))
            {
                return await FailedToCreateGpSession(citizenIdSessionResult, gpSessionCreateResult, gpSystem);
            }

            var userSession = await CreateP9UserSession(citizenIdSessionResult, csrfToken, gpSessionCreateSuccess);

            await DeleteConnectionTokenFromCache(citizenIdSessionResult.Im1ConnectionToken);

            return CreateUserSessionResult.Succeeded(userSession);
        }

        private static bool SupplierIsUnknown(ServiceJourneyRulesResponse serviceJourneyRules)
            => serviceJourneyRules.Journeys.Supplier == Supplier.Unknown;

        private CreateUserSessionResult FailedToDetermineGpSystem(CitizenIdSessionResult citizenIdSessionResult)
        {
            _logger.LogError(
                $"Failed to determine the GP system based on ODS code '{citizenIdSessionResult.Session.OdsCode}'");

            return CreateUserSessionResult.Failed(new ErrorTypes.LoginOdsCodeNotFoundOrNotSupported());
        }

        private IGpSystem FetchGpSystem(ServiceJourneyRulesResponse serviceJourneyRules)
        {
            var gpSystem = _gpSystemFactory.CreateGpSystem(serviceJourneyRules.Journeys.Supplier);
            _logger.LogDebug($"Fetch GP System: '{gpSystem.Supplier}'.");
            return gpSystem;
        }

        private async Task AuditAttemptToCreateSession(CitizenIdSessionResult citizenIdSessionResult, IGpSystem gpSystem)
        {
            await _auditor.AuditSessionEvent(
                citizenIdSessionResult.Session.AccessToken,
                citizenIdSessionResult.NhsNumber,
                gpSystem.Supplier,
                AuditingOperations.SessionCreateRequest,
                "Attempting to create Session");
        }

        private static bool IsInvalidIm1ConnectionToken(IGpSystem gpSystem, string im1ConnectionToken)
        {
            var tokenValidationService = gpSystem.GetTokenValidationService();
            var isInvalidIm1ConnectionToken = !tokenValidationService.IsValidConnectionTokenFormat(im1ConnectionToken);
            return isInvalidIm1ConnectionToken;
        }

        private async Task<CreateUserSessionResult> FailedToValidateIm1Connection(CitizenIdSessionResult citizenIdSessionResult, IGpSystem gpSystem)
        {
            const string errorMessage = "Failed to validate Im1 connection";
            _logger.LogError(errorMessage);

            await _auditor.AuditSessionEvent(
                citizenIdSessionResult.Session.AccessToken,
                citizenIdSessionResult.NhsNumber,
                gpSystem.Supplier,
                AuditingOperations.SessionCreateResponse,
                errorMessage);

            return CreateUserSessionResult.Failed(new ErrorTypes.LoginForbidden());
        }

        private async Task<CreateUserSessionResult> FailedToCreateGpSession(CitizenIdSessionResult citizenIdSessionResult,
            GpSessionCreateResult gpSessionCreateResult, IGpSystem gpSystem)
        {
            var failureStatusCode = gpSessionCreateResult.StatusCode;

            var errorMessage = $"Creating the session failed with status code: '{failureStatusCode}'";
            _logger.LogError(errorMessage);
            await _auditor.AuditSessionEvent(
                citizenIdSessionResult.Session.AccessToken,
                citizenIdSessionResult.NhsNumber,
                gpSystem.Supplier,
                AuditingOperations.SessionCreateResponse,
                errorMessage);

            // 502 Bad gateway error references differ by supplier. The other error types do not.
            return failureStatusCode == StatusCodes.Status502BadGateway
                ? CreateUserSessionResult.Failed(ErrorTypes.LoginBadGateway(_logger, gpSystem.Supplier))
                : CreateUserSessionResult.Failed(ErrorTypes.LookupErrorType(_logger, ErrorCategory.Login, failureStatusCode));
        }

        private async Task<P9UserSession> CreateP9UserSession(CitizenIdSessionResult citizenIdSessionResult, string csrfToken, GpSessionCreateResult.Success result)
        {
            var userSession = new P9UserSession(
                csrfToken,
                citizenIdSessionResult.Session,
                result.UserSession, citizenIdSessionResult.Im1ConnectionToken);

            var sessionId = await _sessionCacheService.CreateUserSession(userSession);
            _logger.LogDebug($"Created Session Id: '{sessionId}'");
            return userSession;
        }

        private async Task DeleteConnectionTokenFromCache(string im1ConnectionToken)
        {
            if (Guid.TryParse(im1ConnectionToken, out _))
            {
                return;
            }

            var tokenObject = JObject.Parse(im1ConnectionToken);

            if (tokenObject.TryGetValue(Im1CacheService.Im1ConnectionTokenCacheKeyPropertyName,
                StringComparison.Ordinal,
                out var cacheKeyToken))
            {
                var cacheKey = cacheKeyToken?.ToString();
                if (!string.IsNullOrEmpty(cacheKey))
                {
                    await _im1CacheService.DeleteIm1ConnectionToken(cacheKey);
                }
            }
        }

    }
}