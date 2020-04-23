using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.GpSystems.LinkedAccounts;
using NHSOnline.Backend.GpSystems.LinkedAccounts.Models;

namespace NHSOnline.Backend.PfsApi.Areas.LinkedAccounts
{
    public class LinkedAccountsResultVisitor : ILinkedAccountsResultVisitor<Task<IActionResult>>
    {
        public async Task<IActionResult> Visit(LinkedAccountsResult.Success result)
        {
            var response = new GetLinkedAccountsResponse
            {
                LinkedAccounts = result.ValidAccounts,
            };

            return await Task.FromResult(new OkObjectResult(response));
        }
    }
}
