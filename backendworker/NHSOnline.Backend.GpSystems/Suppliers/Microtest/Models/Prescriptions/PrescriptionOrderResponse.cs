using System.Collections.Generic;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Prescriptions
{
    public class PrescriptionOrderResponse
    {
        public IEnumerable<PatientRequest> PatientRequests { get; set; }
    }
}
