namespace NHSOnline.Backend.GpSystems.PatientRecord.Models
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
        public int RecordCount { get; set; }
        public string RawHtml { get; set; }
    }
}