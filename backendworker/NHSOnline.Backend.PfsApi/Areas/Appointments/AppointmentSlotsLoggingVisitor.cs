using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.PfsApi.Areas.Appointments
{
    public class AppointmentSlotsLoggingVisitor : IAppointmentSlotsResultVisitor<Task>
    {
        internal class AppointmentSlotsInformation
        {
            public string Supplier { get; }
            public string OdsCode { get; }
            public string[] SlotTypes { get; }
            public string[] SlotTypesFromGpSystem { get; }
            public string[] Locations { get; }
            public int SlotCount { get; }
            public int? FurthestSlotDays { get; }

            public AppointmentSlotsInformation(
                GpUserSession userSession,
                ICollection<Slot> slots)
            {
                Supplier = userSession.Supplier.ToString();
                OdsCode = userSession.OdsCode;
                SlotTypes = slots.Select(x => x.Type).Distinct().ToArray();
                SlotTypesFromGpSystem = slots.Select(x => x.TypeFromGpSystem).Distinct().ToArray();
                Locations = slots.Select(x => x.Location).Distinct().ToArray();
                SlotCount = slots.Count;
                FurthestSlotDays = DeriveFurthestSlotDays(slots);
            }
        }

        private readonly ILogger<AppointmentSlotsController> _logger;
        private readonly IAppointmentSlotMetadataLogger _appointmentSlotMetadataLogger;
        private readonly GpUserSession _gpUserSession;

        public AppointmentSlotsLoggingVisitor(
            ILogger<AppointmentSlotsController> logger,
            IAppointmentSlotMetadataLogger appointmentSlotMetadataLogger,
            GpUserSession gpUserSession)
        {
            _logger = logger;
            _appointmentSlotMetadataLogger = appointmentSlotMetadataLogger;
            _gpUserSession = gpUserSession;
        }

        public Task Visit(AppointmentSlotsResult.Success result)
        {
            try
            {
                LogAppointmentSlotsInformation(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unable to log appointment slot type details. " +
                                    "Catching exception to prevent inability to create appointment");
            }

            return Task.CompletedTask;
        }

        private void LogAppointmentSlotsInformation(AppointmentSlotsResult.Success result)
        {
            _appointmentSlotMetadataLogger.CaptureAppointmentSlotTypes(_gpUserSession, result);

            var slots = result.Response?.Slots ?? Array.Empty<Slot>();
            var slotCount = slots.Count;

            var kvp = new Dictionary<string, string>
            {
                { "Supplier", _gpUserSession.Supplier.ToString() },
                { "OdsCode", _gpUserSession.OdsCode },
                { "Count", slotCount.ToString(CultureInfo.InvariantCulture) }
            };

            _logger.LogInformationKeyValuePairs("Appointment Slot Count", kvp);

            var slotInfo = new AppointmentSlotsInformation(_gpUserSession, slots);

            _logger.LogInformation("appointment_slot_data=" + slotInfo.SerializeJson());
        }

        internal static int? DeriveFurthestSlotDays(ICollection<Slot> slots)
        {
            if (!slots.Any())
            {
                return null;
            }

            var furthestDate = slots.Select(x => x.StartTime).Max().Date;

            var furthestSlotDays = (furthestDate - DateTime.UtcNow.Date).Days;

            return furthestSlotDays;
        }

        #region No-ops for unsuccessful results
        public Task Visit(AppointmentSlotsResult.BadGateway result)
        {
            return Task.CompletedTask;
        }

        public Task Visit(AppointmentSlotsResult.InternalServerError result)
        {
            return Task.CompletedTask;
        }

        public Task Visit(AppointmentSlotsResult.Forbidden result)
        {
            return Task.CompletedTask;
        }
        #endregion
    }
}