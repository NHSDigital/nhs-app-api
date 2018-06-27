using System;
using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.PatientRecord.Model
{
    public class Immunisation
    {
        public string ObservationType { get; set; }
        public string Episodicity { get; set; }
        public double? NumericValue { get; set; }
        public string NumericOperator { get; set; }
        public string NumericUnits { get; set; }
        public string TextValue { get; set; }
        public Range Range { get; set; }
        public bool Abnormal { get; set; }
        public string AbnormalReason { get; set; }
        public List<AssociatedText> AssociatedText { get; set; }
        public string EventGuid { get; set; }
        public string Term { get; set; }
        public DateTimeOffset? AvailabilityDateTime { get; set; }
        public EffectiveDate EffectiveDate { get; set; }
        public long? CodeId { get; set; }
        public string AuthorisingUserInRoleGuid { get; set; }
        public string EnteredByUserInRoleGuid { get; set; }
    }
}