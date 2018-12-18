using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.Worker.Support.Http;

namespace NHSOnline.Backend.Worker.CitizenId
{
    public class ServiceConfigurationModule : Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<CitizenIdHttpRequestIdentifier>();
            
            services.AddHttpClient<CitizenIdHttpClient>()
                .AddHttpMessageHandler<HttpTimeoutHandler<CitizenIdHttpRequestIdentifier>>()
                .AddHttpMessageHandler<HttpRequestIdentificationHandler<CitizenIdHttpRequestIdentifier>>();
            
            services.AddScoped<ICitizenIdService, CitizenIdService>();
            services.AddSingleton<ICitizenIdClient, CitizenIdClient>();
            services.AddSingleton<ICitizenIdConfig, CitizenIdConfig>();
            services.AddSingleton<ITokenValidationParameterBuilder, TokenValidationParameterBuilder>();
            services.AddSingleton<IJwtTokenService<UserProfile>, IdTokenService>();
            services.AddSingleton<ICitizenIdSigningKeysService, CitizenIdSigningKeysService>();
            services.AddScoped<ICitizenIdSessionService, CitizenIdSessionService>();

            base.ConfigureServices(services, configuration);
        }
    }
}
