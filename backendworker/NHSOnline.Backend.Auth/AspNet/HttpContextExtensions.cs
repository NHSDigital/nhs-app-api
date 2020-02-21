using NHSOnline.Backend.Auth.CitizenId.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Auth.AspNet
{
    public static class HttpContextExtensions
    {
        public static AccessToken GetAccessToken(this HttpContext httpContext, ILogger logger)
            => AccessToken.Parse(logger, httpContext);
    }
}