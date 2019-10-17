using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.GpSystems.LinkedAccounts;
using NHSOnline.Backend.GpSystems.LinkedAccounts.Models;

namespace NHSOnline.Backend.PfsApi.Areas.LinkedAccounts
{
    internal class PatientGuidResultVisitor : IPatientGuidResultVisitor<IActionResult>
    {
        public IActionResult Visit(GetPatientGuidResult.Success result)
        {
            return new OkObjectResult(result);
        }
        
    }
}