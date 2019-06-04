using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.GpSystems.Im1Connection;

namespace NHSOnline.Backend.CidApi.Areas.Im1Connection
{
    internal class Im1ConnectionV2RegisterResultVisitor : IIm1ConnectionRegisterResultVisitor<IActionResult>
    {

        private HttpRequest Request { get; }
        private IIm1ConnectionErrorCodes ErrorCodes { get; }

        public Im1ConnectionV2RegisterResultVisitor(HttpRequest request, IIm1ConnectionErrorCodes errorCodes)
        {
            Request = request;
            ErrorCodes = errorCodes;
        }

        public IActionResult Visit(Im1ConnectionRegisterResult.Success result)
        {
            return new CreatedResult(Request.GetDisplayUrl(), result.Response);
        }
        
        public IActionResult Visit(Im1ConnectionRegisterResult.BadRequest result)
        {
            var response = ErrorCodes.GetErrorResponse(result.ErrorCode);

            return new ObjectResult(response)
            {
                StatusCode = StatusCodes.Status400BadRequest
            };
        }

        public IActionResult Visit(Im1ConnectionRegisterResult.NotFound result)
        {
            var response = ErrorCodes.GetErrorResponse(result.ErrorCode);

            return new ObjectResult(response)
            {
                StatusCode = StatusCodes.Status404NotFound
            };
        }

        public IActionResult Visit(Im1ConnectionRegisterResult.Conflict result)
        {
            var response = ErrorCodes.GetErrorResponse(result.ErrorCode);

            return new ObjectResult(response)
            {
                StatusCode = StatusCodes.Status409Conflict
            };
        }

        public IActionResult Visit(Im1ConnectionRegisterResult.BadGateway result)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }
        
        public IActionResult Visit(Im1ConnectionRegisterResult.UnknownError result)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }

        public IActionResult Visit(Im1ConnectionRegisterResult.ErrorCase result)
        {
            var response = ErrorCodes.GetErrorResponse(result.ErrorCode);
            var statusCodeResult = Im1ConnectionV2ErrorCodeMapper.Map(result.ErrorCode);

            return new ObjectResult(response)
            {
                StatusCode = statusCodeResult
            };
        }
    }
}