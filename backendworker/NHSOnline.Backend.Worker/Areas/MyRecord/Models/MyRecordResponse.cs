namespace NHSOnline.Backend.Worker.Areas.MyRecord.Models
{
    public class MyRecordResponse
    {
        public MyRecordResponse()
        {
            Allergies = new Allergies();
            Medications = new Medications();
            Immunisations = new Immunisations();
            Problems = new Problems();
            TestResults = new TestResults();
            TppDcrEvents = new TppDcrEvents();
            Consultations = new Consultations();
            Diagnosis = new Diagnosis();
        }
        
        public bool HasSummaryRecordAccess { get; set; }
        public bool HasDetailedRecordAccess { get; set; }
        public string Supplier {get; set;}
        public Allergies Allergies { get; set; }
        public Medications Medications { get; set; }
        public Immunisations Immunisations { get; set; }
        public TestResults TestResults { get; set; }
        public Problems Problems { get; set; }
        public TppDcrEvents TppDcrEvents { get; set; }
        public Consultations Consultations { get; set; }
        public Diagnosis Diagnosis { get; set; }
    }
}
