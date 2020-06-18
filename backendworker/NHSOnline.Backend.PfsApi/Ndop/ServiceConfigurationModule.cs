using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.PfsApi.Ndop
{
    public class ServiceConfigurationModule: Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<INdopService, NdopService>();
            services.AddTransient<INdopSigning, NdopSigning>();
            base.ConfigureServices(services, configuration);
        }
    }
}