using System;
using System.Collections.Concurrent;
using System.Linq;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.Areas.Appointments
{
    public interface IAppointmentSlotTypeScraper
    {
        void CaptureAppointmentSlotTypes(UserSession userSession, AppointmentSlotsResult result);
    }

    public class AppointmentSlotTypeScraper : IAppointmentSlotTypeScraper
    {
        private static readonly ConcurrentDictionary<string, AppointmentSlotsValue> CapturedAppointmentTypes = 
            new ConcurrentDictionary<string, AppointmentSlotsValue>();
        
        private class AppointmentSlotsInformation
        {
            public string[] SlotTypes { get; }
            public string[] SessionNames { get; }
            public string Supplier { get; }
            public string OdsCode { get; }

            public AppointmentSlotsInformation(string[] slotTypes, string[] sessionNames, string supplier, string odsCode)
            {
                SlotTypes = slotTypes;
                SessionNames = sessionNames;
                Supplier = supplier;
                OdsCode = odsCode;
            }
        }
        
        private class AppointmentSlotsValue
        {
            public DateTime CurrentDate {get;}
            public int SlotCount {get;}

            public AppointmentSlotsValue(DateTime currentDate, int slotCount)
            {
                CurrentDate = currentDate;
                SlotCount = slotCount;
            }
        }
        
        private readonly ILogger<AppointmentSlotTypeScraper> _logger;

        public AppointmentSlotTypeScraper(
            ILogger<AppointmentSlotTypeScraper> logger
        )
        {
            _logger = logger;
        }

        public void CaptureAppointmentSlotTypes(UserSession userSession, AppointmentSlotsResult result)
        {
            if (!(result is AppointmentSlotsResult.SuccessfullyRetrieved successfulResult) 
                || successfulResult.Response.Slots?.Any() != true)
            {
                return;
            }
            
            var slotTypes = successfulResult.Response.Slots.Select(x => x.Type).Distinct().ToArray();
            var sessionNames = successfulResult.Response.Slots.Select(x => x.SessionName).Distinct().ToArray();
            var appointmentSlotsInformation = new AppointmentSlotsInformation(slotTypes, sessionNames,
                userSession.GpUserSession.Supplier.ToString(), userSession.GpUserSession.OdsCode);
            
            if (!ShouldBeLogged(appointmentSlotsInformation))
            {
                return;
            }
            _logger.LogInformation("slot_type_data=" + appointmentSlotsInformation.SerializeJson());
        }

        private static bool ShouldBeLogged(AppointmentSlotsInformation appointmentSlotsInformation)
        {
            var appointmentSlotsValue =
                new AppointmentSlotsValue(DateTime.Today, appointmentSlotsInformation.SlotTypes.Length);
            
            CapturedAppointmentTypes.TryGetValue(appointmentSlotsInformation.OdsCode, out var existingValue);
            
            if (existingValue == null || 
                existingValue.CurrentDate.Date != DateTime.Today.Date ||
                existingValue.SlotCount < appointmentSlotsInformation.SlotTypes.Length)

            {
                CapturedAppointmentTypes.AddOrUpdate(appointmentSlotsInformation.OdsCode, appointmentSlotsValue,
                    (key, oldValue) => appointmentSlotsValue);
                return true;
            }
            
            return false;
        }
    }
}

