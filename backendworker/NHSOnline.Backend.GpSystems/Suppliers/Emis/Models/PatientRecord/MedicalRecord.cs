using System;
using System.Collections.Generic;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.PatientRecord
{
    public class MedicalRecord
    {
        public string PatientGuid { get; set; }
        public string Title { get; set; }
        public string Forenames { get; set; }
        public string Surname { get; set; }
        public string Sex { get; set; }
        public DateTimeOffset? DateOfBirth { get; set; }
        public List<Allergy> Allergies { get; set; }
        public List<Consultation> Consultations { get; set; }
        public List<Document> Documents { get; set; }
        public List<Immunisation> Immunisations { get; set; }
        public List<Medication> Medication { get; set; }
        public List<Problem> Problems { get; set; }
        public List<TestResult> TestResults { get; set; }
        public List<User> Users { get; set; }
    }
}
