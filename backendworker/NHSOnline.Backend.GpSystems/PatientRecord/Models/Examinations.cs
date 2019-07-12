namespace NHSOnline.Backend.GpSystems.PatientRecord.Models
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
        public int RecordCount => RawHtml?.Length ?? 0;
        public string RawHtml { get; set; }
    }
}