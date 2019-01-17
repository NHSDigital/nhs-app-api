namespace NHSOnline.Backend.Worker.Areas.MyRecord.Models
{
    public interface IVisionPatientDataModel : IPatientDataModel
    {
        string RawHtml { get; set; }
    }
}
