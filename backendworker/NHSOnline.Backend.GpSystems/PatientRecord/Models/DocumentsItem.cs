namespace NHSOnline.Backend.GpSystems.PatientRecord.Models
{
    public class DocumentItem
    {
        public MyRecordDate EffectiveDate { get; set; }
        public string DocumentGuid { get; set; }
        public string Term { get; set; }
        public long? Size { get; set; }
        public string Extension { get; set; }
        public bool IsAvailable { get; set; }
        public string Name { get; set; }
        public string eventGuid { get; set; }
        public long? codeId { get; set; }
        public bool IsValidFile { get; set; }
    }
}