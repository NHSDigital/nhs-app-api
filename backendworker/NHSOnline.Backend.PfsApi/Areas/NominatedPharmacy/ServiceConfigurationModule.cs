using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.PfsApi.Areas.NominatedPharmacy
{
    public class ServiceConfigurationModule: Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IPharmacyDetailsToPharmacyDetailsResponseMapper, PharmacyDetailsToPharmacyDetailsResponseMapper>();
            
            base.ConfigureServices(services, configuration);
        }
    }
}