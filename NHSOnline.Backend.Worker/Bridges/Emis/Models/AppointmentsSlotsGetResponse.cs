using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.Bridges.Emis.Models
{
    public class AppointmentsSlotsGetResponse
    {
        public IEnumerable<AppointmentSlotSession> Sessions { get; set; }
    }
}