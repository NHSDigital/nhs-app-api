using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.Appointments.Models;
using NHSOnline.Backend.Worker.Filters;
using NHSOnline.Backend.Worker.GpSystems;
using NHSOnline.Backend.Worker.GpSystems.Appointments;

namespace NHSOnline.Backend.Worker.Areas.Appointments
{
    [Route("patient/appointments")]
    public class AppointmentsController : Controller
    {
        private readonly ILogger<AppointmentsController> _logger;
        private readonly IGpSystemFactory _gpSystemFactory;

        public AppointmentsController(
            ILogger<AppointmentsController> logger,
            IGpSystemFactory gpSystemFactory
            )
        {
            _logger = logger;
            _gpSystemFactory = gpSystemFactory;
        }

        [HttpDelete, TimeoutExceptionFilter]
        public async Task<IActionResult> Delete([FromBody] AppointmentCancelRequest model)
        {
            var userSession = HttpContext.GetUserSession();

            var appointmentsService = GetAppointmentsService(userSession);

            var cancelResult = await appointmentsService.Cancel(userSession, model);
            return cancelResult.Accept(new AppointmentCancelResultVisitor());
        }

        [HttpGet, TimeoutExceptionFilter]
        public async Task<IActionResult> Get(
            [FromQuery] bool includePastAppointments,
            [FromQuery] DateTimeOffset? pastAppointmentsFromDate = null)
        {
            var userSession = HttpContext.GetUserSession();

            var appointmentsService = GetAppointmentsService(userSession);

            var result =
                await appointmentsService.GetAppointments(userSession, includePastAppointments,
                    pastAppointmentsFromDate);

            return result.Accept(new AppointmentsResultVisitor());
        }
        

        [HttpPost, TimeoutExceptionFilter]
        public async Task<IActionResult> Post([FromBody]AppointmentBookRequest model)
        {
            var userSession = HttpContext.GetUserSession();

            var appointmentsService = GetAppointmentsService(userSession);

            var bookResult = await appointmentsService.Book(userSession, model);

            return bookResult.Accept(new AppointmentBookResultVisitor());
        }

        private IAppointmentsService GetAppointmentsService(UserSession userSession)
        {
            _logger.LogDebug($"Fetch Appointments Service for GP System: '{userSession.Supplier}'.");

            return _gpSystemFactory
                .CreateGpSystem(userSession.Supplier)
                .GetAppointmentsService();
        }
    }
}