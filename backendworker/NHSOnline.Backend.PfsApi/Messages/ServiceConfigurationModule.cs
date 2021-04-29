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
            services.AddSingleton<IMessagesApiConfig, MessagesApiConfig>();
            services.AddSingleton<IMessagesClient, MessagesClient>();
            services.AddSingleton<IMessagesService, MessagesService>();
            services.AddSingleton<IMessagesServiceConfig, MessagesServiceConfig>();

            services.AddTransient<MessagesHttpRequestIdentifier>();

            services
                .AddHttpClient<MessagesHttpClient>()
                .AddHttpMessageHandler<HttpTimeoutHandler<MessagesHttpRequestIdentifier>>()
                .AddHttpMessageHandler<HttpRequestIdentificationHandler<MessagesHttpRequestIdentifier>>()
                .AddCorrelationIdForwarding();

            services.AddNhsAppClientHealthCheck<MessagesHttpClient>("Messages", NhsAppHealthCheckTags.Readiness);

            base.ConfigureServices(services, configuration);
        }
    }
}
