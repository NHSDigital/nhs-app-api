using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        private readonly ISessionCacheService _sessionCacheService;
        private readonly ILogger<AuthorizationController> _logger;

        public AuthorizationController(
            ICitizenIdService citizenIdService,
            ISessionCacheService sessionCacheService,
            ILogger<AuthorizationController> logger)
        {
            _citizenIdService = citizenIdService;
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

                var result = await _citizenIdService.RefreshAccessToken(userSession.CitizenIdUserSession.RefreshToken);

                return await result.Accept(new RefreshTokenResultVisitor(_sessionCacheService, userSession, _logger));
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}