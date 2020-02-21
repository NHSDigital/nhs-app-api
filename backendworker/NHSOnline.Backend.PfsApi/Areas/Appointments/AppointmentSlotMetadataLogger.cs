using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Temporal;

namespace NHSOnline.Backend.PfsApi.Areas.Appointments
{
    public interface IAppointmentSlotMetadataLogger
    {
        void CaptureAppointmentSlotTypes(GpUserSession gpUserSession, AppointmentSlotsResult.Success result);
    }

    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global Members are marked as virtual to allow partial mocking in unit tests
    public class AppointmentSlotMetadataLogger : IAppointmentSlotMetadataLogger
    {
        private static readonly ConcurrentDictionary<string, AppointmentSlotsValue> CapturedAppointmentTypes = 
            new ConcurrentDictionary<string, AppointmentSlotsValue>();

        internal class Slot
        {
            public string SessionName { get; }
            
            public string SlotType { get; }

            public Slot(string slotType, string sessionName)
            {
                SlotType = slotType;
                SessionName = sessionName;
            }
        }
        
        internal class AppointmentSlotsInformation
        {
            public string[] SlotTypes { get; }
            public string[] SessionNames { get; }
            public string[] Locations { get; }
            public Slot[] Slots { get; }
            public string Supplier { get; }
            public string OdsCode { get; }
            public bool HasBookingGuidance { get; }
            public string BookingGuidance { get;  }

            public AppointmentSlotsInformation(
                string[] slotTypes, 
                string[] sessionNames, 
                string[] locations,
                Slot[] slots, 
                string supplier, 
                string odsCode,
                string bookingGuidance)
            {
                SlotTypes = slotTypes;
                SessionNames = sessionNames;
                Locations = locations;
                Slots = slots;
                Supplier = supplier;
                OdsCode = odsCode;

                if (!string.IsNullOrWhiteSpace(bookingGuidance))
                {
                    HasBookingGuidance = true;
                    BookingGuidance = bookingGuidance;
                }
            }
        }
        
        private class AppointmentSlotsValue
        {
            public DateTime CurrentDate { get; }
            public int SlotTypeCount { get; }

            public AppointmentSlotsValue(DateTime currentDate, int slotTypeCount)
            {
                CurrentDate = currentDate;
                SlotTypeCount = slotTypeCount;
            }
        }
        
        private readonly ILogger<AppointmentSlotMetadataLogger> _logger;
        private readonly ICurrentDateTimeProvider _dateTimeProvider;

        public AppointmentSlotMetadataLogger(
            ILogger<AppointmentSlotMetadataLogger> logger,
            ICurrentDateTimeProvider dateTimeProvider
        )
        {
            _logger = logger;
            _dateTimeProvider = dateTimeProvider;
        }

        public void CaptureAppointmentSlotTypes(GpUserSession gpUserSession, AppointmentSlotsResult.Success result)
        {
            if (result.Response.Slots?.Any() == false || gpUserSession==null)
            {
                return;
            }

            var appointmentSlotsInformation = BuildSlotsInformation(gpUserSession, result);

            if (ShouldBeLogged(appointmentSlotsInformation))
            {
                LogInformation(appointmentSlotsInformation);
            }
        }

        internal virtual AppointmentSlotsInformation BuildSlotsInformation(GpUserSession gpUserSession, 
            AppointmentSlotsResult.Success successfulResult)
        {
            Debug.Assert(gpUserSession != null);
            Debug.Assert(successfulResult != null);
            Debug.Assert(successfulResult.Response.Slots?.Any() == true);

            var response = successfulResult.Response;
            var responseSlots = response.Slots;
            var slotTypes = responseSlots.Select(x => x.Type).Distinct().ToArray();
            var sessionNames = responseSlots.Select(x => x.SessionName).Distinct().ToArray();
            var locations = responseSlots.Select(x => x.Location).Distinct().ToArray();
            var slots = responseSlots.Select(s => new { s.Type, s.SessionName }).Distinct()
                                     .Select(s => new Slot(s.Type, s.SessionName)).ToArray();
            var bookingGuidance = response.BookingGuidance;
            
            var appointmentSlotsInformation = new AppointmentSlotsInformation(
                slotTypes, sessionNames, locations, slots,
                gpUserSession.Supplier.ToString(), gpUserSession.OdsCode,
                bookingGuidance);

            return appointmentSlotsInformation;
        }

        internal virtual bool ShouldBeLogged(AppointmentSlotsInformation appointmentSlotsInformation)
        {
            Debug.Assert(appointmentSlotsInformation!=null);
            
            var today = _dateTimeProvider.Today;
            
            var appointmentSlotsValue =
                new AppointmentSlotsValue(today, appointmentSlotsInformation.SlotTypes.Length);
            
            CapturedAppointmentTypes.TryGetValue(appointmentSlotsInformation.OdsCode, out var existingValue);
            
            if (existingValue == null || 
                existingValue.CurrentDate.Date != today ||
                existingValue.SlotTypeCount < appointmentSlotsInformation.SlotTypes.Length)

            {
                CapturedAppointmentTypes.AddOrUpdate(appointmentSlotsInformation.OdsCode, appointmentSlotsValue,
                    (key, oldValue) => appointmentSlotsValue);
                return true;
            }
            
            return false;
        }

        internal virtual void LogInformation(AppointmentSlotsInformation appointmentSlotsInformation)
        {
            Debug.Assert(appointmentSlotsInformation!=null);
            _logger.LogInformation("slot_type_data=" + appointmentSlotsInformation.SerializeJson());
        }
    }
}

