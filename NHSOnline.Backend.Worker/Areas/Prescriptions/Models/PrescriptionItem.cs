using System;
using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.Areas.Prescriptions.Models
{
    public class PrescriptionItem
    {
        public IEnumerable<CourseEntry> Courses { get; set; }
        
        public DateTimeOffset OrderDate { get; set; }
    }
}
