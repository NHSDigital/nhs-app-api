using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.Worker.CitizenId
{
    public class ServiceConfigurationModule : Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient<CitizenIdHttpClient>();
            
            services.AddScoped<ICitizenIdService, CitizenIdService>();
            services.AddSingleton<ICitizenIdClient, CitizenIdClient>();
            services.AddSingleton<ICitizenIdConfig, CitizenIdConfig>();
            services.AddSingleton<ITokenValidationParameterBuilder, TokenValidationParameterBuilder>();
            services.AddSingleton<IJwtTokenService<UserProfile>, IdTokenService>();
            services.AddSingleton<ICitizenIdSigningKeysService, CitizenIdSigningKeysService>();

            base.ConfigureServices(services, configuration);
        }
    }
}
