using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.Worker.GpSystems.Linkage;

namespace NHSOnline.Backend.Worker.Areas.Linkage
{
    internal class GetLinkageResultVisitor : IGetLinkageResultVisitor<IActionResult>
    {
        public IActionResult Visit(GetLinkageResult.SuccessfullyRetrieved result)
        {
            return new OkObjectResult(result.Response);
        }

        public IActionResult Visit(GetLinkageResult.NhsNumberNotFound result)
        {
            return new StatusCodeResult(StatusCodes.Status404NotFound);
        }

        public IActionResult Visit(GetLinkageResult.SupplierSystemUnavailable result)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }
        
        public IActionResult Visit(GetLinkageResult.InternalServerError result)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
        
        public IActionResult Visit(GetLinkageResult.LinkageKeyRevoked result)
        {
            return new StatusCodeResult(StatusCodes.Status403Forbidden);
        }
    }
}