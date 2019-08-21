using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Support.AspNet.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public sealed class TimeoutExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public ILogger<TimeoutExceptionFilterAttribute> Logger { get; }

        public IErrorReferenceGenerator ErrorReferenceGenerator { get; }

        public TimeoutExceptionFilterAttribute(ILogger<TimeoutExceptionFilterAttribute> logger,
            IErrorReferenceGenerator errorReferenceGenerator)
        {
            Logger = logger;
            ErrorReferenceGenerator = errorReferenceGenerator;
        }

        public override void OnException(ExceptionContext context)
        {
            if (context.Exception is TimeoutException)
            {
                var sourceApi = SourceApi.None;

                if (context.Exception is NhsTimeoutException nhsTimeoutException)
                {
                    sourceApi = nhsTimeoutException.SourceApi;
                }

                var serviceDeskReference = ErrorReferenceGenerator.GenerateAndLogErrorReference(ErrorCategory.Timeout,
                    StatusCodes.Status504GatewayTimeout, sourceApi);

                Logger.LogError($"Operation timed out - exception: {context.Exception}");

                context.Result = new ObjectResult(new PfsErrorResponse
                {
                    ServiceDeskReference = serviceDeskReference
                })
                {
                    StatusCode = StatusCodes.Status504GatewayTimeout
                };
            }
        }
    }
}