using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Im1Connection;
using NHSOnline.Backend.GpSystems.Im1Connection.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.CidApi.Areas.Im1Connection
{
    internal class Im1ConnectionV2RegisterResultVisitor : IIm1ConnectionRegisterResultVisitor<IActionResult>
    {
        private HttpRequest Request { get; }
        private IIm1ConnectionErrorCodes ErrorCodes { get; }
        private Supplier GpSystem { get; }
        private ILogger Logger { get; }

        public Im1ConnectionV2RegisterResultVisitor(HttpRequest request,
            IIm1ConnectionErrorCodes errorCodes,
            Supplier gpSystem,
            ILogger logger)
        {
            Request = request;
            ErrorCodes = errorCodes;
            GpSystem = gpSystem;
            Logger = logger;
        }

        public IActionResult Visit(Im1ConnectionRegisterResult.Success result)
        {
            Logger.LogInformation("Im1 Connection Registration resulted in a success");
            return new CreatedResult(Request.GetDisplayUrl(), result.Response);
        }

        public IActionResult Visit(Im1ConnectionRegisterResult.BadGateway result)
        {
            Logger.LogInformation("Im1 Connection Registration resulted in a bad gateway error");
            return new ObjectResult(UnknownError())
            {
                StatusCode = StatusCodes.Status502BadGateway
            };
        }

        public IActionResult Visit(Im1ConnectionRegisterResult.UnmappedErrorWithStatusCode result)
        {
            Logger.LogError("Im1 Connection Registration resulted in an error which was unmappable");
            return new ObjectResult(UnknownError())
            {
                StatusCode = StatusCodes.Status502BadGateway
            };
        }

        public IActionResult Visit(Im1ConnectionRegisterResult.ErrorCase result)
        {
            Logger.LogInformation("Im1 Connection Registration resulted in an error which was mapped");
            var response = ErrorCodes.GetAndLogErrorResponse(result.ErrorCode, GpSystem, Logger);
            var externalErrorCode = ErrorCodes.GetExternalCode(result.ErrorCode);
            var statusCodeResult = Im1ConnectionV2ErrorCodeToStatusCodeMapper.Map(externalErrorCode);
            return new ObjectResult(response)
            {
                StatusCode = statusCodeResult
            };
        }

        public Im1ErrorResponse UnknownError()
        {
            return new Im1ErrorResponse
            {
                ErrorCode = (int) Im1ConnectionErrorCodes.ExternalCode.UnknownError,
                ErrorMessage = "Unknown Error",
                GpSystem = GpSystem.ToString()
            };
        }
    }
}