using CorrelationId.HttpClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.AspNet.HealthChecks;
using NHSOnline.Backend.Support.Http;

namespace NHSOnline.Backend.PfsApi.Messages
{
    public class ServiceConfigurationModule : Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IIntroMessagesService, IntroMessagesService>();
            services.AddSingleton<IIntroMessagesServiceConfig, IntroMessagesServiceConfig>();
            base.ConfigureServices(services, configuration);
        }
    }
}
