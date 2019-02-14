using System.Collections.Generic;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.Prescriptions
{
    public class CoursesGetResponse
    {
        public IEnumerable<MedicationCourse> Courses { get; set; }
    }
}
