using System;
using Microsoft.AspNetCore.Mvc.Filters;

namespace NHSOnline.Backend.Auditing
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