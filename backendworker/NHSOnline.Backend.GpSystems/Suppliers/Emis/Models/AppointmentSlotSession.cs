using System.Collections.Generic;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Models
{
    public class AppointmentSlotSession
    {
        public int SessionId { get; set; }
        public IEnumerable<AppointmentSlot> Slots { get; set; }
    }
}
