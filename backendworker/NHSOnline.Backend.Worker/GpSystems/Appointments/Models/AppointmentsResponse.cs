using System.Collections.Generic;
using Newtonsoft.Json;

namespace NHSOnline.Backend.Worker.GpSystems.Appointments.Models
{
    public class AppointmentsResponse
    {
        public IEnumerable<PastAppointment> PastAppointments { get; set; } = new List<PastAppointment>();
        
        // TechDebt: NHSO-4080
        // JsonProperty to be removed in the next release
        [JsonProperty("appointments")]
        public IEnumerable<UpcomingAppointment> UpcomingAppointments { get; set; } = new List<UpcomingAppointment>();
        
        public IEnumerable<CancellationReason> CancellationReasons { get; set; } = new List<CancellationReason>();

        public bool DisableCancellation { get; set; }
        public bool PastAppointmentsEnabled { get; set; }
    }
}
