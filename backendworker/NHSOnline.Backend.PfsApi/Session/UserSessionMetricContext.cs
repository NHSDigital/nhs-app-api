using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.Metrics;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.Session
{
    internal sealed class UserSessionMetricContext: IMetricContext
    {
        private readonly ILogger _logger;
        private readonly IUserSessionService _userSessionService;

        public UserSessionMetricContext(
            ILogger<UserSessionMetricContext> logger,
            IUserSessionService userSessionService)
        {
            _logger = logger;
            _userSessionService = userSessionService;
        }

        public string NhsLoginId => AccessToken.Parse(_logger, CitizenIdUserSession.AccessToken).Subject;
        public ProofLevel ProofLevel => CitizenIdUserSession.ProofLevel;
        public string OdsCode => UserSession.OdsCode;

        private CitizenIdUserSession CitizenIdUserSession => UserSession.CitizenIdUserSession;
        private P5UserSession UserSession => _userSessionService.GetRequiredUserSession<P5UserSession>();
    }
}
