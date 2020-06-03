using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auth.CitizenId.Models;

namespace NHSOnline.Backend.Auth.AspNet
{
    public class AccessTokenProvider : IAccessTokenProvider
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly ILogger<AccessTokenProvider> _logger;

        public AccessTokenProvider(IHttpContextAccessor httpContext, ILogger<AccessTokenProvider> logger)
        {
            _httpContext = httpContext ?? throw new ArgumentNullException(nameof(httpContext), "HttpContextAccessor is not set");
            _logger = logger;
        }

        public AccessToken AccessToken => GetAccessToken();

        private AccessToken GetAccessToken()
        {
            return _httpContext.HttpContext.GetAccessToken(_logger);
        }
    }
}