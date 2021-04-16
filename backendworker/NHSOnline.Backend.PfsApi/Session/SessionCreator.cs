using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.GpSystems.SessionManager;
using NHSOnline.Backend.PfsApi.Areas.Session;
using NHSOnline.Backend.PfsApi.CitizenId;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.Session
{
    internal sealed class SessionCreator : ISessionCreator
    {
        private readonly SessionCreatorCitizenIdService _citizenIdService;
        private readonly SessionCreatorServiceJourneyRuleService _serviceJourneyRulesService;
        private readonly IUserSessionManager _userSessionManager;
        private readonly SessionCreatorUserInfoService _userInfoService;
        private readonly ISessionCacheService _sessionCacheService;
        private readonly ILogger<SessionCreator> _logger;


        public SessionCreator(
            SessionCreatorCitizenIdService citizenIdService,
            SessionCreatorServiceJourneyRuleService serviceJourneyRulesService,
            IUserSessionManager userSessionManager,
            SessionCreatorUserInfoService userInfoService,
            ISessionCacheService sessionCacheService,
            ILogger<SessionCreator> logger)
        {
            _citizenIdService = citizenIdService;
            _serviceJourneyRulesService = serviceJourneyRulesService;
            _userSessionManager = userSessionManager;
            _userInfoService = userInfoService;
            _sessionCacheService = sessionCacheService;
            _logger = logger;
        }

        public async Task<CreateSessionResult> CreateSession(ICreateSessionRequest request)
        {
            var citizenIdSession = await _citizenIdService.FetchUserProfile(request);
            if (citizenIdSession.Failed(out var citizenIdSessionFailure))
            {
                return citizenIdSessionFailure;
            }

            var serviceJourneyRules = await _serviceJourneyRulesService.Fetch(citizenIdSession);
            if (serviceJourneyRules.Failed(out var serviceJourneyRulesFailure))
            {
                return serviceJourneyRulesFailure;
            }

            var userSession = await _userSessionManager.Create(citizenIdSession, serviceJourneyRules, request.CsrfToken);
            if (userSession.Failed(out var userSessionFailure))
            {
                return userSessionFailure;
            }

            await _userInfoService.Update(request, serviceJourneyRules, citizenIdSession);

            return new CreateSessionResult.Success(serviceJourneyRules, userSession);
        }


        public async Task<CreateSessionResult> CreateGpSessionOnDemand(ICreateGpSessionOnDemandRequest request)
        {
            var citizenIdSession = await _citizenIdService.FetchUserProfile(request);
            if (citizenIdSession.Failed(out var citizenIdSessionFailure))
            {
                return citizenIdSessionFailure;
            }

            if (!DoNhsLoginIdsMatch(citizenIdSession, request.UserSession))
            {
                _logger.LogError("NhsLoginId from UserProfile does not match value in existing P9UserSession");
                return new CreateSessionResult.ErrorResult(new ErrorTypes.LoginForbidden());
            }

            request.UserSession.Im1ConnectionToken = GetIm1ConnectionToken(citizenIdSession);
            await _sessionCacheService.UpdateUserSession(request.UserSession);

            return new CreateSessionResult.Success(request.UserSession);
        }

        private static string GetIm1ConnectionToken(CitizenIdSessionResult citizenIdSessionResult)
        {
            return citizenIdSessionResult.Im1ConnectionToken;
        }

        private string GetNhsLoginId(CitizenIdUserSession citizenIdUserSession)
            => AccessToken.Parse(_logger, citizenIdUserSession.AccessToken).Subject;

        private bool DoNhsLoginIdsMatch(CitizenIdSessionResult citizenIdSessionResult, P9UserSession userSession)
        {
            return string.Equals(
                GetNhsLoginId(citizenIdSessionResult.Session), GetNhsLoginId(userSession.CitizenIdUserSession),
                System.StringComparison.Ordinal);
        }
    }
}
