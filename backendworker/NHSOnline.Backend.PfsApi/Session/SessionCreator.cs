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

            if (citizenIdSession.ProcessFinishedEarly(out var citizenIdSessionResult))
            {
                return citizenIdSessionResult;
            }

            var serviceJourneyRules = await _serviceJourneyRulesService.Fetch(citizenIdSession.Result);
            if (serviceJourneyRules.ProcessFinishedEarly(out var serviceJourneyRulesResult))
            {
                return serviceJourneyRulesResult;
            }

            var userSession = await _userSessionManager.Create(citizenIdSession.Result, serviceJourneyRules.Result, request.CsrfToken);
            if (userSession.ProcessFinishedEarly(out var userSessionResult))
            {
                return userSessionResult;
            }

            await _userInfoService.Update(request, serviceJourneyRules.Result, citizenIdSession.Result);

            return new CreateSessionResult.Success(serviceJourneyRules.Result, userSession.Result);
        }
    }
}