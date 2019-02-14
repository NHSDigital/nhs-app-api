using System;
using System.Collections.Generic;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.PatientRecord
{
    public class Consultation
    {
        public string Location { get; set; }
        public string ConsultantName { get; set; }
        public List<Section> Sections { get; set; }
        public string EventGuid { get; set; }
        public string Term { get; set; }
        public DateTimeOffset? AvailabilityDateTime { get; set; }
        public EffectiveDate EffectiveDate { get; set; }
        public long? CodeId { get; set; }
        public string AuthorisingUserInRoleGuid { get; set; }
        public string EnteredByUserInRoleGuid { get; set; }
    }
}