namespace NHSOnline.Backend.GpSystems.PatientRecord.Models
{
    public interface IPatientDataModel
    {
        bool HasAccess { get; set; }
        bool HasErrored { get; set; }
        int RecordCount { get; }
    }
}