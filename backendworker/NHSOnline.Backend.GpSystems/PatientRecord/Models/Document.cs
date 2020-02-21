namespace NHSOnline.Backend.GpSystems.PatientRecord.Models
{
    public class PatientDocument
    {
        public string Content{ get; set; }
        public bool HasErrored { get; set; }
        public string Type { get; set; }
    }
}