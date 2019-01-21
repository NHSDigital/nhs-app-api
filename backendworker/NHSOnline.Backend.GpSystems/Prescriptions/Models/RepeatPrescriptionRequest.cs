using System.Collections.Generic;

namespace NHSOnline.Backend.GpSystems.Prescriptions.Models
{
    public class RepeatPrescriptionRequest
    {
        public IEnumerable<string> CourseIds { get; set; }
        
        public string SpecialRequest { get; set; }
    }
}
