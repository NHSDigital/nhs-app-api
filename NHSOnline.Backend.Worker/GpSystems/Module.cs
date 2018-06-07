using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.Worker.GpSystems
{
    public class Module : Support.DependencyInjection.Module
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IGpSystemFactory, GpSystemFactory>();

            base.ConfigureServices(services, configuration);
        }
    }
}
