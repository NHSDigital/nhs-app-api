namespace NHSOnline.Backend.GpSystems.PatientRecord.Models
{
    public class EncounterItem
    {
        public MyRecordDate RecordedOn { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }
        public string Unit { get; set; }
        
    }
}