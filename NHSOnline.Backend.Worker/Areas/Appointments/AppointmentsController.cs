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
        private readonly IBridgeFactory _bridgeFactory;

        public AppointmentsController(
            ILoggerFactory loggerFactory,
            IBridgeFactory bridgeFactory
            )
        {
            _bridgeFactory = bridgeFactory;
        }

        [HttpPost, TimeoutExceptionFilter]
        public async Task<IActionResult> Post([FromBody]AppointmentBookRequest model)
        {
            var userSession = HttpContext.GetUserSession();

            var appointmentsService = _bridgeFactory
                .CreateBridge(userSession.Supplier)
                .GetAppointmentsService();

            var bookResult = await appointmentsService.Book(userSession, model);

            return bookResult.Accept(new AppointmentBookResultVisitor());
        }
    }
}