using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.PfsApi.Session
{
    internal sealed class SessionLoggingScopeMiddleware
    {
        private readonly RequestDelegate _next;

        public SessionLoggingScopeMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(
            HttpContext context,
            ILogger<SessionLoggingScopeMiddleware> logger,
            SessionLoggerScope scope)
        {
            using (logger.BeginScope(scope))
            {
                await _next(context);
            }
        }
    }
}