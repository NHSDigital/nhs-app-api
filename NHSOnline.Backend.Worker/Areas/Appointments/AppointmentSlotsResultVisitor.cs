using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.Worker.Router.Appointments;

namespace NHSOnline.Backend.Worker.Areas.Appointments
{
    internal class AppointmentSlotsResultVisitor: IAppointmentSlotsResultVisitor<IActionResult>
    {
        public IActionResult Visit(AppointmentSlotsResult.SuccessfullyRetrieved result)
        {
            return new OkObjectResult(result.Response);
        }

        public IActionResult Visit(AppointmentSlotsResult.BadRequest result)
        {
            return new BadRequestResult();
        }
        
        public IActionResult Visit(AppointmentSlotsResult.SupplierSystemUnavailable result)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }
        
        public IActionResult Visit(AppointmentSlotsResult.InternalServerError result)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}
