using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems;
using static NHSOnline.Backend.Support.Constants;

namespace NHSOnline.Backend.PfsApi.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public sealed class InvalidPatientIdExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public ILogger<InvalidPatientIdExceptionFilterAttribute> Logger { get; }

        public InvalidPatientIdExceptionFilterAttribute(
            ILogger<InvalidPatientIdExceptionFilterAttribute> logger)
        {
            Logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            if (context.ExceptionHandled)
            {
                return;
            }

            if (context.Exception is InvalidPatientIdException)
            {
                Logger.LogWarning($"{ nameof(InvalidPatientIdException) } was caught - {context.Exception.Message} - returning { nameof(CustomHttpStatusCodes.Status467InvalidPatientId) }");
                Logger.LogDebug($"{ context.Exception }");
                context.Result = new StatusCodeResult(CustomHttpStatusCodes.Status467InvalidPatientId);
                context.ExceptionHandled = true;
            }
        }
    }
}
