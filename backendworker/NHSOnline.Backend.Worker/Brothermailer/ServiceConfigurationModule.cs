using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.Worker.Brothermailer
{
    public class ServiceConfigurationModule: NHSOnline.Backend.Worker.Support.DependencyInjection.ServiceConfigurationModule
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