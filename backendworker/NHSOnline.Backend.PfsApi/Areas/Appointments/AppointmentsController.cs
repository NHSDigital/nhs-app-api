using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.AspNet;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.PfsApi.Areas.Appointments
{
    [Route("patient/appointments")]
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
                

                var userSession = HttpContext.GetUserSession();

                var appointmentValidator = GetAppointmentsValidationService(userSession);

                if (!appointmentValidator.IsDeleteValid(model))
                {
                    _logger.LogError("Invalid request body supplied to delete request");
                    return BadRequest();
                }

                await _auditor.Audit(AuditingOperations.CancelAppointmentAuditTypeRequest, $"Attempting to cancel appointment with id: {model.AppointmentId}");

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
                
                await _auditor.Audit(AuditingOperations.ViewAppointmentAuditTypeRequest, "Attempting to view booked appointments");
                              
                var userSession = HttpContext.GetUserSession();
                
                var result = await GetAppointmentsService(userSession).GetAppointments(userSession.GpUserSession);

                LogAppointmentsInformation(userSession.GpUserSession, result);
                
                await result.Accept(new AppointmentsAuditingVisitor(_auditor, _logger));
                return await result.Accept(new AppointmentsResultVisitor(_sessionCacheService, userSession));
            }
            finally
            {
                _logger.LogExit();
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AppointmentBookRequest model)
        {
            try
            {
                _logger.LogEnter();

                await _auditor.Audit(AuditingOperations.BookAppointmentAuditTypeRequest,
                    $"Attempting to book appointment with id: {model.SlotId} and startTime: {model.StartTime:O}");

                var userSession = HttpContext.GetUserSession();

                var appointmentValidator = GetAppointmentsValidationService(userSession);
                if (!appointmentValidator.IsPostValid(model))
                {
                    _logger.LogError("Invalid request body supplied to post request");
                    return BadRequest();
                }

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

        private IAppointmentsValidationService GetAppointmentsValidationService(UserSession userSession)
        {
            _logger.LogDebug($"Fetch Appointments Service for GP System: '{userSession.GpUserSession.Supplier}'.");

            return _gpSystemFactory
                .CreateGpSystem(userSession.GpUserSession.Supplier)
                .GetAppointmentsValidationService();
        }

        private void LogAppointmentsInformation(GpUserSession gpUserSession, AppointmentsResult result)
        {
            if (!(result is AppointmentsResult.Success successResult))
            {
                return;
            }

            try
            {
                var upcomingAppointmentsCount = successResult.Response?.UpcomingAppointments?.Count() ?? 0;
                var pastAppointmentsCount = successResult.Response?.PastAppointments?.Count() ?? 0;
            
                var kvp = new Dictionary<string, string>
                {
                    { "Supplier", gpUserSession.Supplier.ToString() },
                    { "OdsCode", gpUserSession.OdsCode },
                    { "Count", (upcomingAppointmentsCount+pastAppointmentsCount).ToString(CultureInfo.InvariantCulture) },
                    { "UpcomingCount", upcomingAppointmentsCount.ToString(CultureInfo.InvariantCulture) },
                    { "HistoricalCount", pastAppointmentsCount.ToString(CultureInfo.InvariantCulture)}
                };

                _logger.LogInformationKeyValuePairs("Appointment Count", kvp);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unable to log appointment details. " +
                                    "Catching exception to prevent inability to view appointments");
            }
        }
    }
}
