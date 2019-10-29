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
using static NHSOnline.Backend.Support.Constants.HttpHeaders;

namespace NHSOnline.Backend.PfsApi.Areas.Appointments
{
    [Route("patient/appointments")]
    public class AppointmentsController : Controller
    {
        private readonly ILogger<AppointmentsController> _logger;
        private readonly IGpSystemFactory _gpSystemFactory;
        private readonly IAuditor _auditor;
        private readonly ISessionCacheService _sessionCacheService;
        private readonly IErrorReferenceGenerator _errorReferenceGenerator;

        public AppointmentsController(
            ILogger<AppointmentsController> logger,
            IGpSystemFactory gpSystemFactory,
            IAuditor auditor,
            ISessionCacheService sessionCacheService,
            IErrorReferenceGenerator errorReferenceGenerator
            )
        {
            _logger = logger;
            _gpSystemFactory = gpSystemFactory;
            _auditor = auditor;
            _sessionCacheService = sessionCacheService;
            _errorReferenceGenerator = errorReferenceGenerator;
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] AppointmentCancelRequest model, [FromHeader(Name=PatientId)] Guid patientId)
        {
            try
            {
                _logger.LogEnter();
                _logger.LogDebug($"{nameof(Delete)} with patientId {patientId}");
   
                await _auditor.Audit(AuditingOperations.CancelAppointmentAuditTypeRequest, $"Attempting to cancel appointment with id: {model.AppointmentId}");
                
                var userSession = HttpContext.GetUserSession();
                
                AppointmentCancelResult cancelResult;
                
                var appointmentValidator = GetAppointmentsValidationService(userSession);
                if (!appointmentValidator.IsDeleteValid(model))
                {
                    _logger.LogError("Invalid request body supplied to delete request");
                    cancelResult = new AppointmentCancelResult.BadRequest();
                }
                else
                {
                    var appointmentsService = GetAppointmentsService(userSession);
                    
                    var gpLinkedAccountsModel = new GpLinkedAccountModel(userSession.GpUserSession, patientId);
                    
                    cancelResult = await appointmentsService.Cancel(gpLinkedAccountsModel, model);
                }
                
                await cancelResult.Accept(new AppointmentCancelAuditingVisitor(_auditor, _logger, model.AppointmentId));
                return cancelResult.Accept(new AppointmentCancelResultVisitor(_errorReferenceGenerator, userSession));
            }
            finally
            {
                _logger.LogExit();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromHeader(Name=PatientId)] Guid patientId)
        {
            try
            {
                _logger.LogEnter();
                _logger.LogDebug($"{nameof(Get)} with patientId {patientId}");
  
                await _auditor.Audit(AuditingOperations.ViewAppointmentAuditTypeRequest, "Attempting to view booked appointments");
                              
                var userSession = HttpContext.GetUserSession();
                
                var gpLinkedAccountUserSession = new GpLinkedAccountModel(
                    userSession.GpUserSession, patientId
                );
                
                var result = await GetAppointmentsService(userSession).GetAppointments(gpLinkedAccountUserSession);

                LogAppointmentsInformation(userSession.GpUserSession, result);
                
                await result.Accept(new AppointmentsAuditingVisitor(_auditor, _logger));
                return await result.Accept(new AppointmentsResultVisitor(_sessionCacheService, _errorReferenceGenerator, userSession));
            }
            finally
            {
                _logger.LogExit();
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AppointmentBookRequest model, [FromHeader(Name=PatientId)] Guid patientId)
        {
            try
            {
                _logger.LogEnter();
                _logger.LogDebug($"{nameof(Post)} with patientId {patientId}");

                await _auditor.Audit(AuditingOperations.BookAppointmentAuditTypeRequest,
                    $"Attempting to book appointment with id: {model.SlotId} and startTime: {model.StartTime:O}");

                var userSession = HttpContext.GetUserSession();

                AppointmentBookResult bookResult;

                var appointmentValidator = GetAppointmentsValidationService(userSession);
                if (!appointmentValidator.IsPostValid(model))
                {
                    _logger.LogError("Invalid request body supplied to post request");
                    bookResult = new AppointmentBookResult.BadRequest();
                }
                else
                {
                    var appointmentsService = GetAppointmentsService(userSession);

                    var gpLinkedAccountUserSession = new GpLinkedAccountModel(
                        userSession.GpUserSession, patientId
                    );
                    bookResult = await appointmentsService.Book(gpLinkedAccountUserSession, model);   
                }

                await bookResult.Accept(new AppointmentBookAuditingVisitor(_auditor, _logger, model.SlotId, model.StartTime));
                return bookResult.Accept(new AppointmentBookResultVisitor(_errorReferenceGenerator, userSession));
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
