using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.Bridges.Emis.Models.Prescriptions
{
    public class MedicationCourse
    {
        public string MedicationCourseGuid { get; set; }

        public string Name { get; set; }

        public string Dosage { get; set; }

        public string QuantityRepresentation { get; set; }

        public PrescriptionType PrescriptionType { get; set; }

        public IEnumerable<string> Constituents { get; set; }

        public bool CanBeRequested { get; set; }
    }
}
