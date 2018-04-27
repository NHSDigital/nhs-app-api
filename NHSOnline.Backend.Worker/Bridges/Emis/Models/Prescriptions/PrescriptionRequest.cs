using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.Bridges.Emis.Models.Prescriptions
{
    public class PrescriptionRequest
    {
        public DateTimeOffset DateRequested { get; set; }

        public IEnumerable<RequestedMedicationCourse> RequestedMedicationCourses { get; set; }
    }
}
