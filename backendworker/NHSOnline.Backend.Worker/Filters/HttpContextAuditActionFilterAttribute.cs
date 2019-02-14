using Microsoft.AspNetCore.Mvc.Filters;
using NHSOnline.Backend.Support.Auditing;
using System;

namespace NHSOnline.Backend.Worker.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public sealed class HttpContextAuditActionFilterAttribute : ActionFilterAttribute
    {
        private IDisposable _context;
        private readonly IAuditor _auditor;
        
        public HttpContextAuditActionFilterAttribute(IAuditor auditor)
        {
            _auditor = auditor;
        }
        
        public IAuditor Auditor => _auditor;

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            _context = _auditor.BeginScope(context.HttpContext);
        }

        public override void OnActionExecuted(ActionExecutedContext context) { }

        public override void OnResultExecuting(ResultExecutingContext context) { }

        public override void OnResultExecuted(ResultExecutedContext context)
        {
            _context?.Dispose();
        }
    }
}