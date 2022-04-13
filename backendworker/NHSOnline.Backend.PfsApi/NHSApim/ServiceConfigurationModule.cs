using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.Auth.APIM;
using NHSOnline.Backend.PfsApi.SecondaryCare;
using NHSOnline.Backend.Support.Http;

namespace NHSOnline.Backend.PfsApi.NHSApim
{
    public class ServiceConfigurationModule : Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<INhsApimConfig, NhsApimConfig>();

            services.AddSingleton<INhsApimClient, NhsApimClient>();

            services.AddSingleton<IApimJwtHelper, ApimJwtHelper>();

            services.AddTransient<NhsApimHttpClientHandler>();
            services.AddTransient<NhsApimHttpRequestIdentifier>();
            services.AddHttpClient<NhsApimHttpClient>()
                .ConfigurePrimaryHttpMessageHandler<NhsApimHttpClientHandler>()
                .AddHttpMessageHandler<HttpTimeoutHandler<NhsApimHttpRequestIdentifier>>()
                .AddHttpMessageHandler<HttpRequestIdentificationHandler<NhsApimHttpRequestIdentifier>>();

            base.ConfigureServices(services, configuration);
        }
    }
}