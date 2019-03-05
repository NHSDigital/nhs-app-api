using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.GpSystems
{
    public class GpSystemRegistrationService : IGpSystemRegistrationService
    {
        public void RegisterPfsServices(IServiceCollection serviceCollection, EnableGpSupplierConfiguration enableGpSupplierConfiguration)
        {
            serviceCollection.RegisterPfsGpSystemsServices(enableGpSupplierConfiguration);
        }
        
        public void RegisterCidServices(IServiceCollection serviceCollection, EnableGpSupplierConfiguration enableGpSupplierConfiguration)
        {
            serviceCollection.RegisterCidGpSystemsServices(enableGpSupplierConfiguration);
        }
    }
}
