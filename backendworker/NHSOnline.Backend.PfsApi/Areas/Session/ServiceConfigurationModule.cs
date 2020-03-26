using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.GpSystems.SessionManager;

namespace NHSOnline.Backend.PfsApi.Areas.Session
{
    public class ServiceConfigurationModule : Backend.Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<ISessionMapper, SessionMapper>();
            base.ConfigureServices(services, configuration);
        }
    }
}
