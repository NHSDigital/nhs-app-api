namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models
{
    public interface ITppRequest
    {
        string RequestType { get; }

        void ApplyConfig(ITppConfig tppConfig);
    }
}