using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Im1Connection;
using CreateIm1ConnectionResponse = NHSOnline.Backend.CidApi.Areas.Im1Connection.Models.CreateIm1ConnectionResponse;

namespace NHSOnline.Backend.CidApi.Areas.Im1Connection
{
    internal class Im1ConnectionRegisterResultVisitor : IIm1ConnectionRegisterResultVisitor<IActionResult>
    {
        private HttpRequest Request { get; }
        public ILogger Logger { get; }

        public Im1ConnectionRegisterResultVisitor(HttpRequest request, ILogger logger)
        {
            Request = request;
            Logger = logger;
        }

        public IActionResult Visit(Im1ConnectionRegisterResult.Success result)
        {
            Logger.LogInformation("Im1 Connection Registration resulted in a success");
            return new CreatedResult(Request.GetDisplayUrl(), CreateResponse(result.Response));
        }

        public IActionResult Visit(Im1ConnectionRegisterResult.BadGateway result)
        {
            Logger.LogInformation("Im1 Connection Registration resulted in a bad gateway");
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }

        public IActionResult Visit(Im1ConnectionRegisterResult.UnmappedErrorWithStatusCode result)
        {
            Logger.LogError("Im1 Connection Registration resulted in an error which was unmappable");
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }

        public IActionResult Visit(Im1ConnectionRegisterResult.ErrorCase result)
        {
            Logger.LogInformation("Im1 Connection Registration resulted in an error which was mapped");
            var statusCodeResult = Im1ConnectionV1ErrorCodeMapper.Map(result.ErrorCode);
            return new StatusCodeResult(statusCodeResult);
        }

        private static CreateIm1ConnectionResponse CreateResponse(GpSystems.Im1Connection.Models.CreateIm1ConnectionResponse response)
            => new CreateIm1ConnectionResponse
            {
                ConnectionToken = response.ConnectionToken,
                NhsNumbers = response.NhsNumbers
            };
    }
}