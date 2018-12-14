using System.Linq;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.GpSystems.Appointments;

namespace NHSOnline.Backend.Worker.Areas.Appointments
{
    public interface IAppointmentSlotTypeScraper
    {
        void CaptureAppointmentSlotTypes(UserSession userSession, AppointmentSlotsResult result);
    }
    
    public class AppointmentSlotTypeScraper : IAppointmentSlotTypeScraper
    {
        private readonly ILogger<AppointmentSlotTypeScraper> _logger;

        public AppointmentSlotTypeScraper(
            ILogger<AppointmentSlotTypeScraper> logger
        )
        {
            _logger = logger;
        }

        public void CaptureAppointmentSlotTypes(UserSession userSession, AppointmentSlotsResult result)
        {
            if (!(result is AppointmentSlotsResult.SuccessfullyRetrieved successfulResult))
            {
                return;
            }
            var slotTypes = successfulResult.Response.Slots.Select(x => x.Type).Distinct().ToArray();
            var appointmentSlotsInformation = new AppointmentSlotTypesDetails
            {
                OdsCode = userSession.OdsCode,
                Supplier = userSession.Supplier.ToString(),
                SlotTypes = slotTypes
            };
            _logger.LogInformation("slot_type_data=" + appointmentSlotsInformation.SerializeJson());
        }
        
        private class AppointmentSlotTypesDetails
        {
            public string[] SlotTypes { get; set; }
            public string Supplier { get; set; }
            public string OdsCode { get;set; }
        }
    }
}
