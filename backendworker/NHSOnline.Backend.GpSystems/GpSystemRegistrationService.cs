using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.GpSystems
{
    public class GpSystemRegistrationService : IGpSystemRegistrationService
    {
        public void RegisterPfsServices(IServiceCollection serviceCollection, EnableGpSupplierConfiguration enableGpSupplierConfiguration)
        {
            serviceCollection.RegisterPfsGpSystemsServices(enableGpSupplierConfiguration);
        }

        public void RegisterPfsServices(IServiceCollection serviceCollection)
        {
            serviceCollection.RegisterPfsGpSystemsServices(null);
        }

        public void RegisterCidServices(IServiceCollection serviceCollection)
        {
            serviceCollection.RegisterCidGpSystemsServices(null);
        }

        public void RegisterCidServices(IServiceCollection serviceCollection, EnableGpSupplierConfiguration enableGpSupplierConfiguration)
        {
            serviceCollection.RegisterCidGpSystemsServices(enableGpSupplierConfiguration);
        }
    }
}
