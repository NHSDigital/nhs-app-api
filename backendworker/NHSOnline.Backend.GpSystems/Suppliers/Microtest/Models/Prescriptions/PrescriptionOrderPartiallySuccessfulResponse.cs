using System.Collections.Generic;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Prescriptions
{
    public class PrescriptionOrderPartiallySuccessfulResponse
    {
        public IEnumerable<PatientRequest> PatientRequests { get; set; }
    }
}
