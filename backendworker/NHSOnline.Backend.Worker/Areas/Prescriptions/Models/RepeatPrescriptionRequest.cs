using System.Collections.Generic;
using NHSOnline.Backend.Worker.ValidationAttributes;

namespace NHSOnline.Backend.Worker.Areas.Prescriptions.Models
{
    public class RepeatPrescriptionRequest
    {
        public IEnumerable<string> CourseIds { get; set; }

        [SafeString]
        public string SpecialRequest { get; set; }
    }
}
