using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.Bridges.Emis.Models.Prescriptions
{
    public class CoursesGetResponse
    {
        public IEnumerable<MedicationCourse> Courses { get; set; }
    }
}
