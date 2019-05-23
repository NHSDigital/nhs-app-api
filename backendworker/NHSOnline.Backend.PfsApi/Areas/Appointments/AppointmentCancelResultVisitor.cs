using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.Areas.Appointments
{
    public class AppointmentCancelResultVisitor : IAppointmentCancelResultVisitor<IActionResult>
    {
        public IActionResult Visit(AppointmentCancelResult.Success result)
        {
            return new NoContentResult();
        }

        public IActionResult Visit(AppointmentCancelResult.BadRequest result)
        {
            return new BadRequestResult();
        }

        public IActionResult Visit(AppointmentCancelResult.AppointmentNotCancellable result)
        {
            return new StatusCodeResult(StatusCodes.Status409Conflict);
        }

        public IActionResult Visit(AppointmentCancelResult.TooLateToCancel result)
        {
            return new StatusCodeResult(Constants.CustomHttpStatusCodes.Status461TooLate);
        }

        public IActionResult Visit(AppointmentCancelResult.Forbidden result)
        {
            return new StatusCodeResult(StatusCodes.Status403Forbidden);
        }

        public IActionResult Visit(AppointmentCancelResult.BadGateway result)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }

        public IActionResult Visit(AppointmentCancelResult.InternalServerError result)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}
