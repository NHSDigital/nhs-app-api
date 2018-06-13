using System;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.PatientRecord
{
    public class MedicationResponse
    {     
        public string Dosage { get; set; }
        public string QuantityRepresentation { get; set; }
        public bool IsMixture { get; set; }
        public MedicationMixtureResponse Mixture { get; set; }
        public DateTimeOffset FirstIssueDate { get; set; }  
        public DateTimeOffset LastIssueDate { get; set; }  
        public string PrescriptionType { get; set; }
        public string DrugStatus { get; set; }
        public string EventGuidId { get; set; }
        public string Term { get; set; }
    }
}