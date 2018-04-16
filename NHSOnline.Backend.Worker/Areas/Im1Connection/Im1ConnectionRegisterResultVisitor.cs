using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.Worker.Router.Im1Connection;

namespace NHSOnline.Backend.Worker.Areas.Im1Connection
{
    internal class Im1ConnectionRegisterResultVisitor : IIm1ConnectionRegisterResultVisitor<IActionResult>
    {
        private HttpRequest Request { get; }

        public Im1ConnectionRegisterResultVisitor(HttpRequest request)
        {
            Request = request;
        }

        public IActionResult Visit(Im1ConnectionRegisterResult.SuccessfullyRegistered result)
        {
            return new CreatedResult(Request.GetDisplayUrl(), result.Response);
        }

        public IActionResult Visit(Im1ConnectionRegisterResult.InsufficientPermissions result)
        {
            return new StatusCodeResult(StatusCodes.Status403Forbidden);
        }

        public IActionResult Visit(Im1ConnectionRegisterResult.NotFound result)
        {
            return new NotFoundResult();
        }

        public IActionResult Visit(Im1ConnectionRegisterResult.AccountAlreadyExists result)
        {
            return new StatusCodeResult(StatusCodes.Status409Conflict);
        }

        public IActionResult Visit(Im1ConnectionRegisterResult.SupplierSystemUnavailable result)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }
    }
}