using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.PfsApi.Brothermailer
{
    public class ServiceConfigurationModule: NHSOnline.Backend.Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IBrothermailerClient, BrothermailerClient>();
            services.AddTransient<IBrothermailerConfig, BrothermailerConfig>();
            services.AddTransient<IBrothermailerService, BrothermailerService>();
            
            services.AddHttpClient<BrothermailerHttpClient>()
                .ConfigurePrimaryHttpMessageHandler(() => new BrothermailerHttpClientHandler());
            
            base.ConfigureServices(services, configuration);
        }
    }
}