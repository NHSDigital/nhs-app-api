using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NHSOnline.Backend.Worker.GpSystems.Appointments
{
    public class MyAppointmentsResultVisitor : IMyAppointmentsResultVisitor<IActionResult>
    {
        public IActionResult Visit(MyAppointmentsResult.SuccessfullyRetrieved result)
        {
            return new OkObjectResult(result.Response);
        }

        public IActionResult Visit(MyAppointmentsResult.BadRequest result)
        {
            return new BadRequestResult();
        }

        public IActionResult Visit(MyAppointmentsResult.SupplierSystemUnavailable result)
        {
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }

        public IActionResult Visit(MyAppointmentsResult.InternalServerError result)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}
