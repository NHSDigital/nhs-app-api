namespace NHSOnline.Backend.GpSystems.PatientRecord.Models
{
    public class DocumentItem
    {
        public MyRecordDate EffectiveDate { get; set; }
        public string DocumentIdentifier { get; set; }
        public string Term { get; set; }
        public long? Size { get; set; }
        public string Extension { get; set; }
        public bool IsAvailable { get; set; }
        public string Name { get; set; }
        public string EventGuid { get; set; }
        public long? CodeId { get; set; }
        public string Type { get; set; }
        public bool IsValidFile { get; set; }
        public string Comments { get; set; }
    }
}