using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.GpSystems
{
    public interface IGpSystemRegistrationService
    {
        void RegisterGpSystemsServices(IServiceCollection serviceCollection, EnableGpSupplierConfiguration enableGpSupplierConfiguration);
    }
}
