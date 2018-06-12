namespace NHSOnline.Backend.Worker.Areas.MyRecord.Models
{
    public class MyRecordResponse
    {
        public MyRecordResponse()
        {
            Allergies = new Allergies();
        }
        
        public bool HasSummaryRecordAccess { get; set; }
        public bool HasDetailedRecordAccess { get; set; }
        
        public Allergies Allergies { get; set; }
    }
}
