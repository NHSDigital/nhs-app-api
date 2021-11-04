using System.Threading.Tasks;
using NHSOnline.Backend.PfsApi.CitizenId;
using NHSOnline.Backend.PfsApi.GpSession;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.Session
{
    internal sealed class SessionCreator : ISessionCreator
    {
        private readonly SessionCreatorCitizenIdService _citizenIdService;
        private readonly SessionCreatorServiceJourneyRuleService _serviceJourneyRulesService;
        private readonly IUserSessionManager _userSessionManager;
        private readonly SessionCreatorUserInfoService _userInfoService;
        private readonly IGpSessionCreator _gpSessionCreator;

        public SessionCreator(
            SessionCreatorCitizenIdService citizenIdService,
            SessionCreatorServiceJourneyRuleService serviceJourneyRulesService,
            IUserSessionManager userSessionManager,
            SessionCreatorUserInfoService userInfoService,
            IGpSessionCreator gpSessionCreator)
        {
            _citizenIdService = citizenIdService;
            _serviceJourneyRulesService = serviceJourneyRulesService;
            _userSessionManager = userSessionManager;
            _userInfoService = userInfoService;
            _gpSessionCreator = gpSessionCreator;
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
            if (request.UserSession.GpUserSession.Supplier != Supplier.Disconnected)
            {
                return new CreateSessionResult.GpSessionExists(request.UserSession);
            }

            var citizenIdSession = await _citizenIdService.FetchUserProfile(request);
            if (citizenIdSession.Failed(out var citizenIdSessionFailure))
            {
                return citizenIdSessionFailure;
            }

            request.UserSession.Im1ConnectionToken = GetIm1ConnectionToken(citizenIdSession);

            var supplier = ((OnDemandGpSession) request.UserSession.GpUserSession).SessionSupplier;

            var createGpSessionResult = await _gpSessionCreator.RecreateGpSession(request.UserSession, supplier);

            switch (createGpSessionResult)
            {
                case GpSessionRecreateResult.RecreatedResult _:
                case GpSessionRecreateResult.SessionStillValidResult _:
                    return new CreateSessionResult.Success(request.UserSession);
                case GpSessionRecreateResult.ErrorResult errorResult:
                    return new CreateSessionResult.ErrorResult(new ErrorTypes.GPSessionUnavailable(errorResult.ErrorType));
                default:
                    return new CreateSessionResult.ErrorResult(new ErrorTypes.GPSessionUnavailable());
            }
        }

        private static string GetIm1ConnectionToken(CitizenIdSessionResult citizenIdSessionResult)
        {
            return citizenIdSessionResult.Im1ConnectionToken;
        }
    }
}
