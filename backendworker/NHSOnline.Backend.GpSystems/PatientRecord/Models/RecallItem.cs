namespace NHSOnline.Backend.GpSystems.PatientRecord.Models
{
    public class RecallItem
    {
        public MyRecordDate RecordDate { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Result { get; set; }
        public string NextDate { get; set; }
        public string Status { get; set; }
    }
}