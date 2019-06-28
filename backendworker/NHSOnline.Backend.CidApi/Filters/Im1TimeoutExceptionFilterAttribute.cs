using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.ApiSupport.Filters;
using NHSOnline.Backend.GpSystems.Im1Connection;
using NHSOnline.Backend.GpSystems.Im1Connection.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.CidApi.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public sealed class Im1TimeoutExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private const Im1ConnectionErrorCodes.ExternalCode UnknownError = Im1ConnectionErrorCodes.ExternalCode.UnknownError;
        public ILogger<TimeoutExceptionFilterAttribute> Logger { get; }

        public Im1TimeoutExceptionFilterAttribute(ILogger<TimeoutExceptionFilterAttribute> logger)
        {
            Logger = logger;
        }
        
        public override void OnException(ExceptionContext context)
        {
            if (typeof(TimeoutException).IsAssignableFrom(context.Exception.GetType()))
            {
                Logger.LogError($"Operation timed out - exception: {context.Exception}");
                context.Result = new ObjectResult(
                    new Im1ErrorResponse
                    {
                        ErrorCode = (int)UnknownError,
                        ErrorMessage = EnumHelper.GetDescriptionOrThrowException(UnknownError),
                        GpSystem = Supplier.Unknown.ToString()
                    })
                {
                    StatusCode = StatusCodes.Status504GatewayTimeout
                };
            }
        }
    }
}
