using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.PfsApi.SecondaryCare
{
    public class ServiceConfigurationModule : Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<ISecondaryCareClient, SecondaryCareClient>();

            services.AddSingleton<ISecondaryCareService, SecondaryCareService>();
            services.AddSingleton<SecondaryCareSummaryService>();

            base.ConfigureServices(services, configuration);
        }
    }
}