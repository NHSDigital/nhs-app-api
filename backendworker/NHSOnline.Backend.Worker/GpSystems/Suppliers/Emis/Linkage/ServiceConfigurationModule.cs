using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Linkage
{
    public class ServiceConfigurationModule : Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<EmisLinkageService>();
            
            services.AddTransient<IEmisLinkageMapper, EmisLinkageMapper>();
            base.ConfigureServices(services, configuration);
        }
    }
}
