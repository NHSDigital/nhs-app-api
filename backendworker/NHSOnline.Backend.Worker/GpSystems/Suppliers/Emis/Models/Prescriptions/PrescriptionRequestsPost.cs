using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.Prescriptions
{
    public class PrescriptionRequestsPost
    {
        public IEnumerable<string> MedicationCourseGuids { get; set; }

        public string RequestComment { get; set; }

        public string UserPatientLinkToken { get; set; }
    }
}
