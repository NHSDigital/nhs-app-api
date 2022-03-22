using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.PfsApi.SecondaryCare;

namespace NHSOnline.Backend.PfsApi.Areas.SecondaryCare
{
    public class SecondaryCareSummaryResultVisitor : ISecondaryCareSummaryResultVisitor<IActionResult>
    {
        public IActionResult Visit(SecondaryCareSummaryResult.Success result)
        {
            return new OkObjectResult(result.Response);
        }

        public IActionResult Visit(SecondaryCareSummaryResult.BadGateway _)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }
    }
}