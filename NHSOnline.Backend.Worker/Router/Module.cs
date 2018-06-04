using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.Worker.Router
{
    public class Module : Support.DependencyInjection.Module
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IBridgeFactory, BridgeFactory>();

            base.ConfigureServices(services, configuration);
        }
    }
}
