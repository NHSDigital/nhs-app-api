namespace NHSOnline.Backend.Worker.Suppliers
{
    public interface ISystemProviderFactory
    {
        ISystemProvider CreateSystemProvider(SupplierEnum supplier);
    }
}