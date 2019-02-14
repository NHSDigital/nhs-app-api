using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.Worker.Areas.Appointments
{
    public class AppointmentCancelResultVisitor : IAppointmentCancelResultVisitor<IActionResult>
    {
        public IActionResult Visit(AppointmentCancelResult.SuccessfullyCancelled successfullyCancelled)
        {
            return new NoContentResult();
        }

        public IActionResult Visit(AppointmentCancelResult.BadRequest badRequest)
        {
            return new BadRequestResult();
        }

        public IActionResult Visit(AppointmentCancelResult.AppointmentNotCancellable appointmentNotCancellable)
        {
            return new StatusCodeResult(StatusCodes.Status409Conflict);
        }

        public IActionResult Visit(AppointmentCancelResult.TooLateToCancel tooLateToCancel)
        {
            return new StatusCodeResult(Constants.CustomHttpStatusCodes.Status461TooLate);
        }

        public IActionResult Visit(AppointmentCancelResult.InsufficientPermissions insufficientPermissions)
        {
            return new StatusCodeResult(StatusCodes.Status403Forbidden);
        }

        public IActionResult Visit(AppointmentCancelResult.SupplierSystemUnavailable supplierSystemUnavailable)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }
    }
}