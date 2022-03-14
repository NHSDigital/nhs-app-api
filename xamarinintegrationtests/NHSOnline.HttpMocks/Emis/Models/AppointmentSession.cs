using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace NHSOnline.HttpMocks.Emis.Models
{
    public class AppointmentSession
    {
        public string? SessionDate { set; get; }
        public string? SessionId { set; get; }

        [SuppressMessage("Usage", "CA2227:Collection properties should be read only",
            Justification = "Required for mocks")]
        public IList<AppointmentSlots> Slots { set; get; } = new List<AppointmentSlots>();
    }

    public class ASession
    {
        public string? SessionId{ get; set; }
        public string? LocationId { get; set; }
    }

    public class Location
    {
        public string? LocationId { get; set; }
        public string? LocationName { get; set; }
    }

    public class AppointmentSlots
    {
        public string? SlotId { get; set; }
        public string? StartTime { get; set; }
        public string? EndTime { get; set; }
        public string? SlotTypeName { get; set; }

        public SlotTypeStatus SlotTypeStatus { get; set; } = SlotTypeStatus.Practice;
        public string? TelephoneNumber { get; set; }
    }

    public enum SlotTypeStatus
    {
        Practice,
    }
}