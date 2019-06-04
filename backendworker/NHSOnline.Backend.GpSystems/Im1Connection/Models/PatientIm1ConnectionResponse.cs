using System.Collections.Generic;

namespace NHSOnline.Backend.GpSystems.Im1Connection.Models
{
    public class PatientIm1ConnectionResponse
    {
        public string ConnectionToken { get; set; }
        public IEnumerable<PatientNhsNumber> NhsNumbers { get; set; }
        public string OdsCode { get; set; }
        public string AccountId { get; set; }
        public string LinkageKey { get; set; }
    }
}