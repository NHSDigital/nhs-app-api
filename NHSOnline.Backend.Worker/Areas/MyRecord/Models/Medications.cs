namespace NHSOnline.Backend.Worker.Areas.MyRecord.Models
{
    public class Medications
    {  
        public bool HasAccess { get; set; }
        public bool HasErrored { get; set; }
        public string[] Errors { get; set; }
        public MedicationsData Data { get; set; }
    }
}