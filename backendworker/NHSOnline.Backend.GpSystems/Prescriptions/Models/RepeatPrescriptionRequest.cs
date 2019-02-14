using System.Collections.Generic;
using NHSOnline.Backend.Support.ValidationAttributes;

namespace NHSOnline.Backend.GpSystems.Prescriptions.Models
{
    public class RepeatPrescriptionRequest
    {
        public IEnumerable<string> CourseIds { get; set; }

        [SafeString]
        public string SpecialRequest { get; set; }
    }
}
