using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.PfsApi.Configuration;

namespace NHSOnline.Backend.PfsApi.KnownServices
{
    public class ServiceConfigurationModule : Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IKnownServicesService, KnownServicesService>();

            base.ConfigureServices(services, configuration);
        }
    }
}