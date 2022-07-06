using System.Collections.Generic;

namespace NHSOnline.Backend.GpSystems.PatientRecord.Models
{
    public class ImmunisationItem
    {
        public string Term { get; set; }
        public List<string> AssociatedText { get; set; } = new List<string>();
        public MyRecordDate EffectiveDate { get; set; }
        public string Status { get; set; }
        public MyRecordDateRawString NextDate { get; set; }
    }
}