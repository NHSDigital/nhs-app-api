using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Im1Connection;
using NHSOnline.Backend.GpSystems.Im1Connection.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.CidApi.Areas.Im1Connection
{
    internal class Im1ConnectionV2VerifyResultVisitor : IIm1ConnectionVerifyResultVisitor<IActionResult>
    {
        private HttpRequest Request { get; }
        private IIm1ConnectionErrorCodes ErrorCodes { get; }
        private Supplier GpSystem { get; }
        private ILogger Logger { get; }

        public Im1ConnectionV2VerifyResultVisitor(HttpRequest request,
            IIm1ConnectionErrorCodes errorCodes,
            Supplier gpSystem,
            ILogger logger)
        {
            Request = request;
            ErrorCodes = errorCodes;
            GpSystem = gpSystem;
            Logger = logger;
        }

        public IActionResult Visit(Im1ConnectionVerifyResult.Success result)
        {
            Logger.LogInformation("Im1 Connection verification resulted in a success");
            return new OkObjectResult(CreateV2Response(result.Response));
        }

        public IActionResult Visit(Im1ConnectionVerifyResult.BadGateway result)
        {
            Logger.LogInformation("Im1 Connection verification resulted in a bad gateway error");
            return new ObjectResult(UnknownError())
            {
                StatusCode = StatusCodes.Status502BadGateway
            };
        }

        public IActionResult Visit(Im1ConnectionVerifyResult.BadRequest result)
        {
            Logger.LogInformation("Im1 Connection verification resulted in a bad gateway error");
            return new ObjectResult(UnknownError())
            {
                StatusCode = StatusCodes.Status400BadRequest
            };
        }

        public IActionResult Visit(Im1ConnectionVerifyResult.UnmappedErrorWithStatusCode result)
        {
            Logger.LogError("Im1 Connection verification resulted in an error which was unmappable");
            return new ObjectResult(UnknownError())
            {
                StatusCode = StatusCodes.Status502BadGateway
            };
        }

        public IActionResult Visit(Im1ConnectionVerifyResult.InternalServerError result)
        {
            Logger.LogError("Im1 Connection verification resulted in an internal server error");
            return new ObjectResult(UnknownError())
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }

        public IActionResult Visit(Im1ConnectionVerifyResult.Forbidden result)
        {
            Logger.LogError("Im1 Connection verification resulted in a forbidden error");
            return new ObjectResult(UnknownError())
            {
                StatusCode = StatusCodes.Status403Forbidden
            };
        }

        public IActionResult Visit(Im1ConnectionVerifyResult.NotFound result)
        {
            Logger.LogError("Im1 Connection verification resulted in a not found error");
            return new ObjectResult(UnknownError())
            {
                StatusCode = StatusCodes.Status404NotFound
            };
        }

        public IActionResult Visit(Im1ConnectionVerifyResult.ErrorCase result)
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

        private Im1ErrorResponse UnknownError()
        {
            return new Im1ErrorResponse
            {
                ErrorCode = (int) Im1ConnectionErrorCodes.ExternalCode.UnknownError,
                ErrorMessage = "Unknown Error",
                GpSystem = GpSystem.ToString()
            };
        }

        private static PatientIm1ConnectionResponse CreateV2Response(PatientIm1ConnectionResponse response)
            => new PatientIm1ConnectionResponse
            {
                ConnectionToken = response.ConnectionToken,
                NhsNumbers = response.NhsNumbers,
                OdsCode = response.OdsCode
            };
    }
}