using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.Worker.Router.Prescriptions;

namespace NHSOnline.Backend.Worker.Areas.Prescriptions
{
    internal class PrescriptionResultVisitor : IPrescriptionResultVisitor<IActionResult>
    {
        public IActionResult Visit(PrescriptionResult.SuccessfullGet result)
        {
            return new OkObjectResult(result);
        }
        
        public IActionResult Visit(PrescriptionResult.SuccessfullPost result)
        {
            return new CreatedResult(string.Empty, null);
        }

        public IActionResult Visit(PrescriptionResult.SupplierSystemUnavailable result)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }
        
        public IActionResult Visit(PrescriptionResult.InsufficientPermissions result)
        {
            return new StatusCodeResult(StatusCodes.Status403Forbidden);
        }
        
        public IActionResult Visit(PrescriptionResult.BadRequest result)
        {
            return new StatusCodeResult(StatusCodes.Status400BadRequest);
        }
        
        public IActionResult Visit(PrescriptionResult.UnexpectedError result)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
        
        public IActionResult Visit(PrescriptionResult.CannotReorderPrescription result)
        {
            return new StatusCodeResult(StatusCodes.Status409Conflict);
        }
    }
}