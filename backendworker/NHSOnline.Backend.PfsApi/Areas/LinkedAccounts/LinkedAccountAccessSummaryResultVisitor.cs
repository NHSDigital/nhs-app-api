using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.GpSystems.LinkedAccounts;

namespace NHSOnline.Backend.PfsApi.Areas.LinkedAccounts
{
    public class LinkedAccountAccessSummaryResultVisitor : ILinkedAccountAccessSummaryResultVisitor<Task<IActionResult>>
    {
        public async Task<IActionResult> Visit(LinkedAccountAccessSummaryResult.Success result)
        {
            return await Task.FromResult(new OkObjectResult(result.Response));
        }

        public async Task<IActionResult> Visit(LinkedAccountAccessSummaryResult.NotFound result)
        {
            return await Task.FromResult(new NotFoundResult());
        }

        public async Task<IActionResult> Visit(LinkedAccountAccessSummaryResult.BadGateway result)
        {
            return await Task.FromResult(new StatusCodeResult(StatusCodes.Status502BadGateway));
        }
    }
}
