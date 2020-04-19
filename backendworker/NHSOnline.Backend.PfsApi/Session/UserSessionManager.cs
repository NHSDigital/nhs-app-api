using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.Session;
using NHSOnline.Backend.GpSystems.SessionManager;
using NHSOnline.Backend.PfsApi.Areas.Session;
using NHSOnline.Backend.PfsApi.CitizenId;
using NHSOnline.Backend.ServiceJourneyRulesApi.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.Session
{
    public class UserSessionManager : IUserSessionManager
    {
        private readonly IUserSessionService _userSessionService;
        private readonly ISessionCacheService _sessionCacheService;
        private readonly IAuditor _auditor;
        private readonly ILogger<UserSessionManager> _logger;
        private readonly IGpSystemFactory _gpSystemFactory;
        private readonly IGpSessionManager _gpSessionManager;

        public UserSessionManager(
            IUserSessionService userSessionService,
            ISessionCacheService sessionCacheService,
            IAuditor auditor,
            ILogger<UserSessionManager> logger,
            IGpSystemFactory gpSystemFactory,
            IGpSessionManager gpSessionManager)
        {
            _userSessionService = userSessionService;
            _sessionCacheService = sessionCacheService;
            _auditor = auditor;
            _logger = logger;
            _gpSystemFactory = gpSystemFactory;
            _gpSessionManager = gpSessionManager;
        }

        public async Task<CreateUserSessionResult> Create(
            ServiceJourneyRulesResponse serviceJourneyRules,
            CitizenIdSessionResult citizenIdSessionResult,
            string csrfToken)
        {
            if (serviceJourneyRules.Journeys.Supplier == Supplier.Unknown)
            {
                _logger.LogError(
                    $"Failed to determine the GP system based on ODS code '{citizenIdSessionResult.OdsCode}'");

                return CreateUserSessionResult.Failed(new ErrorTypes.LoginOdsCodeNotFoundOrNotSupported());
            }

            var gpSystem = _gpSystemFactory.CreateGpSystem(serviceJourneyRules.Journeys.Supplier);
            _logger.LogDebug($"Fetch GP System: '{gpSystem.Supplier}'.");

            await _auditor.AuditSessionEvent(
                citizenIdSessionResult.Session.AccessToken,
                citizenIdSessionResult.NhsNumber,
                gpSystem.Supplier,
                AuditingOperations.SessionCreateRequest,
                "Attempting to create Session");

            // Validate the format of the IM1 connection token for this GP system.
            var tokenValidationService = gpSystem.GetTokenValidationService();
            if (!tokenValidationService.IsValidConnectionTokenFormat(citizenIdSessionResult.Im1ConnectionToken))
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

            return await GetUserSessionAndCreateSession(gpSystem, citizenIdSessionResult, csrfToken);
        }

        private async Task<CreateUserSessionResult> GetUserSessionAndCreateSession(
            IGpSystem gpSystem,
            CitizenIdSessionResult citizenIdSessionResult,
            string csrfToken)
        {
            var gpSessionCreateResult =
                await _gpSessionManager.CreateSession(new GpSessionCreateArgs(gpSystem, citizenIdSessionResult));

            if (!(gpSessionCreateResult is GpSessionCreateResult.Success result))
            {
                var failureStatusCode = gpSessionCreateResult.StatusCode;

                var errorMessage =
                    $"Creating the session failed with status code: '{failureStatusCode}'";
                _logger.LogError(errorMessage);
                await _auditor.AuditSessionEvent(
                    citizenIdSessionResult.Session.AccessToken,
                    citizenIdSessionResult.NhsNumber,
                    gpSystem.Supplier,
                    AuditingOperations.SessionCreateResponse,
                    errorMessage);

                // 502 Bad gateway error references differ by supplier. The other error types do not.
                var objectResult = failureStatusCode == StatusCodes.Status502BadGateway
                    ? CreateUserSessionResult.Failed(ErrorTypes.LoginBadGateway(_logger, gpSystem.Supplier))
                    : CreateUserSessionResult.Failed(ErrorTypes.LookupErrorType(_logger, ErrorCategory.Login, failureStatusCode));

                return objectResult;
            }

            var userSession = new P9UserSession(
                csrfToken,
                citizenIdSessionResult.Session,
                result.UserSession, citizenIdSessionResult.Im1ConnectionToken);

            var sessionId = await _sessionCacheService.CreateUserSession(userSession);
            _logger.LogDebug($"Created Session Id: '{sessionId}'");

            return CreateUserSessionResult.Succeeded(userSession);
        }

        public async Task<bool> SignOutAsync(HttpContext httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            var success = true;
            var userSession = _userSessionService.GetRequiredUserSession<P9UserSession>();

            var gpUserSession = userSession.GpUserSession;
            var citizenIdUserSession = userSession.CitizenIdUserSession;

            try
            {
                if (!await _sessionCacheService.DeleteUserSession(userSession.Key))
                {
                    _logger.LogError("No active session was found");
                }
            }
            catch (Exception e)
            {
                success = false;
                _logger.LogError(e, $"Delete session failed with error: {e.Message}");
                await _auditor.AuditSessionEvent(
                    citizenIdUserSession.AccessToken,
                    gpUserSession.NhsNumber,
                    gpUserSession.Supplier,
                    AuditingOperations.SessionDeleteResponse,
                    "Delete session failed");
            }

            await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!success)
            {
                return false;
            }

            _logger.LogDebug("Session successfully deleted.");
            await _auditor.AuditSessionEvent(
                citizenIdUserSession.AccessToken,
                gpUserSession.NhsNumber,
                gpUserSession.Supplier,
                AuditingOperations.SessionDeleteResponse,
                "Session successfully deleted"
            );

            return true;
        }
    }
}
