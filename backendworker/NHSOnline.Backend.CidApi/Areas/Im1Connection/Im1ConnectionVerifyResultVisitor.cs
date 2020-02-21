using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Im1Connection;
using PatientIm1ConnectionResponse = NHSOnline.Backend.CidApi.Areas.Im1Connection.Models.PatientIm1ConnectionResponse;

namespace NHSOnline.Backend.CidApi.Areas.Im1Connection
{
    internal class Im1ConnectionVerifyResultVisitor : IIm1ConnectionVerifyResultVisitor<IActionResult>
    {
        private HttpRequest Request { get; }
        public ILogger Logger { get; }

        public Im1ConnectionVerifyResultVisitor(HttpRequest request, ILogger logger)
        {
            Request = request;
            Logger = logger;
        }
        public IActionResult Visit(Im1ConnectionVerifyResult.Success result)
        {
            Logger.LogInformation("Im1 Connection verification resulted in a success");
            return new OkObjectResult(CreateResponse(result.Response));
        }

        public IActionResult Visit(Im1ConnectionVerifyResult.BadGateway result)
        {
            Logger.LogInformation("Im1 Connection verification resulted in a bad gateway");
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }
        
        public IActionResult Visit(Im1ConnectionVerifyResult.BadRequest result)
        {
            Logger.LogInformation("Im1 Connection verification resulted in a bad request");
            return new StatusCodeResult(StatusCodes.Status400BadRequest);
        }

        public IActionResult Visit(Im1ConnectionVerifyResult.Forbidden result)
        {
            Logger.LogInformation("Im1 Connection verification resulted in forbidden");
            return new StatusCodeResult(StatusCodes.Status403Forbidden);
        }

        public IActionResult Visit(Im1ConnectionVerifyResult.UnmappedErrorWithStatusCode result)
        {
            Logger.LogError("Im1 Connection verification resulted in an error which was unmappable");
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }
        
        public IActionResult Visit(Im1ConnectionVerifyResult.NotFound result)
        {
            Logger.LogError("Im1 Connection verification resulted in Not Found");
            return new StatusCodeResult(StatusCodes.Status404NotFound);
        }

        public IActionResult Visit(Im1ConnectionVerifyResult.ErrorCase result)
        {
            Logger.LogInformation("Im1 Connection verification resulted in an error which was mapped");
            var statusCodeResult = Im1ConnectionV1ErrorCodeMapper.Map(result.ErrorCode);
            return new StatusCodeResult(statusCodeResult);
        }

        public IActionResult Visit(Im1ConnectionVerifyResult.InternalServerError result)
        {
            Logger.LogInformation("Im1 Connection verification resulted in an internal server error");
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }

        private static PatientIm1ConnectionResponse CreateResponse(GpSystems.Im1Connection.Models.PatientIm1ConnectionResponse response)
            => new PatientIm1ConnectionResponse
            {
                ConnectionToken = response.ConnectionToken,
                NhsNumbers = response.NhsNumbers
            };
    }
}