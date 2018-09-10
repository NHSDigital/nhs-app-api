using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.Worker.CitizenId
{
    public class ServiceConfigurationModule : Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient<CitizenIdHttpClient>()
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler().ConfigureForwardProxy(configuration));
            
            services.AddScoped<ICitizenIdService, CitizenIdService>();
            services.AddSingleton<ICitizenIdClient, CitizenIdClient>();
            services.AddSingleton<ICitizenIdConfig, CitizenIdConfig>();

            base.ConfigureServices(services, configuration);
        }
    }
}
