using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.Support.Certificate
{
    public class ServiceConfigurationModule : DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<ISigning, Signing>();
            services.AddTransient<ICertificateService, CertificateService>();

            services.AddSingleton<AuthSigningConfig>();

            base.ConfigureServices(services, configuration);
        }
    }
}