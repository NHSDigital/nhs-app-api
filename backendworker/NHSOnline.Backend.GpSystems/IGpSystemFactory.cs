using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems
{
    public interface IGpSystemFactory
    {
        IGpSystem CreateGpSystem(Supplier supplier);
    }
}