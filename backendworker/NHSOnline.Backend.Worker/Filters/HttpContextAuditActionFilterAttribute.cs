using Microsoft.AspNetCore.Mvc.Filters;
using NHSOnline.Backend.Worker.Support.Auditing;
using System;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Worker.Filters
{
    public class HttpContextAuditActionFilterAttribute : Attribute, IActionFilter, IResultFilter
    {
        private IDisposable _context;
        private readonly IAuditor _auditor;

        public HttpContextAuditActionFilterAttribute(IAuditor auditor)
        {
            _auditor = auditor;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            _context = _auditor.BeginScope(context.HttpContext);
        }

        public void OnActionExecuted(ActionExecutedContext context) { }

        public void OnResultExecuting(ResultExecutingContext context) { }

        public void OnResultExecuted(ResultExecutedContext context)
        {
            _context?.Dispose();
        }
    }
}