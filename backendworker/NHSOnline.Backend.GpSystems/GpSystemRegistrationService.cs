using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.GpSystems
{
    public class GpSystemRegistrationService : IGpSystemRegistrationService
    {
        public void RegisterPfsServices(
            IServiceCollection serviceCollection,
            EnableGpSupplierConfiguration enableGpSupplierConfiguration,
            bool isHealthCheckLoggingEnabled)
        {
            serviceCollection.RegisterPfsGpSystemsServices(enableGpSupplierConfiguration, isHealthCheckLoggingEnabled);
        }

        public void RegisterPfsServices(IServiceCollection serviceCollection, bool isHealthCheckLoggingEnabled)
        {
            serviceCollection.RegisterPfsGpSystemsServices(null, isHealthCheckLoggingEnabled);
        }

        public void RegisterCidServices(IServiceCollection serviceCollection, bool isHealthCheckLoggingEnabled)
        {
            serviceCollection.RegisterCidGpSystemsServices(null, isHealthCheckLoggingEnabled);
        }

        public void RegisterCidServices(
            IServiceCollection serviceCollection,
            EnableGpSupplierConfiguration enableGpSupplierConfiguration,
            bool isHealthCheckLoggingEnabled)
        {
            serviceCollection.RegisterCidGpSystemsServices(enableGpSupplierConfiguration, isHealthCheckLoggingEnabled);
        }
    }
}
