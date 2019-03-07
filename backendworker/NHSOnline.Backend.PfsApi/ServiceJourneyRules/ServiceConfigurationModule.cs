using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.PfsApi.ServiceJourneyRules
{
    public class ServiceConfigurationModule: NHSOnline.Backend.Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IServiceJourneyRulesService, ServiceJourneyRulesService>();
            services.AddTransient<IServiceJourneyRulesClient, ServiceJourneyRulesClient>();
            services.AddTransient<IServiceJourneyRulesConfig, ServiceJourneyRulesConfig>();
            services.AddHttpClient<ServiceJourneyRulesHttpClient>();         
            
            base.ConfigureServices(services, configuration);
        }
    }
}