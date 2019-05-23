using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.ClinicalDecisionSupportApi.ServiceDefinition
{
    public class ServiceConfigurationModule : NHSOnline.Backend.Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IServiceDefinitionService, ServiceDefinitionService>();
            
            base.ConfigureServices(services, configuration);
        }
    }
}