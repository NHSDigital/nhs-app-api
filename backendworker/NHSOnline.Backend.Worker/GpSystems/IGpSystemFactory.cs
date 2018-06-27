namespace NHSOnline.Backend.Worker.GpSystems
{
    public interface IGpSystemFactory
    {
        IGpSystem CreateGpSystem(SupplierEnum supplier);
    }
}