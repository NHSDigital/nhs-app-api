using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NHOnline.Backend.LoggerApi.Logging
{
    public class ServiceConfigurationModule : NHSOnline.Backend.Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<ILoggingService, LoggingService>();

            base.ConfigureServices(services, configuration);
        }

    }
}