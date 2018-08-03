using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.Worker.Support.Temporal
{
    public class ServiceConfigurationModule : Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<TimeZoneInfoProvider>();
            services.AddSingleton<TimeZoneConverter>();
            services.AddSingleton<IDateTimeOffsetProvider, DateTimeOffsetProvider>();

            base.ConfigureServices(services, configuration);
        }
    }
}
