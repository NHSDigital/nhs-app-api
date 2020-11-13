using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace NHSOnline.Backend.AspNet.Middleware
{
    internal class LogRequestHeaderMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LogRequestHeaderMiddleware> _logger;
        private readonly LogRequestHeaderOptions _options;
        private string[] _templateParts;

        public LogRequestHeaderMiddleware(RequestDelegate next,
            ILoggerFactory loggerFactory, IOptions<LogRequestHeaderOptions> options)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<LogRequestHeaderMiddleware>();

            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));

            PrepareTemplate(_options.HeaderName);

            if(_templateParts.Length > 2)
            {
                throw new ArgumentException("LogTemplate contains more than one instance of {value}");
            }
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (!string.IsNullOrEmpty(httpContext.Request.Headers[_options.HeaderName]))
            {
                var logEntry = CreateLogEntry(httpContext);
                using(_logger.BeginScope(logEntry))
                {
                    await _next.Invoke(httpContext);
                }
            }
            else
            {
                await _next.Invoke(httpContext);
            }
        }

        private string CreateLogEntry(HttpContext httpContext)
        {
            string headerValue = httpContext.Request.Headers[_options.HeaderName];

            return _templateParts.Length == 1 ? _templateParts[0]
                : $"{_templateParts[0]}{headerValue}{_templateParts[1]}";
        }

        private void PrepareTemplate(string name)
        {
            var templatePart = _options.LogTemplate
                .Replace(LogRequestHeaderOptions.NameText, name, StringComparison.Ordinal);

            _templateParts = templatePart.Split(LogRequestHeaderOptions.ValueText);
        }
    }
}