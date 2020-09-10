using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.Support.Settings;

namespace NHSOnline.Backend.Support.Cipher
{
    public class ServiceConfigurationModule : DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<CipherConfiguration>();
            services.AddSingleton<ICipherService, CipherService>();
            base.ConfigureServices(services, configuration);
        }
    }
}
