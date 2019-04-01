using System;
using System.Collections.Generic;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Appointments
{
    public class Slot
    {
        public string Id { get; set; }

        public string Type { get; set; }

        public DateTimeOffset? StartTime { get; set; }

        public DateTimeOffset? EndTime { get; set; }

        public string Location { get; set; }

        public IEnumerable<string> Clinicians { get; set; } = Array.Empty<string>();
        public string Channel { get; set; }
    }
}
