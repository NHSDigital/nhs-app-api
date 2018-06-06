using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.Appointments.Models;
using NHSOnline.Backend.Worker.Filters;
using NHSOnline.Backend.Worker.GpSystems;

namespace NHSOnline.Backend.Worker.Areas.Appointments
{
    [Route("patient/appointments")]
    public class AppointmentsController : Controller
    {
        private readonly IGpSystemFactory _gpSystemFactory;

        public AppointmentsController(
            ILoggerFactory loggerFactory,
            IGpSystemFactory gpSystemFactory
            )
        {
            _gpSystemFactory = gpSystemFactory;
        }

        [HttpPost, TimeoutExceptionFilter]
        public async Task<IActionResult> Post([FromBody]AppointmentBookRequest model)
        {
            var userSession = HttpContext.GetUserSession();

            var appointmentsService = _gpSystemFactory
                .CreateGpSystem(userSession.Supplier)
                .GetAppointmentsService();

            var bookResult = await appointmentsService.Book(userSession, model);

            return bookResult.Accept(new AppointmentBookResultVisitor());
        }
    }
}