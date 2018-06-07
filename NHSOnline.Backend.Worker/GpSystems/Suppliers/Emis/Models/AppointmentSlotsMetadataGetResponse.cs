using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models
{
    public class AppointmentSlotsMetadataGetResponse
    {
        public IEnumerable<Location> Locations { get; set; }
        public IEnumerable<SessionHolder> SessionHolders { get; set; }
        public IEnumerable<Session> Sessions { get; set; }
    }
}