using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Support.AspNet.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public sealed class UnhandledExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public ILogger<UnhandledExceptionFilterAttribute> Logger { get; }
        
        public IErrorReferenceGenerator ErrorReferenceGenerator { get; }

        public UnhandledExceptionFilterAttribute(
            
            ILogger<UnhandledExceptionFilterAttribute> logger,
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

            var serviceDeskReference =
                    ErrorReferenceGenerator.GenerateAndLogErrorReference(new ErrorTypes.UnhandledError());

            Logger.LogError(context.Exception, "Unhandled Exception");

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