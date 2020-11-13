using CorrelationId.HttpClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.AspNet.HealthChecks;
using NHSOnline.Backend.Support.Http;

namespace NHSOnline.Backend.PfsApi.UserInfo
{
    public class ServiceConfigurationModule : Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IUserInfoApiConfig, UserInfoApiConfig>();
            services.AddSingleton<IUserInfoClient, UserInfoClient>();
            services.AddSingleton<IUserInfoService, UserInfoService>();

            services.AddTransient<UserInfoHttpRequestIdentifier>();

            services.AddHttpClient<UserInfoHttpClient>()
                .AddHttpMessageHandler<HttpTimeoutHandler<UserInfoHttpRequestIdentifier>>()
                .AddHttpMessageHandler<HttpRequestIdentificationHandler<UserInfoHttpRequestIdentifier>>()
                .AddCorrelationIdForwarding();

            services.AddNhsAppHealthCheck<UserInfoHttpClient>("UserInfo");

            base.ConfigureServices(services, configuration);
        }
    }
}