using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.PatientRecord;

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
            Problems = new Problems();
            TppDcrEvents = new TppDcrEvents();
            Consultations = new Consultations();
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
    }
}
