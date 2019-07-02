using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.GpSystems.Im1Connection;
using PatientIm1ConnectionResponse = NHSOnline.Backend.CidApi.Areas.Im1Connection.Models.PatientIm1ConnectionResponse;

namespace NHSOnline.Backend.CidApi.Areas.Im1Connection
{
    internal class Im1ConnectionRegisterResultVisitor : IIm1ConnectionRegisterResultVisitor<IActionResult>
    {
        private HttpRequest Request { get; }

        public Im1ConnectionRegisterResultVisitor(HttpRequest request)
        {
            Request = request;
        }

        public IActionResult Visit(Im1ConnectionRegisterResult.Success result)
        {
            
            return new CreatedResult(Request.GetDisplayUrl(), CreateResponse(result.Response));
        }
        
        public IActionResult Visit(Im1ConnectionRegisterResult.BadRequest result)
        {
            return new StatusCodeResult(StatusCodes.Status400BadRequest);
        }

        public IActionResult Visit(Im1ConnectionRegisterResult.NotFound result)
        {
            return new NotFoundResult();
        }

        public IActionResult Visit(Im1ConnectionRegisterResult.BadGateway result)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }

        public IActionResult Visit(Im1ConnectionRegisterResult.Conflict result)
        {
            return new StatusCodeResult(StatusCodes.Status409Conflict);
        }

        public IActionResult Visit(Im1ConnectionRegisterResult.UnknownError result)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }
        
        public IActionResult Visit(Im1ConnectionRegisterResult.ErrorCase result)
        {
            var statusCodeResult = Im1ConnectionV1ErrorCodeMapper.Map(result.ErrorCode);
            return new StatusCodeResult(statusCodeResult);
        }

        private static PatientIm1ConnectionResponse CreateResponse(GpSystems.Im1Connection.Models.PatientIm1ConnectionResponse response)
            => new PatientIm1ConnectionResponse
            {
                ConnectionToken = response.ConnectionToken,
                NhsNumbers = response.NhsNumbers
            };
    }
}