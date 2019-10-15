using System.Collections.Generic;

namespace NHSOnline.Backend.GpSystems.PatientRecord.Models
{
    public class DocumentItem
    {
        public MyRecordDate EffectiveDate { get; set; }
        public string DocumentGuid { get; set; }
        public string Term { get; set; }
        public int Size { get; set; }
        public string Extension { get; set; }
        public bool IsAvailable { get; set; }
        public string Name { get; set; }
    }
}