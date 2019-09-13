namespace NHSOnline.Backend.GpSystems.PatientRecord.Models
{
    public class MedicalHistoryItem
    {
        public MyRecordDate StartDate { get; set; }
        public string Rubric { get; set; }
        public string Description { get; set; }
    }
}