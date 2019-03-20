using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Http;

namespace NHSOnline.Backend.CidApi.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public sealed class UnauthorisedGpSystemHttpRequestExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly ILogger<UnauthorisedGpSystemHttpRequestExceptionFilterAttribute> _logger;

        public ILogger<UnauthorisedGpSystemHttpRequestExceptionFilterAttribute> Logger => _logger;
        
        public UnauthorisedGpSystemHttpRequestExceptionFilterAttribute(
            ILogger<UnauthorisedGpSystemHttpRequestExceptionFilterAttribute> logger)
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
