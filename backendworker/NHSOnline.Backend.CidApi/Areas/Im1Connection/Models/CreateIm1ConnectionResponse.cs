using System.Collections.Generic;
using NHSOnline.Backend.GpSystems.Im1Connection.Models;

namespace NHSOnline.Backend.CidApi.Areas.Im1Connection.Models
{
    public class CreateIm1ConnectionResponse
    {
        public string ConnectionToken { get; set; }
        public IEnumerable<PatientNhsNumber> NhsNumbers { get; set; }
        public string OdsCode { get; set; }
        public string AccountId { get; set; }
        public string LinkageKey { get; set; }
    }
}