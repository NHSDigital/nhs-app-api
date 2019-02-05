namespace NHSOnline.Backend.Worker.GpSystems.PatientRecord.Models
{
    public class Diagnosis : IVisionPatientDataModel
    {
        public Diagnosis()
        {
            HasAccess = true;
            HasErrored = false;
        }

        public bool HasAccess { get; set; }
        public bool HasErrored { get; set; }
        public string RawHtml { get; set; }
    }
}
