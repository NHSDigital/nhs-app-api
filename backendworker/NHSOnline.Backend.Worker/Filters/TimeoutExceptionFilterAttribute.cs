using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Worker.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public sealed class TimeoutExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly ILogger<TimeoutExceptionFilterAttribute> _logger;

        public ILogger<TimeoutExceptionFilterAttribute> Logger => _logger;

        public TimeoutExceptionFilterAttribute(ILogger<TimeoutExceptionFilterAttribute> logger)
        {
            this._logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            if (typeof(OperationCanceledException).IsAssignableFrom(context.Exception.GetType()))
            {
                _logger.LogError("Operation timed out, exception: {}", context.Exception);
                context.Result = new StatusCodeResult(StatusCodes.Status504GatewayTimeout);
            }
        }
    }
}
