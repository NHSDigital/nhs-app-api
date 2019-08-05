using System;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Support.AspNet.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public sealed class HttpContextLogActionFilterAttribute : ActionFilterAttribute
    {
        private IDisposable _context;
        private readonly ILogger<HttpContextLogActionFilterAttribute> _logger;
        private readonly ILoggerFactory _loggerFactory;

        public HttpContextLogActionFilterAttribute(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<HttpContextLogActionFilterAttribute>();
        }
        
        public ILoggerFactory LoggerFactory => _loggerFactory;

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            _context = _logger.BeginScope(context.HttpContext);
        }

        public override void OnActionExecuted(ActionExecutedContext context) { }

        public override void OnResultExecuting(ResultExecutingContext context) { }

        public override void OnResultExecuted(ResultExecutedContext context)
        {
            _context?.Dispose();
        }
    }
}
