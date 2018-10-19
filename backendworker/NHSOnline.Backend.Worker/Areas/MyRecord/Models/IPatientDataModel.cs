namespace NHSOnline.Backend.Worker.Areas.MyRecord.Models
{
    public interface IPatientDataModel
    {
        bool HasAccess { get; set; }
        bool HasErrored { get; set; }
    }
}