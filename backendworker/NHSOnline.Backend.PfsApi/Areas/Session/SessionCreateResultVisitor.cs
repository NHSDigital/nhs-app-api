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
using NHSOnline.Backend.Support.Settings;

namespace NHSOnline.Backend.PfsApi.Areas.Session
{
    internal class SessionCreateResultVisitor : ICreateSessionResultVisitor<Task<IActionResult>>
    {
        private readonly UserSessionService _userSessionService;
        private readonly ILogger<SessionCreateResultVisitor> _logger;
        private readonly ConfigurationSettings _settings;
        private readonly IMetricLogger _metricLogger;
        private readonly ISessionErrorResultBuilder _errorResultBuilder;

        public SessionCreateResultVisitor(
            UserSessionService userSessionService,
            ILogger<SessionCreateResultVisitor> logger,
            ConfigurationSettings settings,
            IMetricLogger metricLogger,
            ISessionErrorResultBuilder errorResultBuilder)
        {
            _userSessionService = userSessionService;
            _logger = logger;
            _settings = settings;
            _metricLogger = metricLogger;
            _errorResultBuilder = errorResultBuilder;
        }

        public async Task<IActionResult> Visit(CreateSessionResult.Success success, HttpContext httpContext)
        {
            var userSession = success.UserSession;
            var serviceJourneyRules = success.ServiceJourneyRules;

            await AppendCookieToResponse(userSession.Key, httpContext);

            _userSessionService.SetUserSession(userSession);
            _logger.LogInformation($"Created {userSession.GetType().Name}");

            var responseBody = new PostUserSessionResponse
            {
                ServiceJourneyRules = serviceJourneyRules
            };

            responseBody = userSession.Accept(new CreateResponseFromUserSessionVisitor<PostUserSessionResponse>(_settings, responseBody));

            var metricLoggingData = new LoginData(httpContext.TraceIdentifier);
            await _metricLogger.Login(metricLoggingData);

            return new CreatedResult(string.Empty, responseBody);
        }

        public Task<IActionResult> Visit(CreateSessionResult.ErrorResult errorResultResult)
        {
            return Task.FromResult(_errorResultBuilder.BuildResult(errorResultResult.ErrorTypes));
        }

        private async Task AppendCookieToResponse(string sessionId, HttpContext httpContext)
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
    }
}