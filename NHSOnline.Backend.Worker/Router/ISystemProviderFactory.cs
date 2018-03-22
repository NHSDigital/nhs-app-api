namespace NHSOnline.Backend.Worker.Router
{
    public interface ISystemProviderFactory
    {
        ISystemProvider CreateSystemProvider(SupplierEnum supplier);
    }
}