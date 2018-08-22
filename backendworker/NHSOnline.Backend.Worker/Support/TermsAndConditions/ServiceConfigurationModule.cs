using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.Worker.Support.TermsAndConditions
{
    public class ServiceConfigurationModule : Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ITermsAndConditionsService, TermsAndConditionsService>();
            services.AddSingleton<ITermsAndConditionsConfig, TermsAndConditionsConfig>();

            base.ConfigureServices(services, configuration);
        }
    }
}