namespace NHSOnline.Backend.GpSystems.PatientRecord.Models
{
    public class ReferralItem
    {
        public MyRecordDate RecordDate { get; set; }
        public string Description { get; set; }
        public string Speciality { get; set; }
        public string Ubrn { get; set; }
    }
}