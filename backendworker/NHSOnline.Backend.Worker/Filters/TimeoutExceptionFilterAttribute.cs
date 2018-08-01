using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace NHSOnline.Backend.Worker.Filters
{
    public sealed class TimeoutExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            if (typeof(OperationCanceledException).IsAssignableFrom(context.Exception.GetType()))
            {
                context.Result = new StatusCodeResult(StatusCodes.Status504GatewayTimeout);
            }
        }
    }
}
