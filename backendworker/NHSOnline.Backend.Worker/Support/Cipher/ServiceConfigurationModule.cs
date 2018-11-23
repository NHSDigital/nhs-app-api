using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.Worker.Settings;

namespace NHSOnline.Backend.Worker.Support.Cipher
{
    public class ServiceConfigurationModule : Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<CipherConfiguration>();
            services.AddSingleton<ICipherService, CipherService>();
            base.ConfigureServices(services, configuration);
        }
    }
}
