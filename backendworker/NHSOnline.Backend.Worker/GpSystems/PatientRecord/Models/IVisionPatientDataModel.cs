namespace NHSOnline.Backend.Worker.GpSystems.PatientRecord.Models
{
    public interface IVisionPatientDataModel : IPatientDataModel
    {
        string RawHtml { get; set; }
    }
}
