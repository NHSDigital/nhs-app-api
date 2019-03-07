using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.ServiceJourneyRulesApi.Service;

namespace NHSOnline.Backend.ServiceJourneyRulesApi
{
    public class ServiceConfigurationModule: NHSOnline.Backend.Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IServiceJourneyRulesService, ServiceJourneyRulesService>();           
            base.ConfigureServices(services, configuration);
        }
    }
}