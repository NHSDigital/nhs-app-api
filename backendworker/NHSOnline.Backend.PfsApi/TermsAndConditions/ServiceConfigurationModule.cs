using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.PfsApi.TermsAndConditions
{
    public class ServiceConfigurationModule : Backend.Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<ITermsAndConditionsService, TermsAndConditionsService>();
            services.AddSingleton<ITermsAndConditionsConfig, TermsAndConditionsConfig>();

            base.ConfigureServices(services, configuration);
        }
    }
}