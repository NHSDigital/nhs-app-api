using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.LoggerApi
{
    internal sealed class LoggerSessionLoggingScopeMiddleware
    {
        private readonly RequestDelegate _next;

        public LoggerSessionLoggingScopeMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(
            HttpContext context,
            ILogger<LoggerSessionLoggingScopeMiddleware> logger,
            LoggerSessionLoggerScope scope)
        {
            using (logger.BeginScope(scope))
            {
                await _next(context);
            }
        }
    }
}