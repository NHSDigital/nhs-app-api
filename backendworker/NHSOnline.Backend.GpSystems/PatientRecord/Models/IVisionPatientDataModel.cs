namespace NHSOnline.Backend.GpSystems.PatientRecord.Models
{
    public interface IVisionPatientDataModel : IPatientDataModel
    {
        string RawHtml { get; set; }
    }
}
