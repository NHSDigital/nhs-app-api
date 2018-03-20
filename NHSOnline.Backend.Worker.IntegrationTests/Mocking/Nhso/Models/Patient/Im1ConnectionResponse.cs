using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.IntegrationTests.Mocking.Nhso.Models.Patient
{
    public class Im1ConnectionResponse
    {
        public string ConnectionToken { get; set; }
        public IEnumerable<PatientNhsNumber> NhsNumbers { get; set; }
    }
}
