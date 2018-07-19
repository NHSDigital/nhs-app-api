using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace NHSOnline.Backend.Worker.Filters
{
    public sealed class TimeoutExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            if (context.Exception is OperationCanceledException)
            {
                context.Result = new StatusCodeResult(StatusCodes.Status504GatewayTimeout);
            }
        }
    }
}
