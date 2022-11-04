using System;
using System.Collections.Generic;

namespace NHSOnline.HttpMocks.Emis.Models.Prescriptions
{
    public class PrescriptionRequest
    {
        public string? RequestedByDisplayName { get; set; }

        public string? RequestedByForenames { get; set; }

        public string? RequestedBySurname { get; set; }

        public DateTimeOffset DateRequested { get; set; }

        public IEnumerable<RequestedMedicationCourse> RequestedMedicationCourses { get; set; } =
            new List<RequestedMedicationCourse>();
    }
}