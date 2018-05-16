using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.Appointments.Models;
using NHSOnline.Backend.Worker.Filters;
using NHSOnline.Backend.Worker.Router;

namespace NHSOnline.Backend.Worker.Areas.Appointments
{
    [Route("patient/appointments")]
    public class AppointmentsController : Controller
    {
        private readonly ISystemProviderFactory _systemProviderFactory;

        public AppointmentsController(
            ILoggerFactory loggerFactory,
            ISystemProviderFactory systemProviderFactory
            )
        {
            _systemProviderFactory = systemProviderFactory;
        }

        [HttpPost, TimeoutExceptionFilter]
        public async Task<IActionResult> Post([FromBody]AppointmentBookRequest model)
        {
            var userSession = HttpContext.GetUserSession();

            var appointmentsService = _systemProviderFactory
                .CreateSystemProvider(userSession.Supplier)
                .GetAppointmentsService();

            var bookResult = await appointmentsService.Book(userSession, model);

            return bookResult.Accept(new AppointmentBookResultVisitor());
        }
    }
}