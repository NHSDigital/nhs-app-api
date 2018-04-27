using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.Areas.Prescriptions.Models
{
    public class PrescriptionListResponse
    {
        public IEnumerable<PrescriptionItem> Prescriptions { get; set; }

        public IEnumerable<Course> Courses { get; set; }
    }
}
