using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.GpSystems.Im1Connection;

namespace NHSOnline.Backend.CidApi.Areas.Im1Connection
{
    internal class Im1ConnectionVerifyResultVisitor : IIm1ConnectionVerifyResultVisitor<IActionResult>
    {
        public IActionResult Visit(Im1ConnectionVerifyResult.Success result)
        {
            return new OkObjectResult(result.Response);
        }

        public IActionResult Visit(Im1ConnectionVerifyResult.NotFound result)
        {
            return new NotFoundResult();
        }

        public IActionResult Visit(Im1ConnectionVerifyResult.BadGateway result)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }

        public IActionResult Visit(Im1ConnectionVerifyResult.InternalServerError result)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }

        public IActionResult Visit(Im1ConnectionVerifyResult.BadRequest result)
        {
            return new StatusCodeResult(StatusCodes.Status400BadRequest);
        }
    }
}