using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.GpSystems.SessionManager;
using NHSOnline.Backend.Metrics;
using NHSOnline.Backend.PfsApi.GpSession;
using NHSOnline.Backend.PfsApi.Session;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.AspNet;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Support.Session;
using static NHSOnline.Backend.Support.Constants.HttpHeaders;

namespace NHSOnline.Backend.PfsApi.Areas.Appointments
{
    [ApiVersionRoute("patient/appointments")]
    public class AppointmentsController : Controller
    {
        private readonly ILogger<AppointmentsController> _logger;
        private readonly IGpSystemFactory _gpSystemFactory;
        private readonly IAuditor _auditor;
        private readonly ISessionCacheService _sessionCacheService;
        private readonly IErrorReferenceGenerator _errorReferenceGenerator;
        private readonly IAppointmentTypeTransformingVisitor _appointmentTypeTransformingVisitor;
        private readonly IAnonymousMetricLogger _anonymousMetricLogger;
        private readonly IMetricLogger _metricLogger;

        public AppointmentsController(
            ILogger<AppointmentsController> logger,
            IGpSystemFactory gpSystemFactory,
            IAuditor auditor,
            ISessionCacheService sessionCacheService,
            IErrorReferenceGenerator errorReferenceGenerator,
            IAppointmentTypeTransformingVisitor appointmentTypeTransformingVisitor,
            IAnonymousMetricLogger anonymousMetricLogger,
            IMetricLogger metricLogger)
        {
            _logger = logger;
            _gpSystemFactory = gpSystemFactory;
            _auditor = auditor;
            _sessionCacheService = sessionCacheService;
            _errorReferenceGenerator = errorReferenceGenerator;
            _appointmentTypeTransformingVisitor = appointmentTypeTransformingVisitor;
            _anonymousMetricLogger = anonymousMetricLogger;
            _metricLogger = metricLogger;
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(
            [FromBody] AppointmentCancelRequest request,
            [FromHeader(Name=PatientId)] Guid patientId,
            [GpSession] GpUserSession gpUserSession,
            [UserSession] P9UserSession p9UserSession)
        {
            try
            {
                _logger.LogEnter();
                _logger.LogDebug($"{nameof(Delete)} with patientId {patientId}");

                await _auditor.Audit(
                    AuditingOperations.CancelAppointmentAuditTypeRequest,
                    $"Attempting to cancel appointment with id: {request.AppointmentId}");

                var result = await Cancel(request, gpUserSession, patientId);

                await result.Accept(new AppointmentCancelAuditingVisitor(_auditor, _logger, request.AppointmentId, _metricLogger, p9UserSession));

                return await result.Accept(
                    new AppointmentCancelResultVisitor(request, _errorReferenceGenerator, gpUserSession.Supplier, _anonymousMetricLogger));
            }
            finally
            {
                _logger.LogExit();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Get(
            [FromHeader(Name=PatientId)] Guid patientId,
            [UserSession] P9UserSession p9UserSession,
            [GpSession] GpUserSession gpUserSession)
        {
            try
            {
                _logger.LogEnter();
                _logger.LogDebug($"{nameof(Get)} with patientId {patientId}");

                await _auditor.Audit(
                    AuditingOperations.ViewAppointmentAuditTypeRequest,
                    "Attempting to view booked appointments");

                var gpLinkedAccountModel = new GpLinkedAccountModel(gpUserSession, patientId);

                var result = await GetAppointmentsService(gpUserSession).GetAppointments(gpLinkedAccountModel);

                await result.Accept(_appointmentTypeTransformingVisitor);
                await result.Accept(new AppointmentsLoggingVisitor(_logger, p9UserSession));
                await result.Accept(new AppointmentsAuditingVisitor(_auditor, _logger));

                return await result.Accept(
                    new AppointmentsResultVisitor(
                        _sessionCacheService, _errorReferenceGenerator, p9UserSession, gpUserSession.Supplier));
            }
            finally
            {
                _logger.LogExit();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post(
            [FromBody] AppointmentBookRequest request,
            [FromHeader(Name=PatientId)] Guid patientId,
            [GpSession] GpUserSession gpUserSession,
            [UserSession] P9UserSession p9UserSession)
        {
            try
            {
                _logger.LogEnter();
                _logger.LogDebug($"{nameof(Post)} with patientId {patientId}");

                await _auditor.Audit(
                    AuditingOperations.BookAppointmentAuditTypeRequest,
                    $"Attempting to book appointment with id: {request.SlotId} and " +
                    $"startTime: {request.StartTime:O}");

                var result = await Book(request, gpUserSession, patientId);

                await result.Accept(
                    new AppointmentBookAuditingVisitor(_auditor, _logger, request.SlotId, request.StartTime, _metricLogger, p9UserSession));

                return await result.Accept(new AppointmentBookResultVisitor(request, _errorReferenceGenerator, gpUserSession.Supplier, _anonymousMetricLogger));
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private async Task<AppointmentCancelResult> Cancel(
            AppointmentCancelRequest request,
            GpUserSession userSession,
            Guid patientId)
        {
            var appointmentValidator = GetAppointmentsValidationService(userSession);

            if (!appointmentValidator.IsDeleteValid(request))
            {
                _logger.LogError("Invalid request body supplied to delete request");
                return new AppointmentCancelResult.BadRequest();
            }

            var appointmentsService = GetAppointmentsService(userSession);
            var gpLinkedAccountsModel = new GpLinkedAccountModel(userSession, patientId);

            return await appointmentsService.Cancel(gpLinkedAccountsModel, request);
        }

        private async Task<AppointmentBookResult> Book(
            AppointmentBookRequest request,
            GpUserSession userSession,
            Guid patientId)
        {
            var appointmentValidator = GetAppointmentsValidationService(userSession);

            if (!appointmentValidator.IsPostValid(request))
            {
                _logger.LogError("Invalid request body supplied to post request");

                return new AppointmentBookResult.BadRequest();
            }

            var appointmentsService = GetAppointmentsService(userSession);
            var gpLinkedAccountModel = new GpLinkedAccountModel(userSession, patientId);

            return await appointmentsService.Book(gpLinkedAccountModel, request);
        }

        private IAppointmentsService GetAppointmentsService(GpUserSession userSession)
        {
            _logger.LogDebug($"Fetch Appointments Service for GP System: '{userSession.Supplier}'.");

            return _gpSystemFactory
                .CreateGpSystem(userSession.Supplier)
                .GetAppointmentsService();
        }

        private IAppointmentsValidationService GetAppointmentsValidationService(GpUserSession userSession)
        {
            _logger.LogDebug($"Fetch Appointments Service for GP System: '{userSession.Supplier}'.");

            return _gpSystemFactory
                .CreateGpSystem(userSession.Supplier)
                .GetAppointmentsValidationService();
        }
    }
}
