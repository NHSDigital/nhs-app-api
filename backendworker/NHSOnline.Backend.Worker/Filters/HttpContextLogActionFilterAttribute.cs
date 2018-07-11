using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;

namespace NHSOnline.Backend.Worker.Filters
{
    public class HttpContextLogActionFilterAttribute : Attribute, IActionFilter, IResultFilter
    {
        private IDisposable _context;
        private readonly ILogger<HttpContextLogActionFilterAttribute> _logger;

        public HttpContextLogActionFilterAttribute(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<HttpContextLogActionFilterAttribute>();
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            _context = _logger.BeginScope(context.HttpContext);
        }

        public void OnActionExecuted(ActionExecutedContext context) { }

        public void OnResultExecuting(ResultExecutingContext context) { }

        public void OnResultExecuted(ResultExecutedContext context)
        {
            _context?.Dispose();
        }
    }
}
