namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models
{
    public interface ITppApplicationRequest : ITppRequest
    {
        Application Application { get; set; }
    }
}