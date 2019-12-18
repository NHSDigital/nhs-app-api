using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Support.AspNet.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public sealed class UnparsableExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public ILogger<UnparsableExceptionFilterAttribute> Logger { get; }

        public IErrorReferenceGenerator ErrorReferenceGenerator { get; }

        public UnparsableExceptionFilterAttribute(ILogger<UnparsableExceptionFilterAttribute> logger,
            IErrorReferenceGenerator errorReferenceGenerator)
        {
            Logger = logger;
            ErrorReferenceGenerator = errorReferenceGenerator;
        }

        public override void OnException(ExceptionContext context)
        {
            if (context.ExceptionHandled)
            {
                return;
            }

            if (context.Exception is NhsUnparsableException unparsableException)
            {
                var serviceDeskReference =
                    ErrorReferenceGenerator.GenerateAndLogErrorReference(new ErrorTypes.UnhandledError());

                var redactedExceptions = string.Join(", ",
                    unparsableException.ErrorMessages.Select(error => error.ToString()));
                Logger.LogError($"Response was unparsable: {redactedExceptions}");

                context.Result = new ObjectResult(new PfsErrorResponse
                {
                    ServiceDeskReference = serviceDeskReference
                })
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };

                context.ExceptionHandled = true;
            }
        }
    }
}