using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Support.Logging;
using NHSOnline.Backend.Worker.GpSystems.Appointments.Models;
using NHSOnline.Backend.Worker.GpSystems;
using NHSOnline.Backend.Worker.GpSystems.Appointments;
using NHSOnline.Backend.Worker.Support.Auditing;

namespace NHSOnline.Backend.Worker.Areas.Appointments
{
    [Route("patient/appointments"),PfsSecurityMode]
    public class AppointmentsController : Controller
    {
        private readonly ILogger<AppointmentsController> _logger;
        private readonly IGpSystemFactory _gpSystemFactory;
        private readonly IAuditor _auditor;
        private readonly ISessionCacheService _sessionCacheService;

        public AppointmentsController(
            ILogger<AppointmentsController> logger,
            IGpSystemFactory gpSystemFactory,
            IAuditor auditor,
            ISessionCacheService sessionCacheService
            )
        {
            _logger = logger;
            _gpSystemFactory = gpSystemFactory;
            _auditor = auditor;
            _sessionCacheService = sessionCacheService;
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] AppointmentCancelRequest model)
        {
            try
            {
                _logger.LogEnter();
                
                await _auditor.Audit(Constants.AuditingTitles.CancelAppointmentAuditTypeRequest, "Attempting to cancel appointment with id: {0}",
                    model.AppointmentId);

                var userSession = HttpContext.GetUserSession();

                var appointmentsService = GetAppointmentsService(userSession);

                var cancelResult = await appointmentsService.Cancel(userSession.GpUserSession, model);

                await cancelResult.Accept(new AppointmentCancelAuditingVisitor(_auditor, _logger, model.AppointmentId));
                return cancelResult.Accept(new AppointmentCancelResultVisitor());
            }
            finally
            {
                _logger.LogExit();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                _logger.LogEnter();
                
                await _auditor.Audit(Constants.AuditingTitles.ViewAppointmentAuditTypeRequest, "Attempting to view booked appointments");

                var userSession = HttpContext.GetUserSession();

                var appointmentsService = GetAppointmentsService(userSession);

                var result = await appointmentsService.GetAppointments(userSession.GpUserSession);

                await result.Accept(new AppointmentsAuditingVisitor(_auditor, _logger, userSession));

                return await result.Accept(new AppointmentsResultVisitor(_sessionCacheService, userSession));
            }
            finally
            {
                _logger.LogExit();
            }
        }
        

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]AppointmentBookRequest model)
        {
            try
            {
                _logger.LogEnter();
                
                await _auditor.Audit(Constants.AuditingTitles.BookAppointmentAuditTypeRequest,
                    "Attempting to book appointment with id: {0} and startTime: {1:O}", model.SlotId, model.StartTime);

                var userSession = HttpContext.GetUserSession();

                var appointmentsService = GetAppointmentsService(userSession);

                var bookResult = await appointmentsService.Book(userSession.GpUserSession, model);

                await bookResult.Accept(new AppointmentBookAuditingVisitor(_auditor, _logger, model.SlotId, model.StartTime));
                return bookResult.Accept(new AppointmentBookResultVisitor());
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private IAppointmentsService GetAppointmentsService(UserSession userSession)
        {
            _logger.LogDebug($"Fetch Appointments Service for GP System: '{userSession.GpUserSession.Supplier}'.");

            return _gpSystemFactory
                .CreateGpSystem(userSession.GpUserSession.Supplier)
                .GetAppointmentsService();
        }
    }
}
