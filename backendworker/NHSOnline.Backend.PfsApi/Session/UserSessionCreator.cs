using System.Threading.Tasks;
using NHSOnline.Backend.PfsApi.CitizenId;
using NHSOnline.Backend.ServiceJourneyRulesApi.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.Session
{
    internal sealed class UserSessionCreator
    {
        private readonly P9UserSessionCreator _p9UserSessionCreator;
        private readonly P5UserSessionCreator _p5UserSessionCreator;

        public UserSessionCreator(P9UserSessionCreator p9UserSessionCreator, P5UserSessionCreator p5UserSessionCreator)
        {
            _p9UserSessionCreator = p9UserSessionCreator;
            _p5UserSessionCreator = p5UserSessionCreator;
        }

        public async Task<CreateUserSessionResult> Create(
            ServiceJourneyRulesResponse serviceJourneyRules,
            CitizenIdSessionResult citizenIdSessionResult,
            string csrfToken)
        {
            switch (citizenIdSessionResult.Session.ProofLevel)
            {
                case ProofLevel.P9:
                    return await _p9UserSessionCreator.Create(serviceJourneyRules, citizenIdSessionResult, csrfToken);
                case ProofLevel.P5:
                    return await _p5UserSessionCreator.Create(citizenIdSessionResult, csrfToken);
                default:
                    return CreateUserSessionResult.Failed(new ErrorTypes.UnhandledError());
            }
        }
    }
}