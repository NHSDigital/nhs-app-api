using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.ServiceJourneyRules.Common;
using NHSOnline.Backend.Support.Http;

namespace NHSOnline.Backend.CidApi.ServiceJourneyRules
{
    public class ServiceConfigurationModule: NHSOnline.Backend.Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IServiceJourneyRulesClient, ServiceJourneyRulesClient>();
            services.AddSingleton<IServiceJourneyRulesConfig, ServiceJourneyRulesConfig>();
            
            services.AddTransient<ServiceJourneyRulesHttpRequestIdentifier>();
            
            services.AddHttpClient<ServiceJourneyRulesHttpClient>()
                .AddHttpMessageHandler<HttpTimeoutHandler<ServiceJourneyRulesHttpRequestIdentifier>>()
                .AddHttpMessageHandler<HttpRequestIdentificationHandler<ServiceJourneyRulesHttpRequestIdentifier>>();
            
            base.ConfigureServices(services, configuration);
        }
    }
}