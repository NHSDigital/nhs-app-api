using System.Collections.Generic;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Models
{
    public class AppointmentSlotsGetResponse
    {
        public IEnumerable<AppointmentSlotSession> Sessions { get; set; }
    }
}