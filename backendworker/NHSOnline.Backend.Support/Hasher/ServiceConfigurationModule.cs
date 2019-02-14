using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Cryptography;

namespace NHSOnline.Backend.Support.Hasher
{
    public class ServiceConfigurationModule : Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<SHA512CryptoServiceProvider>();
            services.AddSingleton<IHashingService, HashingService>();
            base.ConfigureServices(services, configuration);
        }
    }
}
