using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Support.Logging;
using NHSOnline.Backend.Worker.Areas.Appointments.Models;
using NHSOnline.Backend.Worker.Filters;
using NHSOnline.Backend.Worker.GpSystems;
using NHSOnline.Backend.Worker.GpSystems.Appointments;
using NHSOnline.Backend.Worker.Support.Auditing;

namespace NHSOnline.Backend.Worker.Areas.Appointments
{
    [Route("patient/appointments")]
    public class AppointmentsController : Controller
    {
        private readonly ILogger<AppointmentsController> _logger;
        private readonly IGpSystemFactory _gpSystemFactory;
        private readonly IAuditor _auditor;

        public AppointmentsController(
            ILogger<AppointmentsController> logger,
            IGpSystemFactory gpSystemFactory,
            IAuditor auditor
            )
        {
            _logger = logger;
            _gpSystemFactory = gpSystemFactory;
            _auditor = auditor;
        }

        [HttpDelete, TimeoutExceptionFilter]
        public async Task<IActionResult> Delete([FromBody] AppointmentCancelRequest model)
        {
            try
            {
                _logger.LogEnter(nameof(Delete));
                
                _auditor.Audit(Constants.AuditingTitles.CancelAppointmentAuditTypeRequest, "Attempting to cancel appointment with id: {0}",
                    model.AppointmentId);

                var userSession = HttpContext.GetUserSession();

                var appointmentsService = GetAppointmentsService(userSession);

                var cancelResult = await appointmentsService.Cancel(userSession, model);

                cancelResult.Accept(new AppointmentCancelAuditingVisitor(_auditor, model.AppointmentId));
                return cancelResult.Accept(new AppointmentCancelResultVisitor());
            }
            finally
            {
                _logger.LogExit(nameof(Delete));
            }
        }

        [HttpGet, TimeoutExceptionFilter]
        public async Task<IActionResult> Get(
            [FromQuery] bool includePastAppointments,
            [FromQuery] DateTimeOffset? pastAppointmentsFromDate = null)
        {
            try
            {
                _logger.LogEnter(nameof(Get));
                
                _auditor.Audit(Constants.AuditingTitles.ViewAppointmentAuditTypeRequest, "Attempting to view booked appointments");

                var userSession = HttpContext.GetUserSession();

                var appointmentsService = GetAppointmentsService(userSession);

                var result =
                    await appointmentsService.GetAppointments(userSession, includePastAppointments,
                        pastAppointmentsFromDate);

                result.Accept(new AppointmentsAuditingVisitor(_auditor));

                return result.Accept(new AppointmentsResultVisitor());
            }
            finally
            {
                _logger.LogExit(nameof(Get));
            }
        }
        

        [HttpPost, TimeoutExceptionFilter]
        public async Task<IActionResult> Post([FromBody]AppointmentBookRequest model)
        {
            try
            {
                _logger.LogEnter(nameof(Post));
                
                _auditor.Audit(Constants.AuditingTitles.BookAppointmentAuditTypeRequest,
                    "Attempting to book appointment with id: {0} and startTimeDate: {1:O}", model.SlotId, model.StartTime);

                var userSession = HttpContext.GetUserSession();

                var appointmentsService = GetAppointmentsService(userSession);

                var bookResult = await appointmentsService.Book(userSession, model);

                bookResult.Accept(new AppointmentBookAuditingVisitor(_auditor, model.SlotId, model.StartTime));
                return bookResult.Accept(new AppointmentBookResultVisitor());
            }
            finally
            {
                _logger.LogExit(nameof(Post));
            }
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
