using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.Worker.GpSystems.Linkage;

namespace NHSOnline.Backend.Worker.Areas.Linkage
{
    internal class CreateLinkageResultVisitor : ICreateLinkageResultVisitor<IActionResult>
    {
        public IActionResult Visit(CreateLinkageResult.SuccessfullyRetrieved result)
        {
            return new OkObjectResult(result.Response);
        }

        public IActionResult Visit(CreateLinkageResult.NhsNumberNotFound result)
        {
            return new StatusCodeResult(StatusCodes.Status404NotFound);
        }

        public IActionResult Visit(CreateLinkageResult.LinkageKeyAlreadyExists result)
        {
            return new StatusCodeResult(StatusCodes.Status409Conflict);
        }

        public IActionResult Visit(CreateLinkageResult.SupplierSystemUnavailable result)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }
        
        public IActionResult Visit(CreateLinkageResult.InternalServerError result)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}