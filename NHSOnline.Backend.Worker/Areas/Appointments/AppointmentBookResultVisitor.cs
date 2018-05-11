using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.Worker.Router.Appointments;

namespace NHSOnline.Backend.Worker.Areas.Appointments
{
    public class AppointmentBookResultVisitor : IAppointmentBookResultVisitor<IActionResult>
    {
        public IActionResult Visit(AppointmentBookResult.SuccessfullyBooked successfullyBooked)
        {
            return new CreatedResult(string.Empty, null);
        }

        public IActionResult Visit(AppointmentBookResult.InsufficientPermissions insufficientPermissions)
        {
            return new StatusCodeResult(StatusCodes.Status403Forbidden);
        }

        public IActionResult Visit(AppointmentBookResult.SlotNotAvailable slotNotAvailable)
        {
            return new StatusCodeResult(StatusCodes.Status409Conflict);
        }

        public IActionResult Visit(AppointmentBookResult.SupplierSystemUnavailable supplierSystemUnavailable)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }
    }
}
