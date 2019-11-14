using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Temporal;
using NHSOnline.Backend.Support.AspNet;
using static NHSOnline.Backend.Support.Constants.HttpHeaders;

namespace NHSOnline.Backend.PfsApi.Areas.Appointments
{
    [Route("patient/appointment-slots")]
    public class AppointmentSlotsController : Controller
    {
        internal class AppointmentSlotsInformation
        {
            public string Supplier { get; }
            public string OdsCode { get; }
            public string[] SlotTypes { get; }
            public string[] SlotTypesFromGpSystem { get; }
            public string[] Locations { get; }
            public int SlotCount { get; }

            public AppointmentSlotsInformation(
                string supplier,
                string odsCode,
                string[] slotTypes, 
                string[] slotTypesFromGpSystem,
                string[] locations, 
                int slotCount)
            {
                Supplier = supplier;
                OdsCode = odsCode;
                SlotTypes = slotTypes;
                SlotTypesFromGpSystem = slotTypesFromGpSystem;
                Locations = locations;
                SlotCount = slotCount;
            }
        }
        
        private readonly IGpSystemFactory _gpSystemFactory;
        private readonly IDateTimeOffsetProvider _dateTimeOffsetProvider;
        private readonly ILogger<AppointmentSlotsController> _logger;
        private readonly IAuditor _auditor;
        private readonly IAppointmentSlotMetadataLogger _appointmentSlotMetadataLogger;
        private readonly IErrorReferenceGenerator _errorReferenceGenerator;
        private readonly IAppointmentTypeTransformingVisitor _appointmentTypeTransformingVisitor;

        public AppointmentSlotsController(
            IGpSystemFactory gpSystemFactory,
            IDateTimeOffsetProvider dateTimeOffsetProvider,
            ILogger<AppointmentSlotsController> logger,
            IAuditor auditor,
            IAppointmentSlotMetadataLogger appointmentSlotMetadataLogger, 
            IErrorReferenceGenerator errorReferenceGenerator,
            IAppointmentTypeTransformingVisitor appointmentTypeTransformingVisitor)
        {
            _gpSystemFactory = gpSystemFactory;
            _dateTimeOffsetProvider = dateTimeOffsetProvider;
            _logger = logger;
            _auditor = auditor;
            _appointmentSlotMetadataLogger = appointmentSlotMetadataLogger;
            _errorReferenceGenerator = errorReferenceGenerator;
            _appointmentTypeTransformingVisitor = appointmentTypeTransformingVisitor;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromHeader(Name=PatientId)] Guid patientId)
        {
            try
            {
                _logger.LogEnter();
                _logger.LogDebug($"{nameof(Get)} with patientId {patientId}");
               
                await _auditor.Audit(AuditingOperations.GetSlotsAuditTypeRequest, "Attempting to get available appointments");

                var userSession = HttpContext.GetUserSession();
                _logger.LogDebug($"Fetch Appointment Slots Service for GP System: '{userSession.GpUserSession.Supplier}'.");
                var appointmentService = _gpSystemFactory.CreateGpSystem(userSession.GpUserSession.Supplier)
                    .GetAppointmentSlotsService();

                var dateRange = new AppointmentSlotsDateRange(_dateTimeOffsetProvider);

                var gpLinkedAccountsModel = new GpLinkedAccountModel(userSession.GpUserSession, patientId);

                var result = await appointmentService.GetSlots(gpLinkedAccountsModel, dateRange);
                
                await result.Accept(_appointmentTypeTransformingVisitor);
                
                LogAppointmentSlotInformation(userSession.GpUserSession, result);

                await result.Accept(new AppointmentSlotsAuditingVisitor(_auditor, _logger));
                return result.Accept(new AppointmentSlotsResultVisitor(_errorReferenceGenerator, userSession));
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private void LogAppointmentSlotInformation(GpUserSession gpUserSession, AppointmentSlotsResult result)
        {
            if (!(result is AppointmentSlotsResult.Success successResult))
            {
                return;
            }
            
            try
            {
                _appointmentSlotMetadataLogger.CaptureAppointmentSlotTypes(gpUserSession, successResult);
                
                var slots = successResult.Response?.Slots ?? Array.Empty<Slot>();
                var slotCount = slots.Count();
            
                var kvp = new Dictionary<string, string>
                {
                    { "Supplier", gpUserSession.Supplier.ToString() },
                    { "OdsCode", gpUserSession.OdsCode },
                    { "Count", slotCount.ToString(CultureInfo.InvariantCulture) }
                };

                _logger.LogInformationKeyValuePairs("Appointment Slot Count", kvp);

                var slotInfo = new AppointmentSlotsInformation(
                    gpUserSession.Supplier.ToString(),
                    gpUserSession.OdsCode,
                    slots.Select(x => x.Type).Distinct().ToArray(),
                    slots.Select(x => x.TypeFromGpSystem).Distinct().ToArray(),
                    slots.Select(x => x.Location).Distinct().ToArray(),
                    slotCount);

                _logger.LogInformation("appointment_slot_data=" + slotInfo.SerializeJson());
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unable to log appointment slot type details. " +
                                    "Catching exception to prevent inability to create appointment");
            }
        }
    }
}
