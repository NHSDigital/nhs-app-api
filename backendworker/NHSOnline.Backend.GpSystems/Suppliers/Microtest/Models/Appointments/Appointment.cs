using System;
using System.Collections.Generic;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Appointments
{
    public class Appointment
    {
        public string Id { get; set; }

        public string Type { get; set; }

        public string StartTime { get; set; }

        public string EndTime { get; set; }

        public string Location { get; set; }

        public IEnumerable<string> Clinicians { get; set; } = Array.Empty<string>();
        public string Channel { get; set; }
        public string TelephoneNumber { get; set; }
    }
}
