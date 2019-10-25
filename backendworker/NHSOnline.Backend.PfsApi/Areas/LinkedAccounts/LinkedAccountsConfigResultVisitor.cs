using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.GpSystems.LinkedAccounts;
using NHSOnline.Backend.GpSystems.LinkedAccounts.Models;

namespace NHSOnline.Backend.PfsApi.Areas.LinkedAccounts
{
    internal class LinkedAccountsConfigResultVisitor : ILinkedAccountsConfigResultVisitor<IActionResult>
    {
        public IActionResult Visit(LinkedAccountsConfigResult.Success result)
        {
            return new OkObjectResult(result);
        }
        
    }
}