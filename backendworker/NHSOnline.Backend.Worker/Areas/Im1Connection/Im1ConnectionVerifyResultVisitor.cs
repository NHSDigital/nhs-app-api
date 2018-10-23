using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.Worker.GpSystems.Im1Connection;

namespace NHSOnline.Backend.Worker.Areas.Im1Connection
{
    internal class Im1ConnectionVerifyResultVisitor : IIm1ConnectionVerifyResultVisitor<IActionResult>
    {
        public IActionResult Visit(Im1ConnectionVerifyResult.SuccessfullyVerified result)
        {
            return new OkObjectResult(result.Response);
        }

        public IActionResult Visit(Im1ConnectionVerifyResult.InsufficientPermissions result)
        {
            return new StatusCodeResult(StatusCodes.Status403Forbidden);
        }

        public IActionResult Visit(Im1ConnectionVerifyResult.NotFound result)
        {
            return new NotFoundResult();
        }

        public IActionResult Visit(Im1ConnectionVerifyResult.SupplierSystemUnavailable result)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }

        public IActionResult Visit(Im1ConnectionVerifyResult.ErrorProcessingSecurityHeader errorProcessingSecurityHeader)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }

        public IActionResult Visit(Im1ConnectionVerifyResult.InvalidUserCredentials invalidUserCredentials)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }

        public IActionResult Visit(Im1ConnectionVerifyResult.InvalidRequest invalidRequest)
        {
            return new StatusCodeResult(StatusCodes.Status400BadRequest);
        }

        public IActionResult Visit(Im1ConnectionVerifyResult.UnknownError unknownError)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }
    }
}