namespace NHSOnline.Backend.Worker.Areas.MyRecord.Models
{
    public class Procedures : IVisionPatientDataModel
    {
        public Procedures()
        {
            HasAccess = true;
            HasErrored = false;
        }

        public bool HasAccess { get; set; }
        public bool HasErrored { get; set; }
        public string RawHtml { get; set; }
    }
}