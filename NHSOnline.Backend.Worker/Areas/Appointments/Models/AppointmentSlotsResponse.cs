using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NHSOnline.Backend.Worker.Areas.Appointments.Models
{
    public class AppointmentSlotsResponse
    {
        public Clinician[] Clinicians { get; set; }
        public AppointmentSession[] AppointmentSessions { get; set; }
        public Location[] Locations { get; set; }
        public Slot[] Slots { get; set; }
    }
}
