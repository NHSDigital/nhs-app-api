using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.Models.Patient
{
    public class PatientIm1ConnectionResponse
    {
        public IEnumerable<PatientNhsNumber> NhsNumbers { get; set; }
    }
}
