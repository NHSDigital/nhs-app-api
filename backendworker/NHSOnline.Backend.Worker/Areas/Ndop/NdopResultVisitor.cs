using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.Worker.Ndop;

namespace NHSOnline.Backend.Worker.Areas.Ndop
{
    internal class NdopResultVisitor : INdopResultVisitor<IActionResult>
    {
        public IActionResult Visit(GetNdopResult.SuccessfullyRetrieved result)
        {
            return new OkObjectResult(result);
        }

        public IActionResult Visit(GetNdopResult.Unsuccessful result)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}