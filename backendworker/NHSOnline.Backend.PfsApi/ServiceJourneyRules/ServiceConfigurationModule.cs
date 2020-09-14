using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.HealthChecks;
using NHSOnline.Backend.ServiceJourneyRules.Common;
using NHSOnline.Backend.Support.Http;

namespace NHSOnline.Backend.PfsApi.ServiceJourneyRules
{
    public class ServiceConfigurationModule: Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IServiceJourneyRulesService, ServiceJourneyRulesService>();
            services.AddSingleton<IServiceJourneyRulesClient, ServiceJourneyRulesClient>();
            services.AddSingleton<IServiceJourneyRulesConfig, ServiceJourneyRulesConfig>();
            
            services.AddTransient<ServiceJourneyRulesHttpRequestIdentifier>();
            
            services.AddHttpClient<ServiceJourneyRulesHttpClient>()
                .AddHttpMessageHandler<HttpTimeoutHandler<ServiceJourneyRulesHttpRequestIdentifier>>()
                .AddHttpMessageHandler<HttpRequestIdentificationHandler<ServiceJourneyRulesHttpRequestIdentifier>>();

            services.AddNhsAppHealthCheck<ServiceJourneyRulesHttpClient>("SJR");
            
            base.ConfigureServices(services, configuration);
        }
    }
}