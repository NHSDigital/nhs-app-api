using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.PfsApi.Configs;
using NHSOnline.Backend.Support.Http;

namespace NHSOnline.Backend.PfsApi.UserInfo
{
    public class ServiceConfigurationModule : Backend.Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IApiConfig, ApiConfig>();
            services.AddSingleton<IUserInfoClient, UserInfoClient>();
            services.AddSingleton<IUserInfoService, UserInfoService>();

            services.AddTransient<UserInfoHttpRequestIdentifier>();
            
            services.AddHttpClient<UserInfoHttpClient>()
                .AddHttpMessageHandler<HttpTimeoutHandler<UserInfoHttpRequestIdentifier>>()
                .AddHttpMessageHandler<HttpRequestIdentificationHandler<UserInfoHttpRequestIdentifier>>();

            base.ConfigureServices(services, configuration);
        }
    }
}