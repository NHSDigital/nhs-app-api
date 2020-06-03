using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auth.CitizenId.Models;

namespace NHSOnline.Backend.Auth.AspNet
{
    public class AccessTokenProvider : IAccessTokenProvider
    {
        public AccessTokenProvider(IHttpContextAccessor httpContext, ILogger<AccessTokenProvider> logger)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext), "HttpContextAccessor is not set");
            }
            AccessToken = httpContext.HttpContext.GetAccessToken(logger);
        }

        public AccessToken AccessToken { get; }
    }
}