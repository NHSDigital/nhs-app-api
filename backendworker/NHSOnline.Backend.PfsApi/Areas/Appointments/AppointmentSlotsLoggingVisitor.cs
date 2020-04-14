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
                P9UserSession userSession,
                ICollection<Slot> slots)
            {
                Supplier = userSession.GpUserSession.Supplier.ToString();
                OdsCode = userSession.GpUserSession.OdsCode;
                SlotTypes = slots.Select(x => x.Type).Distinct().ToArray();
                SlotTypesFromGpSystem = slots.Select(x => x.TypeFromGpSystem).Distinct().ToArray();
                Locations = slots.Select(x => x.Location).Distinct().ToArray();
                SlotCount = slots.Count;
                FurthestSlotDays = DeriveFurthestSlotDays(slots);
            }
        }

        private readonly ILogger<AppointmentSlotsController> _logger;
        private readonly IAppointmentSlotMetadataLogger _appointmentSlotMetadataLogger;
        private readonly P9UserSession _userSession;

        public AppointmentSlotsLoggingVisitor(
            ILogger<AppointmentSlotsController> logger,
            IAppointmentSlotMetadataLogger appointmentSlotMetadataLogger,
            P9UserSession userSession)
        {
            _logger = logger;
            _appointmentSlotMetadataLogger = appointmentSlotMetadataLogger;
            _userSession = userSession;
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
            var gpUserSession = _userSession.GpUserSession;

            _appointmentSlotMetadataLogger.CaptureAppointmentSlotTypes(gpUserSession, result);

            var slots = result.Response?.Slots ?? Array.Empty<Slot>();
            var slotCount = slots.Count;

            var kvp = new Dictionary<string, string>
            {
                { "Supplier", gpUserSession.Supplier.ToString() },
                { "OdsCode", gpUserSession.OdsCode },
                { "Count", slotCount.ToString(CultureInfo.InvariantCulture) }
            };

            _logger.LogInformationKeyValuePairs("Appointment Slot Count", kvp);

            var slotInfo = new AppointmentSlotsInformation(_userSession, slots);

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