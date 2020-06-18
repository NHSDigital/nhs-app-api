using System.Threading.Tasks;

namespace NHSOnline.Backend.PfsApi.Session
{
    internal sealed class SessionCreator : ISessionCreator
    {
        private readonly SessionCreatorCitizenIdService _citizenIdService;
        private readonly SessionCreatorServiceJourneyRuleService _serviceJourneyRulesService;
        private readonly IUserSessionManager _userSessionManager;
        private readonly SessionCreatorUserInfoService _userInfoService;

        public SessionCreator(
            SessionCreatorCitizenIdService citizenIdService,
            SessionCreatorServiceJourneyRuleService serviceJourneyRulesService,
            IUserSessionManager userSessionManager,
            SessionCreatorUserInfoService userInfoService)
        {
            _citizenIdService = citizenIdService;
            _serviceJourneyRulesService = serviceJourneyRulesService;
            _userSessionManager = userSessionManager;
            _userInfoService = userInfoService;
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
    }
}