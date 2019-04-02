using System;
using Newtonsoft.Json;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Appointments
{
    [Serializable]
    public class CancelAppointmentDeleteRequest
    {
        public CancelAppointmentDeleteRequest(string appointmentId, string cancelReason)
        {
            AppointmentId = appointmentId;
            CancelReason = cancelReason;
        }

        [JsonProperty("appointmentId")]
        public string AppointmentId { get; }

        [JsonProperty("cancelReason")]
        public string CancelReason { get; }
    }
}