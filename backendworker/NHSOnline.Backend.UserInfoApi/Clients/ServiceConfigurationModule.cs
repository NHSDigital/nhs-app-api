using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.Support.Http;

namespace NHSOnline.Backend.UserInfoApi.Clients
{
    public class ServiceConfigurationModule: Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<QualtricsHttpRequestIdentifier>();

            services.AddHttpClient<QualtricsHttpClient>()
                .AddHttpMessageHandler<HttpTimeoutHandler<QualtricsHttpRequestIdentifier>>()
                .AddHttpMessageHandler<HttpRequestIdentificationHandler<QualtricsHttpRequestIdentifier>>();

            services.AddSingleton<IQualtricsConfig, QualtricsConfig>();

            services.AddSingleton<IUserResearchClient, QualtricsClient>();

            base.ConfigureServices(services, configuration);
        }
    }
}