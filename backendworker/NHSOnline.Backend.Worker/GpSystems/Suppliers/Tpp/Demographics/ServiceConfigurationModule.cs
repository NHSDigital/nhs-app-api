using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Demographics
{
    public class ServiceConfigurationModule : Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<TppDemographicsService>();
            
            services.AddTransient<ITppDemographicsMapper, TppDemographicsMapper>();
            base.ConfigureServices(services, configuration);
        }
    }
}
