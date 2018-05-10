using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.Areas.Prescriptions.Models
{
    public class CourseListResponse
    {
        public IEnumerable<Course> Courses { get; set; }
    }
}
