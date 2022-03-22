using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace NHSOnline.HttpMocks.Emis.Models.Prescriptions
{
    [SuppressMessage("Usage", "CA2227:Collection properties should be read only",
        Justification = "Required for mocks")]
    public class MedicationCourse
    {
        public string? MedicationCourseGuid { get; set; }
        public string? Name { get; set; }
        public string? Dosage { get; set; }
        public string? QuantityRepresentation { get; set; }
        public PrescriptionType? PrescriptionType { get; set; }
        public IList<string>? Constituents { get; set; }
        public bool CanBeRequested { get; set; }
    }
}