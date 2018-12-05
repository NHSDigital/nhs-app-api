using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.Worker.GpSystems.Linkage;

namespace NHSOnline.Backend.Worker.GpSystems
{
    public class ServiceConfigurationModule : Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IGpSystemFactory, GpSystemFactory>();
            services.AddSingleton<IIm1CacheService, Im1CacheService>();
            services.AddSingleton<IIm1CacheKeyGenerator, Im1CacheKeyGenerator>();

            base.ConfigureServices(services, configuration);
        }
    }
}
