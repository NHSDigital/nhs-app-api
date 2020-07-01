using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Metrics;
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
        private readonly IErrorReferenceGenerator _errorReferenceGenerator;
        private readonly IAntiforgery _antiforgery;
        private readonly ISessionCreator _sessionCreator;
        private readonly UserSessionService _userSessionService;
        private readonly IUserSessionManager _userSessionManager;
        private readonly IMetricLogger _metricLogger;

        public SessionController(
            ConfigurationSettings settings,
            ILogger<SessionController> logger,
            IErrorReferenceGenerator errorReferenceGenerator,
            IAntiforgery antiforgery,
            ISessionCreator sessionCreator,
            UserSessionService userSessionService,
            IUserSessionManager userSessionManager,
            IMetricLogger metricLogger)
        {
            _settings = settings;
            _logger = logger;
            _errorReferenceGenerator = errorReferenceGenerator;
            _antiforgery = antiforgery;
            _sessionCreator = sessionCreator;
            _userSessionService = userSessionService;
            _userSessionManager = userSessionManager;
            _metricLogger = metricLogger;
        }

        [HttpGet]
        public IActionResult Get([UserSession] P5UserSession userSession)
        {
            try
            {
                _logger.LogEnter();

                var responseBody = userSession.Accept(new CreateResponseFromUserSessionVisitor<UserSessionResponse>(_settings, new UserSessionResponse()));

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
                    return BuildErrorResult(new ErrorTypes.LoginBadRequest());
                }

                var csrfToken = _antiforgery.GetTokens(HttpContext).RequestToken;
                var request = new CreateSessionRequest(model, csrfToken, HttpContext);
                var result = await _sessionCreator.CreateSession(request);

                return await result.Accept(new SessionCreateResultVisitor(this));
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

        private IActionResult BuildErrorResult(ErrorTypes errorTypes)
        {
            var serviceDeskReference = _errorReferenceGenerator.GenerateAndLogErrorReference(errorTypes);

            return new ObjectResult(new PfsErrorResponse { ServiceDeskReference = serviceDeskReference })
            {
                StatusCode = errorTypes.StatusCode
            };
        }

        private sealed class SessionCreateResultVisitor : ICreateSessionResultVisitor<Task<IActionResult>>
        {
            private readonly SessionController _controller;

            public SessionCreateResultVisitor(SessionController controller) => _controller = controller;

            private UserSessionService UserSessionService => _controller._userSessionService;
            private ILogger Logger => _controller._logger;
            private ConfigurationSettings Settings => _controller._settings;
            private HttpContext HttpContext => _controller.HttpContext;

            public async Task<IActionResult> Visit(CreateSessionResult.Success success)
            {
                var userSession = success.UserSession;
                var serviceJourneyRules = success.ServiceJourneyRules;

                await AppendCookieToResponse(userSession.Key);

                UserSessionService.SetUserSession(userSession);
                Logger.LogInformation($"Created {userSession.GetType().Name}");

                var responseBody = new PostUserSessionResponse { ServiceJourneyRules = serviceJourneyRules };
                responseBody = userSession.Accept(new CreateResponseFromUserSessionVisitor<PostUserSessionResponse>(Settings, responseBody));

                await _controller._metricLogger.Login();

                return new CreatedResult(string.Empty, responseBody);
            }

            public Task<IActionResult> Visit(CreateSessionResult.Error error)
                => Task.FromResult(_controller.BuildErrorResult(error.ErrorTypes));

            private async Task AppendCookieToResponse(string sessionId)
            {
                var claims = new List<Claim>
                {
                    new Claim(Constants.ClaimTypes.SessionId, sessionId)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));
            }
        }
    }
}