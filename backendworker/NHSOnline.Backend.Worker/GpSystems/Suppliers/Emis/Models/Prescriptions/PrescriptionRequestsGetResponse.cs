using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.Prescriptions
{
    public class PrescriptionRequestsGetResponse
    {
        public IEnumerable<PrescriptionRequest> PrescriptionRequests { get; set; }

        public IEnumerable<MedicationCourse> MedicationCourses { get; set; }
    }
}
