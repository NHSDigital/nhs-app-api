using System;
using Microsoft.AspNetCore.Mvc.Filters;

namespace NHSOnline.Backend.Worker.Filters
{
    public class AccessControlAllowOriginFilter : Attribute, IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var headerValue = Environment.GetEnvironmentVariable("CORS_AUTHORITY");
            if (string.IsNullOrEmpty(headerValue))
            {
                return;
            }

            context.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", headerValue);
        }

        public void OnActionExecuted(ActionExecutedContext context) { }
    }
}
