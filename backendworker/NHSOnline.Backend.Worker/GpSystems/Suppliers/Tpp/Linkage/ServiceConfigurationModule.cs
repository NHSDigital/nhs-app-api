using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Linkage
{
    public class ServiceConfigurationModule : Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<TppLinkageService>();
            services.AddSingleton<TppLinkageRequestValidationService>();
            services.AddTransient<ITppLinkageMapper, TppLinkageMapper>();
            
            base.ConfigureServices(services, configuration);
        }
    }
}