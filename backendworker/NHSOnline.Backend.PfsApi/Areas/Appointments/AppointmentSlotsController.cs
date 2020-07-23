using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.PfsApi.Session;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Temporal;
using NHSOnline.Backend.Support.AspNet;
using NHSOnline.Backend.Support.Session;
using static NHSOnline.Backend.Support.Constants.HttpHeaders;

namespace NHSOnline.Backend.PfsApi.Areas.Appointments
{
    [ApiVersionRoute("patient/appointment-slots")]
    public class AppointmentSlotsController : Controller
    {
        private readonly IGpSystemFactory _gpSystemFactory;
        private readonly IDateTimeOffsetProvider _dateTimeOffsetProvider;
        private readonly ILogger<AppointmentSlotsController> _logger;
        private readonly IAuditor _auditor;
        private readonly IAppointmentSlotMetadataLogger _appointmentSlotMetadataLogger;
        private readonly IErrorReferenceGenerator _errorReferenceGenerator;
        private readonly IAppointmentTypeTransformingVisitor _appointmentTypeTransformingVisitor;
        private readonly AppointmentsConfigurationSettings _appointmentsConfigurationSettings;

        public AppointmentSlotsController(
            IGpSystemFactory gpSystemFactory,
            IDateTimeOffsetProvider dateTimeOffsetProvider,
            ILogger<AppointmentSlotsController> logger,
            IAuditor auditor,
            IAppointmentSlotMetadataLogger appointmentSlotMetadataLogger,
            IErrorReferenceGenerator errorReferenceGenerator,
            IAppointmentTypeTransformingVisitor appointmentTypeTransformingVisitor,
            AppointmentsConfigurationSettings appointmentsConfigurationSettings)
        {
            _gpSystemFactory = gpSystemFactory;
            _dateTimeOffsetProvider = dateTimeOffsetProvider;
            _logger = logger;
            _auditor = auditor;
            _appointmentSlotMetadataLogger = appointmentSlotMetadataLogger;
            _errorReferenceGenerator = errorReferenceGenerator;
            _appointmentTypeTransformingVisitor = appointmentTypeTransformingVisitor;
            _appointmentsConfigurationSettings = appointmentsConfigurationSettings;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromHeader(Name=PatientId)] Guid patientId, [UserSession] P9UserSession userSession)
        {
            try
            {
                _logger.LogEnter();
                _logger.LogDebug($"{nameof(Get)} with patientId {patientId}");

                await _auditor.Audit(AuditingOperations.GetSlotsAuditTypeRequest, "Attempting to get available appointments");

                _logger.LogDebug($"Fetch Appointment Slots Service for GP System: '{userSession.GpUserSession.Supplier}'.");
                var appointmentService = _gpSystemFactory.CreateGpSystem(userSession.GpUserSession.Supplier)
                    .GetAppointmentSlotsService();

                var dateRange = new AppointmentSlotsDateRange(_dateTimeOffsetProvider, _appointmentsConfigurationSettings.SixteenWeeksSlotsEnabled);

                var gpLinkedAccountsModel = new GpLinkedAccountModel(userSession.GpUserSession, patientId);

                var result = await appointmentService.GetSlots(gpLinkedAccountsModel, dateRange);

                await result.Accept(_appointmentTypeTransformingVisitor);
                await result.Accept(
                    new AppointmentSlotsLoggingVisitor(_logger, _appointmentSlotMetadataLogger, userSession));
                await result.Accept(new AppointmentSlotsAuditingVisitor(_auditor, _logger));
                return result.Accept(new AppointmentSlotsResultVisitor(_errorReferenceGenerator, userSession));
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}
