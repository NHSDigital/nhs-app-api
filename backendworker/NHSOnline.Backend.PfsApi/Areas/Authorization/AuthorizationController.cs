using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.Auth.CitizenId;
using NHSOnline.Backend.GpSystems.SessionManager;
using NHSOnline.Backend.PfsApi.Session;
using NHSOnline.Backend.Support.AspNet;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.Areas.Authorization
{
    [ApiVersionRoute("patient/authorization")]
    public class AuthorizationController : Controller
    {
        private readonly ICitizenIdService _citizenIdService;
        private readonly IAuditor _auditor;
        private readonly ISessionCacheService _sessionCacheService;
        private readonly ILogger<AuthorizationController> _logger;

        public AuthorizationController(
            ICitizenIdService citizenIdService,
            IAuditor auditor,
            ISessionCacheService sessionCacheService,
            ILogger<AuthorizationController> logger
            )
        {
            _citizenIdService = citizenIdService;
            _auditor = auditor;
            _sessionCacheService = sessionCacheService;
            _logger = logger;
        }

        [HttpPost]
        [Route("access-token/refresh")]
        public async Task<IActionResult> RefreshAccessToken([UserSession] P5UserSession userSession)
        {
            try
            {
                _logger.LogEnter();
                await _auditor.Audit(AuditingOperations.PostRefreshPatientAccessTokenRequest,
                    "Attempting to refresh access token");

                var result = await _citizenIdService.RefreshAccessToken(userSession.CitizenIdUserSession.RefreshToken);

                await result.Accept(new RefreshTokenResultAuditVisitor(_auditor, _logger));
                return await result.Accept(new RefreshTokenResultVisitor(_sessionCacheService, userSession, _logger));
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}