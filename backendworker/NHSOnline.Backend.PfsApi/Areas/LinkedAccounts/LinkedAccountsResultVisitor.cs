using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.GpSystems.LinkedAccounts;

namespace NHSOnline.Backend.PfsApi.Areas.LinkedAccounts
{
    public class LinkedAccountsResultVisitor : ILinkedAccountsResultVisitor<Task<IActionResult>>
    {
        public async Task<IActionResult> Visit(LinkedAccountsResult.Success result)
        {
            return await Task.FromResult(new OkObjectResult(result.Response));
        }
    }
}
