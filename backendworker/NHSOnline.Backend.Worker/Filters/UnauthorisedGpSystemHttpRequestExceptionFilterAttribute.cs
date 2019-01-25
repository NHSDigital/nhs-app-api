using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Support.Http;

namespace NHSOnline.Backend.Worker.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public sealed class UnauthorisedGpSystemHttpRequestExceptionFilterAttributeAttribute : ExceptionFilterAttribute
    {
        private readonly ILogger<UnauthorisedGpSystemHttpRequestExceptionFilterAttributeAttribute> _logger;

        public ILogger<UnauthorisedGpSystemHttpRequestExceptionFilterAttributeAttribute> Logger => _logger;

        public UnauthorisedGpSystemHttpRequestExceptionFilterAttributeAttribute(ILogger<UnauthorisedGpSystemHttpRequestExceptionFilterAttributeAttribute> logger)
        {
            _logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            if (context.Exception is UnauthorisedGpSystemHttpRequestException)
            {
                _logger.LogWarning($"{ nameof(UnauthorisedGpSystemHttpRequestException) } was caught - returning { nameof(StatusCodes.Status401Unauthorized) }");
                _logger.LogDebug($"{ context.Exception }");
                context.Result = new StatusCodeResult(StatusCodes.Status401Unauthorized);
            }
        }
    }
}
