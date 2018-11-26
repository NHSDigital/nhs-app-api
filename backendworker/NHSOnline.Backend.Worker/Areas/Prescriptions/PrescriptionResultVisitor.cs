using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.Worker.GpSystems.Prescriptions;

namespace NHSOnline.Backend.Worker.Areas.Prescriptions
{
    internal class PrescriptionResultVisitor : IPrescriptionResultVisitor<IActionResult>
    {
        public IActionResult Visit(PrescriptionResult.SuccessfulGet result)
        {
            return new OkObjectResult(result.Response);
        }
        
        public IActionResult Visit(PrescriptionResult.SuccessfulPost result)
        {
            return new CreatedResult(string.Empty, null);
        }

        public IActionResult Visit(PrescriptionResult.SupplierSystemUnavailable result)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }
        
        public IActionResult Visit(PrescriptionResult.SupplierNotEnabled result)
        {
            return new StatusCodeResult(StatusCodes.Status403Forbidden);
        }
        
        public IActionResult Visit(PrescriptionResult.BadRequest result)
        {
            return new StatusCodeResult(StatusCodes.Status400BadRequest);
        }
        
        public IActionResult Visit(PrescriptionResult.InternalServerError result)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
        
        public IActionResult Visit(PrescriptionResult.CannotReorderPrescription result)
        {
            return new StatusCodeResult(StatusCodes.Status409Conflict);
        }

        public IActionResult Visit(PrescriptionResult.MedicationAlreadyOrderedWithinLast30Days result)
        {
            return new StatusCodeResult(Constants.CustomHttpStatusCodes.Status466MedicationAlreadyOrderedWithinLast30Days);
        }
    }
}