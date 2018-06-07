using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models
{
    public class AppointmentsSlotsGetResponse
    {
        public IEnumerable<AppointmentSlotSession> Sessions { get; set; }
    }
}