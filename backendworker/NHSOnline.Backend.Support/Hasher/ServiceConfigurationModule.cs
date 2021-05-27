using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.Support.Hasher
{
    public class ServiceConfigurationModule : DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<ISha512ProviderFactory, Sha512ProviderFactory>();
            services.AddSingleton<IHashingService, HashingService>();

            base.ConfigureServices(services, configuration);
        }
    }
}
