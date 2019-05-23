using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.GpSystems.Appointments;
using static NHSOnline.Backend.Support.Constants;

namespace NHSOnline.Backend.PfsApi.Areas.Appointments
{
    public class AppointmentBookResultVisitor : IAppointmentBookResultVisitor<IActionResult>
    {
        public IActionResult Visit(AppointmentBookResult.Success result)
        {
            return new CreatedResult(string.Empty, null);
        }

        public IActionResult Visit(AppointmentBookResult.Forbidden result)
        {
            return new StatusCodeResult(StatusCodes.Status403Forbidden);
        }

        public IActionResult Visit(AppointmentBookResult.SlotNotAvailable result)
        {
            return new StatusCodeResult(StatusCodes.Status409Conflict);
        }

        public IActionResult Visit(AppointmentBookResult.BadGateway result)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }

        public IActionResult Visit(AppointmentBookResult.BadRequest result)
        {
            return new BadRequestResult();
        }

        public IActionResult Visit(AppointmentBookResult.AppointmentLimitReached result)
        {
            return new StatusCodeResult(CustomHttpStatusCodes.Status460LimitReached);
        }

        public IActionResult Visit(AppointmentBookResult.InternalServerError result)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}
