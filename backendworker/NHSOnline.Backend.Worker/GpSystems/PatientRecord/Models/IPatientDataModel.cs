namespace NHSOnline.Backend.Worker.GpSystems.PatientRecord.Models
{
    public interface IPatientDataModel
    {
        bool HasAccess { get; set; }
        bool HasErrored { get; set; }
    }
}