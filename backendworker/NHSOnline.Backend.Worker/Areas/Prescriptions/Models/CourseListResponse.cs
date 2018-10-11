using System.Collections.Generic;
using NHSOnline.Backend.Worker.Areas.SharedModels;

namespace NHSOnline.Backend.Worker.Areas.Prescriptions.Models
{
    public class CourseListResponse
    {
        public IEnumerable<Course> Courses { get; set; }

        public Necessity SpecialRequestNecessity { get; set; } = Necessity.Optional;
    }
}
