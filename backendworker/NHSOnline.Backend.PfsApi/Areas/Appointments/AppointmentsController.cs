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
        private readonly IAppointmentTypeTransformingVisitor _appointmentTypeTransformingVisitor;

        public AppointmentsController(
            ILogger<AppointmentsController> logger,
            IGpSystemFactory gpSystemFactory,
            IAuditor auditor,
            ISessionCacheService sessionCacheService,
            IErrorReferenceGenerator errorReferenceGenerator,
            IAppointmentTypeTransformingVisitor appointmentTypeTransformingVisitor
            )
        {
            _logger = logger;
            _gpSystemFactory = gpSystemFactory;
            _auditor = auditor;
            _sessionCacheService = sessionCacheService;
            _errorReferenceGenerator = errorReferenceGenerator;
            _appointmentTypeTransformingVisitor = appointmentTypeTransformingVisitor;
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] AppointmentCancelRequest request, [FromHeader(Name=PatientId)] Guid patientId)
        {
            try
            {
                _logger.LogEnter();
                _logger.LogDebug($"{nameof(Delete)} with patientId {patientId}");

                await _auditor.Audit(AuditingOperations.CancelAppointmentAuditTypeRequest, $"Attempting to cancel appointment with id: {request.AppointmentId}");

                var userSession = HttpContext.GetUserSession();
                
                var result = await Cancel(request, userSession, patientId);

                await result.Accept(new AppointmentCancelAuditingVisitor(_auditor, _logger, request.AppointmentId));
                return result.Accept(new AppointmentCancelResultVisitor(_errorReferenceGenerator, userSession));
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
                
                var gpLinkedAccountModel = new GpLinkedAccountModel(
                    userSession.GpUserSession, patientId
                );
                
                var result = await GetAppointmentsService(userSession).GetAppointments(gpLinkedAccountModel);

                await result.Accept(_appointmentTypeTransformingVisitor);
                
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
        public async Task<IActionResult> Post([FromBody] AppointmentBookRequest request, [FromHeader(Name=PatientId)] Guid patientId)
        {
            try
            {
                _logger.LogEnter();
                _logger.LogDebug($"{nameof(Post)} with patientId {patientId}");

                await _auditor.Audit(AuditingOperations.BookAppointmentAuditTypeRequest,
                    $"Attempting to book appointment with id: {request.SlotId} and startTime: {request.StartTime:O}");

                var userSession = HttpContext.GetUserSession();
                var result = await Book(request, userSession, patientId);
                
                await result.Accept(new AppointmentBookAuditingVisitor(_auditor, _logger, request.SlotId, request.StartTime));
                return result.Accept(new AppointmentBookResultVisitor(_errorReferenceGenerator, userSession));
            }
            finally
            {
                _logger.LogExit();
            }
        }
        
        private async Task<AppointmentCancelResult> Cancel(AppointmentCancelRequest request, UserSession userSession, Guid patientId)
        {
            var appointmentValidator = GetAppointmentsValidationService(userSession);
            if (!appointmentValidator.IsDeleteValid(request))
            {
                _logger.LogError("Invalid request body supplied to delete request");
                return new AppointmentCancelResult.BadRequest();
            }

            var appointmentsService = GetAppointmentsService(userSession);
            var gpLinkedAccountsModel = new GpLinkedAccountModel(userSession.GpUserSession, patientId);
            return await appointmentsService.Cancel(gpLinkedAccountsModel, request);
        }

        private async Task<AppointmentBookResult> Book(AppointmentBookRequest request, UserSession userSession, Guid patientId)
        {
            var appointmentValidator = GetAppointmentsValidationService(userSession);
            if (!appointmentValidator.IsPostValid(request))
            {
                _logger.LogError("Invalid request body supplied to post request");
                return new AppointmentBookResult.BadRequest();
            }

            var appointmentsService = GetAppointmentsService(userSession);
            var gpLinkedAccountModel = new GpLinkedAccountModel(
                userSession.GpUserSession, patientId
            );
            return await appointmentsService.Book(gpLinkedAccountModel, request);
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
