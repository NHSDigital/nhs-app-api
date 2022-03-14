using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Metrics;
using NHSOnline.Backend.PfsApi.Areas.Session.Models;
using NHSOnline.Backend.PfsApi.Session;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Session;
using NHSOnline.Backend.Support.Settings;

namespace NHSOnline.Backend.PfsApi.Areas.Session
{
    internal class SessionResultVisitor : ISessionResultVisitor<Task<IActionResult>>
    {
        private readonly UserSessionService _userSessionService;
        private readonly ILogger<SessionResultVisitor> _logger;
        private readonly ConfigurationSettings _settings;
        private readonly IMetricLogger _metricLogger;
        private readonly ISessionErrorResultBuilder _errorResultBuilder;
        private readonly ISessionExpiryCookieCreator _sessionExpiryCookieCreator;

        public SessionResultVisitor(
            UserSessionService userSessionService,
            ILogger<SessionResultVisitor> logger,
            ConfigurationSettings settings,
            IMetricLogger metricLogger,
            ISessionErrorResultBuilder errorResultBuilder,
            ISessionExpiryCookieCreator sessionExpiryCookieCreator)
        {
            _userSessionService = userSessionService;
            _logger = logger;
            _settings = settings;
            _metricLogger = metricLogger;
            _errorResultBuilder = errorResultBuilder;
            _sessionExpiryCookieCreator = sessionExpiryCookieCreator;
        }

        public async Task<IActionResult> Visit(CreateSessionResult.Success success, HttpContext httpContext, string sessionCookieExpiryToken, string referrer)
        {
            var userSession = success.UserSession;
            var serviceJourneyRules = success.ServiceJourneyRules;

            await AppendCookieToResponse(userSession.Key, httpContext);
            _sessionExpiryCookieCreator.AppendSessionExpiryCookie(httpContext, sessionCookieExpiryToken);

            _userSessionService.SetUserSession(userSession);
            _logger.LogInformation($"Created {userSession.GetType().Name}");

            var responseBody = new PostUserSessionResponse
            {
                ServiceJourneyRules = serviceJourneyRules
            };

            responseBody = userSession.Accept(new CreateResponseFromUserSessionVisitor<PostUserSessionResponse>(_settings, responseBody));

            await LoginLogMetrics(httpContext, userSession, referrer);

            return new CreatedResult(string.Empty, responseBody);
        }

        public async Task<IActionResult> Visit(CreateSessionResult.Success success, HttpContext httpContext, string referrer)
        {
            var userSession = success.UserSession;

            _userSessionService.SetUserSession(userSession);

            var responseBody = new PostUserSessionResponse();

            responseBody = userSession.Accept(new CreateResponseFromUserSessionVisitor<PostUserSessionResponse>(_settings, responseBody));

            await GpSessionCreatedLogMetrics(httpContext, userSession, referrer);

            return new CreatedResult(string.Empty, responseBody);
        }

        public Task<IActionResult> Visit(CreateSessionResult.ErrorResult errorResultResult)
        {
            return Task.FromResult(_errorResultBuilder.BuildResult(errorResultResult.ErrorTypes));
        }

        public async Task<IActionResult> Visit(CreateSessionResult.GpSessionExists gpSessionExists)
        {
            var userSession = gpSessionExists.UserSession; ;

            var responseBody = new PostUserSessionResponse();

            responseBody = gpSessionExists.UserSession.Accept(new CreateResponseFromUserSessionVisitor<PostUserSessionResponse>(_settings, responseBody));

            _logger.LogInformation($"User already has user session {userSession.GetType().Name}");

            return await Task.FromResult(new OkObjectResult(responseBody));
        }

        private static async Task AppendCookieToResponse(string sessionId, HttpContext httpContext)
        {
            var claims = new List<Claim>
            {
                new Claim(Constants.ClaimTypes.SessionId, sessionId)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await httpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));
        }

        private static LoginData CreateLoginData(HttpContext httpContext, UserSession userSession, string referrer)
        {
            var userAgent = httpContext.Request.Headers[Constants.HttpHeaders.UserAgent];
            return new LoginData(httpContext.TraceIdentifier, userSession.Key, userAgent, referrer);
        }

        private async Task LoginLogMetrics(HttpContext httpContext, UserSession userSession, string referrer)
        {
            await _metricLogger.Login(CreateLoginData(httpContext, userSession,  referrer));
        }

        private async Task GpSessionCreatedLogMetrics(HttpContext httpContext, UserSession userSession, string referrer)
        {
            await _metricLogger.GpSessionCreated(CreateLoginData(httpContext, userSession,  referrer));
        }
    }
}
