using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.GpSystems
{
    public interface IGpSystemRegistrationService
    {
        void RegisterPfsServices(IServiceCollection serviceCollection, EnableGpSupplierConfiguration enableGpSupplierConfiguration);

        void RegisterPfsServices(IServiceCollection serviceCollection);

        void RegisterCidServices(IServiceCollection serviceCollection);

        void RegisterCidServices(IServiceCollection serviceCollection, EnableGpSupplierConfiguration enableGpSupplierConfiguration);

    }
}
