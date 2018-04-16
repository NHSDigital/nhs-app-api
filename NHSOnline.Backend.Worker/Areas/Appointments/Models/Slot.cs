using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NHSOnline.Backend.Worker.Areas.Appointments.Models
{
    public class Slot
    {
        public string Id { get; set; }
        public DateTime StartTime { get; set; } 
        public DateTime EndTime { get; set; }
        public string LocationId { get; set; }
        public string AppointmentSessionId { get; set; }
        public string[] ClinicianIds { get; set; }
    }
}
