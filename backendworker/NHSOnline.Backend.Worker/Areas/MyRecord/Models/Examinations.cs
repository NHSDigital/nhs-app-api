namespace NHSOnline.Backend.Worker.Areas.MyRecord.Models
{
    public class Examinations : IVisionPatientDataModel
    {
        public Examinations()
        {
            HasAccess = true;
            HasErrored = false;
        }

        public bool HasAccess { get; set; }
        public bool HasErrored { get; set; }
        public string RawHtml { get; set; }
    }
}