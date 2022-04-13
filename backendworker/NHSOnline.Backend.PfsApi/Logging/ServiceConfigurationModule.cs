using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.Logger.Logging;

namespace NHSOnline.Backend.PfsApi.Logging
{
    public class ServiceConfigurationModule : Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<ILoggingService, LoggingService>();

            base.ConfigureServices(services, configuration);
        }
    }
}