using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.GpSystems.Prescriptions;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.Areas.Prescriptions
{
    internal class GetPrescriptionsResultVisitor : IGetPrescriptionsResultVisitor<IActionResult>
    {
        public IActionResult Visit(GetPrescriptionsResult.Success result)
        {
            return new OkObjectResult(result.Response);
        }

        public IActionResult Visit(GetPrescriptionsResult.BadGateway result)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }
        
        public IActionResult Visit(GetPrescriptionsResult.Forbidden result)
        {
            return new StatusCodeResult(StatusCodes.Status403Forbidden);
        }
        
        public IActionResult Visit(GetPrescriptionsResult.BadRequest result)
        {
            return new StatusCodeResult(StatusCodes.Status400BadRequest);
        }
        
        public IActionResult Visit(GetPrescriptionsResult.InternalServerError result)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
        
        public IActionResult Visit(GetPrescriptionsResult.CannotReorderPrescription result)
        {
            return new StatusCodeResult(StatusCodes.Status409Conflict);
        }

        public IActionResult Visit(GetPrescriptionsResult.MedicationAlreadyOrderedWithinLast30Days result)
        {
            return new StatusCodeResult(Constants.CustomHttpStatusCodes.Status466MedicationAlreadyOrderedWithinLast30Days);
        }
    }
}