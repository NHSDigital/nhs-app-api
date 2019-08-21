using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.GpSystems.Prescriptions;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.Areas.Prescriptions
{
    internal class OrderPrescriptionResultVisitor : IOrderPrescriptionResultVisitor<IActionResult>
    {
        public IActionResult Visit(OrderPrescriptionResult.Success result)
        {
            return new CreatedResult(string.Empty, null);
        }

        public IActionResult Visit(OrderPrescriptionResult.BadGateway result)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }
        
        public IActionResult Visit(OrderPrescriptionResult.Forbidden result)
        {
            return new StatusCodeResult(StatusCodes.Status403Forbidden);
        }
        
        public IActionResult Visit(OrderPrescriptionResult.BadRequest result)
        {
            return new StatusCodeResult(StatusCodes.Status400BadRequest);
        }
        
        public IActionResult Visit(OrderPrescriptionResult.InternalServerError result)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
        
        public IActionResult Visit(OrderPrescriptionResult.CannotReorderPrescription result)
        {
            return new StatusCodeResult(StatusCodes.Status409Conflict);
        }

        public IActionResult Visit(OrderPrescriptionResult.MedicationAlreadyOrderedWithinLast30Days result)
        {
            return new StatusCodeResult(Constants.CustomHttpStatusCodes.Status466MedicationAlreadyOrderedWithinLast30Days);
        }

        public IActionResult Visit(OrderPrescriptionResult.PartialSuccess result)
        {
            return new OkObjectResult(result.Response)
            {
                StatusCode = StatusCodes.Status202Accepted,
            };
        }
    }
}
