using System;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.PatientRecord
{
    public class Medication
    {
        public string DrugName { get; set; }
        public string Dosage { get; set; }
        public double? Quantity { get; set; }
        public string QuantityUnit { get; set; }
        public string QuantityRepresentation { get; set; }
        public bool IsMixture { get; set; }
        public Mixture Mixture { get; set; }
        public DateTimeOffset? FirstIssueDate { get; set; }
        public DateTimeOffset? LastIssueDate { get; set; }
        public string PrescriptionType { get; set; }
        public string DrugStatus { get; set; }
        public string EventGuid { get; set; }
        public string Term { get; set; }
        public DateTimeOffset? AvailabilityDateTime { get; set; }
        public EffectiveDate EffectiveDate { get; set; }
        public long? CodeId { get; set; }
        public string AuthorisingUserInRoleGuid { get; set; }
        public string EnteredByUserInRoleGuid { get; set; }
    }
}