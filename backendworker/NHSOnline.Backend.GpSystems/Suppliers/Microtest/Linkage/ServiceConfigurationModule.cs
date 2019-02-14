using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.Linkage
{
    public class ServiceConfigurationModule : Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<MicrotestLinkageService>();
            services.AddSingleton<MicrotestLinkageRequestValidationService>();

            base.ConfigureServices(services, configuration);
        }
    }
}
