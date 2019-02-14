using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Models
{
    [SuppressMessage("Microsoft.Naming", "CA1724", Justification = "Deliberately matching the name specified by the GPSS")]
    public class Session
    {
        public string SessionType { get; set; }
        public string SessionName { get; set; }
        public int SessionId { get; set; }
        public int LocationId { get; set; }
        public IEnumerable<int> ClinicianIds { get; set; }
    }
}
