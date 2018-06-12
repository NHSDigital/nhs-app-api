using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.Worker.GpSystems.Appointments;

namespace NHSOnline.Backend.Worker.Areas.Appointments
{
    public class AppointmentsResultVisitor : IAppointmentsResultVisitor<IActionResult>
    {
        public IActionResult Visit(AppointmentsResult.SuccessfullyRetrieved result)
        {
            return new OkObjectResult(result.Response);
        }

        public IActionResult Visit(AppointmentsResult.BadRequest result)
        {
            return new BadRequestResult();
        }

        public IActionResult Visit(AppointmentsResult.SupplierSystemUnavailable result)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }

        public IActionResult Visit(AppointmentsResult.InternalServerError result)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}
