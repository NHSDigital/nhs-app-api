using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.Session
{
    public sealed class ServiceConfigurationModule : Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<UserSessionService>();
            services.AddTransient<IUserSessionService>(sp => sp.GetRequiredService<UserSessionService>());
            services.AddTransient<IUserSessionManager, UserSessionManager>();
            services.AddTransient<UserSessionCreator>();
            services.AddTransient<P9UserSessionCreator>();
        }
    }
}