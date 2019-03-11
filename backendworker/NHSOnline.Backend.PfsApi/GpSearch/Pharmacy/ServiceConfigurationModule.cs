using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.Worker.GpSearch.Pharmacy
{
    public class ServiceConfigurationModule: Backend.Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IPharmacyService, PharmacyService>();
            
            base.ConfigureServices(services, configuration);
        }
    }
}