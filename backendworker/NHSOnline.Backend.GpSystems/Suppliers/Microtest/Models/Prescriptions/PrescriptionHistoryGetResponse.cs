using System.Collections.Generic;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Prescriptions
{
    public class PrescriptionHistoryGetResponse
    {
        public IEnumerable<PrescriptionCourse> Courses { get; set; } = new List<PrescriptionCourse>();
    }
}
