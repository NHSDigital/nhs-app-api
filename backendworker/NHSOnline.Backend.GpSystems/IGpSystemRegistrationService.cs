using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.GpSystems
{
    public interface IGpSystemRegistrationService
    {
        void RegisterPfsServices(IServiceCollection serviceCollection,
            EnableGpSupplierConfiguration enableGpSupplierConfiguration,
            bool isHealthCheckLoggingEnabled);

        void RegisterPfsServices(IServiceCollection serviceCollection, bool isHealthCheckLoggingEnabled);

        void RegisterCidServices(IServiceCollection serviceCollection, bool isHealthCheckLoggingEnabled);

        void RegisterCidServices(IServiceCollection serviceCollection,
            EnableGpSupplierConfiguration enableGpSupplierConfiguration,
            bool isHealthCheckLoggingEnabled);

    }
}
