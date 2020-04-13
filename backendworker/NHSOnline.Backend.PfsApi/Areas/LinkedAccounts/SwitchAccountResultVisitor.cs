using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.GpSystems.LinkedAccounts;

namespace NHSOnline.Backend.PfsApi.Areas.LinkedAccounts
{
    public class SwitchAccountResultVisitor : ISwitchAccountResultVisitor<Task<IActionResult>>
    {
        public async Task<IActionResult> Visit(SwitchAccountResult.Success result)
        {
            return await Task.FromResult(new OkResult());
        }

        public async Task<IActionResult> Visit(SwitchAccountResult.Failure result)
        {
            return await Task.FromResult(new NotFoundResult());
        }
    }
}
