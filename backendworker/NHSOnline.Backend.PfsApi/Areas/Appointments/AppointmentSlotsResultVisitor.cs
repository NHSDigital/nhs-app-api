using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.GpSystems.Appointments;

namespace NHSOnline.Backend.PfsApi.Areas.Appointments
{
    public class AppointmentSlotsResultVisitor : IAppointmentSlotsResultVisitor<IActionResult>
    {
        public IActionResult Visit(AppointmentSlotsResult.Success result)
        {
            return new OkObjectResult(result.Response);
        }

        public IActionResult Visit(AppointmentSlotsResult.BadGateway result)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }

        public IActionResult Visit(AppointmentSlotsResult.InternalServerError result)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }

        public IActionResult Visit(AppointmentSlotsResult.Forbidden result)
        {
            return new StatusCodeResult(StatusCodes.Status403Forbidden);
        }
    }
}
