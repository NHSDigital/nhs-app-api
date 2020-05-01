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
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.GpSystems.SessionManager;
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
        private readonly IAuditor _auditor;
        private readonly IErrorReferenceGenerator _errorReferenceGenerator;
        private readonly IGpSessionManager _gpSessionManager;
        private readonly IAntiforgery _antiforgery;
        private readonly ISessionCreator _sessionCreator;
        private readonly UserSessionService _userSessionService;

        public SessionController(
            ConfigurationSettings settings,
            ILogger<SessionController> logger,
            IAuditor auditor,
            IErrorReferenceGenerator errorReferenceGenerator,
            IGpSessionManager gpSessionManager,
            IAntiforgery antiforgery,
            ISessionCreator sessionCreator,
            UserSessionService userSessionService)
        {
            _settings = settings;
            _logger = logger;
            _auditor = auditor;
            _errorReferenceGenerator = errorReferenceGenerator;
            _gpSessionManager = gpSessionManager;
            _antiforgery = antiforgery;
            _sessionCreator = sessionCreator;
            _userSessionService = userSessionService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([UserSession] P5UserSession userSession)
        {
            try
            {
                _logger.LogEnter();
                if (userSession is P9UserSession)
                {
                    await _auditor.Audit(AuditingOperations.SessionGetRequest, "Session Get called.");
                }

                var responseBody = userSession.Accept(new UserSessionResponseVisitor<UserSessionResponse>(_settings, new UserSessionResponse()));

                if (userSession is P9UserSession)
                {
                    await _auditor.Audit(AuditingOperations.SessionGetResponse, "Successfully retrieved session.");
                }

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
        public async Task<IActionResult> Delete([UserSession] P9UserSession userSession)
        {
            try
            {
                _logger.LogEnter();
                await _auditor.Audit(AuditingOperations.SessionDeleteRequest, "Session delete called.");

                // Delete GP supplier session
                var gpUserSession = userSession.GpUserSession;
                var citizenIdUserSession = userSession.CitizenIdUserSession;

                var closeSessionResult = await _gpSessionManager.CloseAndDeleteSession(userSession);

                if (closeSessionResult is CloseSessionResult.Failure)
                {
                    await _auditor.AuditSessionEvent(
                        citizenIdUserSession.AccessToken,
                        gpUserSession.NhsNumber,
                        gpUserSession.Supplier,
                        AuditingOperations.SessionDeleteResponse,
                        "Delete session failed");

                    return new StatusCodeResult(StatusCodes.Status500InternalServerError);
                }

                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                _logger.LogDebug(
                    $"Session successfully deleted. Finished with status code: {StatusCodes.Status204NoContent}");

                await _auditor.AuditSessionEvent(
                    citizenIdUserSession.AccessToken,
                    gpUserSession.NhsNumber,
                    gpUserSession.Supplier,
                    AuditingOperations.SessionDeleteResponse,
                    "Session successfully deleted");

                return new StatusCodeResult(StatusCodes.Status204NoContent);
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

        private sealed class CreateSessionRequest: ICreateSessionRequest
        {
            private readonly UserSessionRequest _model;

            internal CreateSessionRequest(UserSessionRequest model, string csrfToken, HttpContext httpContext)
            {
                _model = model;
                CsrfToken = csrfToken;
                HttpContext = httpContext;
            }

            public string AuthCode => _model.AuthCode;
            public string CodeVerifier => _model.CodeVerifier;
            public Uri RedirectUrl => new Uri(_model.RedirectUrl);
            public string CsrfToken { get; }
            public HttpContext HttpContext { get; }
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
                responseBody = userSession.Accept(new UserSessionResponseVisitor<PostUserSessionResponse>(Settings, responseBody));

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

        private sealed class UserSessionResponseVisitor<TUserSessionResponse> : IUserSessionVisitor<TUserSessionResponse>
            where TUserSessionResponse : UserSessionResponse
        {
            private readonly ConfigurationSettings _settings;
            private readonly TUserSessionResponse _userSessionResponse;

            public UserSessionResponseVisitor(
                ConfigurationSettings settings,
                TUserSessionResponse userSessionResponse)
            {
                _settings = settings;
                _userSessionResponse = userSessionResponse;
            }

            public TUserSessionResponse Visit(P5UserSession userSession)
            {
                SetCommonProperties(userSession);
                _userSessionResponse.Name = $"{userSession.CitizenIdUserSession.GivenName} {userSession.CitizenIdUserSession.FamilyName}";
                _userSessionResponse.Im1MessagingEnabled = false;
                return _userSessionResponse;
            }

            public TUserSessionResponse Visit(P9UserSession userSession)
            {
                SetCommonProperties(userSession);
                _userSessionResponse.Name = userSession.GpUserSession.Name;
                _userSessionResponse.NhsNumber = userSession.GpUserSession.NhsNumber;
                _userSessionResponse.Im1MessagingEnabled = userSession.GpUserSession.Im1MessagingEnabled;
                return _userSessionResponse;
            }

            private void SetCommonProperties(P5UserSession userSession)
            {
                _userSessionResponse.SessionTimeout = (int)TimeSpan.FromMinutes(_settings.DefaultSessionExpiryMinutes).TotalSeconds;
                _userSessionResponse.OdsCode = userSession.OdsCode;
                _userSessionResponse.Token = userSession.CsrfToken;
                _userSessionResponse.DateOfBirth = userSession.CitizenIdUserSession.DateOfBirth;
                _userSessionResponse.AccessToken = userSession.CitizenIdUserSession.AccessToken;
                _userSessionResponse.ProofLevel = userSession.CitizenIdUserSession.ProofLevel;
            }
        }
    }
}