using System.Collections.Generic;

namespace NHSOnline.Backend.GpSystems.PatientRecord.Models
{
    public class ProblemItem
    {
        public MyRecordDate EffectiveDate { get; set; }     
        public List<ProblemLineItem> LineItems { get; set; }   
    }
}