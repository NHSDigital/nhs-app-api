namespace NHSOnline.Backend.Worker.Areas.MyRecord.Models
{
    public class MyRecordResponse
    {
        public MyRecordResponse()
        {
            Allergies = new Allergies();
            Immunisations = new Immunisations();
            TestResults = new TestResults();
            Medications = new Medications();
        }
        
        public bool HasSummaryRecordAccess { get; set; }
        public bool HasDetailedRecordAccess { get; set; }
        
        public Allergies Allergies { get; set; }
        public Medications Medications { get; set; }
        public Immunisations Immunisations { get; set; }
        public TestResults TestResults { get; set; }
        public Problems Problems { get; set; }
    }
}
