using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.GpSystems.Appointments;
using static NHSOnline.Backend.Support.Constants;

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

        public IActionResult Visit(AppointmentBookResult.BadRequest badRequest)
        {
            return new BadRequestResult();
        }

        public IActionResult Visit(AppointmentBookResult.AppointmentLimitReached appointmentLimitReached)
        {
            return new StatusCodeResult(CustomHttpStatusCodes.Status460LimitReached);
        }
    }
}
