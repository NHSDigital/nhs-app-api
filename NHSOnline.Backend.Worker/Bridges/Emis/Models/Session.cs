using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.Bridges.Emis.Models
{
    public class Session
    {
        public string SessionType { get; set; }
        public string SessionName { get; set; }
        public int SessionId { get; set; }
        public int LocationId { get; set; }
        public IEnumerable<int> ClinicianIds { get; set; }
    }
}
