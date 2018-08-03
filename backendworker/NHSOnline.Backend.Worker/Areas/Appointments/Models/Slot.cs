using System;
using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.Areas.Appointments.Models
{
    public class Slot
    {
        public string Id { get; set; }
        public DateTimeOffset StartTime { get; set; } 
        public DateTimeOffset? EndTime { get; set; }
        public string Location { get; set; }
        public IEnumerable<string> Clinicians { get; set; }
        public string Type { get; set; }
    }
}
