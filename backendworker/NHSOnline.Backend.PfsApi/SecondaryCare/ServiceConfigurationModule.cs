using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.PfsApi.SecondaryCare.Mappers;
using NHSOnline.Backend.Support.Http;

namespace NHSOnline.Backend.PfsApi.SecondaryCare
{
    public class ServiceConfigurationModule : Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<ISecondaryCareConfig, SecondaryCareConfig>();

            services.AddSingleton<ISecondaryCareService, SecondaryCareService>();
            services.AddSingleton<SecondaryCareSummaryService>();

            services.AddSingleton<ISecondaryCareSummaryMapper, SecondaryCareSummaryMapper>();

            services.AddSingleton<ISecondaryCareClient, SecondaryCareClient>();

            services.AddTransient<SecondaryCareHttpClientHandler>();
            services.AddTransient<SecondaryCareHttpRequestIdentifier>();
            services.AddHttpClient<SecondaryCareHttpClient>()
                .ConfigurePrimaryHttpMessageHandler<SecondaryCareHttpClientHandler>()
                .AddHttpMessageHandler<HttpTimeoutHandler<SecondaryCareHttpRequestIdentifier>>()
                .AddHttpMessageHandler<HttpRequestIdentificationHandler<SecondaryCareHttpRequestIdentifier>>();

            base.ConfigureServices(services, configuration);
        }
    }
}