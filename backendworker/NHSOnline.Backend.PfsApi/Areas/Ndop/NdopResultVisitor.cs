using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.PfsApi.Ndop;

namespace NHSOnline.Backend.PfsApi.Areas.Ndop
{
    internal class NdopResultVisitor : INdopResultVisitor<IActionResult>
    {
        public IActionResult Visit(GetNdopResult.Success result)
        {
            return new OkObjectResult(result);
        }

        public IActionResult Visit(GetNdopResult.InternalServerError result)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}