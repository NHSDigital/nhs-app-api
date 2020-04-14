namespace NHSOnline.Backend.GpSystems.PatientRecord.Models
{
    public class PatientDocument
    {
        public string Content{ get; set; }
        public bool HasErrored { get; set; }
        public string Type { get; set; }
        public bool IsViewable { get; set; }
        public bool IsDownloadable { get; set; }
    }
}
