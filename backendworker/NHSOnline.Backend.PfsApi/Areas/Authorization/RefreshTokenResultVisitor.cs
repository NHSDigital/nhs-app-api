using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auth.CitizenId;
using NHSOnline.Backend.GpSystems.SessionManager;
using NHSOnline.Backend.PfsApi.Areas.Authorization.Models;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.Areas.Authorization
{
    public class RefreshTokenResultVisitor : IRefreshTokenResultVisitor<Task<IActionResult>>
    {
        private readonly ISessionCacheService _sessionCacheService;
        private readonly P5UserSession _userSession;
        private readonly ILogger _logger;

        public RefreshTokenResultVisitor(
            ISessionCacheService sessionCacheService,
            P5UserSession userSession,
            ILogger logger)
        {
            _sessionCacheService = sessionCacheService;
            _userSession = userSession;
            _logger = logger;
        }

        public async Task<IActionResult> Visit(RefreshAccessTokenResult.Success result)
        {
            _logger.LogInformation("Updating user's access token in session cache");
            _userSession.CitizenIdUserSession.AccessToken = result.AccessToken;
            await _sessionCacheService.UpdateUserSession(_userSession);

            return new OkObjectResult(new TokenRefreshResponse
            {
                Token = result.AccessToken
            });
        }

        public async Task<IActionResult> Visit(RefreshAccessTokenResult.BadGateway result)
        {
            return await Task.FromResult(new StatusCodeResult(StatusCodes.Status502BadGateway));
        }

        public async Task<IActionResult> Visit(RefreshAccessTokenResult.InternalServerError result)
        {
            return await Task.FromResult(new StatusCodeResult(StatusCodes.Status500InternalServerError));
        }
    }
}