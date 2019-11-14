using System;
using System.Collections.Generic;

namespace NHSOnline.Backend.GpSystems.Appointments.Models
{
    public interface ISlot
    {
        DateTimeOffset StartTime { get; set; }
        DateTimeOffset? EndTime { get; set; }
        string Location { get; set; }
        IEnumerable<string> Clinicians { get; set; }
        string Type { get; set; }
        string TypeFromGpSystem { get; set; }
        string SessionName { get; set; }
        Channel Channel { get; set; }
    }
}