using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.PfsApi.Areas.Session.Models;
using NHSOnline.Backend.PfsApi.Session;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.AspNet;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Support.Session;
using NHSOnline.Backend.Support.Settings;

namespace NHSOnline.Backend.PfsApi.Areas.Session
{
    [ApiVersionRoute("session")]
    public class SessionController : Controller
    {
        private readonly ConfigurationSettings _settings;
        private readonly ILogger<SessionController> _logger;
        private readonly ISessionErrorResultBuilder _errorResultBuilder;
        private readonly IAntiforgery _antiforgery;
        private readonly ISessionCreator _sessionCreator;
        private readonly IUserSessionManager _userSessionManager;
        private readonly ICreateSessionResultVisitor<Task<IActionResult>> _sessionResultVisitor;
        private readonly ISessionExpiryCookieCreator _sessionExpiryCookieCreator;

        public SessionController(
            ConfigurationSettings settings,
            ILogger<SessionController> logger,
            ISessionErrorResultBuilder errorResultBuilder,
            IAntiforgery antiforgery,
            ISessionCreator sessionCreator,
            IUserSessionManager userSessionManager,
            ICreateSessionResultVisitor<Task<IActionResult>> sessionResultVisitor,
            ISessionExpiryCookieCreator sessionExpiryCookieCreator)
        {
            _settings = settings;
            _logger = logger;
            _errorResultBuilder = errorResultBuilder;
            _antiforgery = antiforgery;
            _sessionCreator = sessionCreator;
            _userSessionManager = userSessionManager;
            _sessionResultVisitor = sessionResultVisitor;
            _sessionExpiryCookieCreator = sessionExpiryCookieCreator;
        }

        [HttpGet]
        public IActionResult Get([UserSession] P5UserSession userSession)
        {
            try
            {
                _logger.LogEnter();

                var responseBody = userSession.Accept(
                    new CreateResponseFromUserSessionVisitor<UserSessionResponse>(
                        _settings,
                        new UserSessionResponse()));

                return new OkObjectResult(responseBody);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> Post([FromBody] UserSessionRequest model)
        {
            try
            {
                _logger.LogEnter();

                if (!ModelState.IsValid)
                {
                    return new BadRequestObjectResult(ModelState);
                }

                var validator = new SessionValidator(_logger);

                if (!validator.IsPostValid(model))
                {
                    return _errorResultBuilder.BuildResult(new ErrorTypes.LoginBadRequest());
                }

                var sessionExpiryCookieToken = _sessionExpiryCookieCreator.CreateSessionExpiryToken();

                if (sessionExpiryCookieToken is null)
                {
                    return _errorResultBuilder.BuildResult(new ErrorTypes.LoginUnexpectedError());
                }

                var csrfToken = _antiforgery.GetTokens(HttpContext).RequestToken;
                var request = new CreateSessionRequest(model, csrfToken, HttpContext);
                var result = await _sessionCreator.CreateSession(request);

                var referrer = "";

                if (model?.Referrer != null)
                {
                    referrer = model.Referrer;
                }

                return await result.Accept(_sessionResultVisitor, HttpContext, sessionExpiryCookieToken, referrer);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([UserSession] UserSession userSession)
        {
            try
            {
                _logger.LogEnter();

                var result = await _userSessionManager.Delete(HttpContext, userSession);

                return result.Accept(new DeleteUserSessionResponseVisitor());
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}
