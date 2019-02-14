using System;
using System.Collections.Generic;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.Prescriptions
{
    public class PrescriptionRequest
    {
        public DateTimeOffset DateRequested { get; set; }

        public IEnumerable<RequestedMedicationCourse> RequestedMedicationCourses { get; set; }
    }
}
