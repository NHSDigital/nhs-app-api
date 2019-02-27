using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.GpSystems
{
    public class GpSystemRegistrationService : IGpSystemRegistrationService
    {
        public void RegisterGpSystemsServices(IServiceCollection serviceCollection, EnableGpSupplierConfiguration enableGpSupplierConfiguration)
        {
            serviceCollection.RegisterGpSystemsServices(enableGpSupplierConfiguration);
        }
    }
}
