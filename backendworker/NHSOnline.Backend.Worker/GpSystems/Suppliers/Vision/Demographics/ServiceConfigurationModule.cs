using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Demographics
{
    public class ServiceConfigurationModule : Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<VisionDemographicsService>();
            
            services.AddTransient<IVisionDemographicsMapper, VisionDemographicsMapper>();
            base.ConfigureServices(services, configuration);
        }
    }
}