using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace NHSOnline.Backend.GpSystems.Appointments.Models
{
    public class Slot : ISlot
    {
        public string Id { get; set; }
        public DateTimeOffset StartTime { get; set; } 
        public DateTimeOffset? EndTime { get; set; }
        public string Location { get; set; }
        public IEnumerable<string> Clinicians { get; set; }
        public string Type { get; set; }
        
        [JsonIgnore]
        public string TypeFromGpSystem { get; set; }
        public string SessionName { get; set; }
        public Channel Channel { get; set; } = Channel.Unknown;
    }
}