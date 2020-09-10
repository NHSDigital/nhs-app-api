using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.Support.Temporal
{
    public class ServiceConfigurationModule : DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<ITimeZoneInfoProvider, TimeZoneInfoProvider>();
            services.AddSingleton<TimeZoneConverter>();
            services.AddSingleton<ICurrentDateTimeProvider, CurrentDateTimeProvider>();
            services.AddSingleton<IDateTimeOffsetProvider, DateTimeOffsetProvider>();
            services.AddTransient<IMinimumAgeValidator, MinimumAgeValidator>();

            base.ConfigureServices(services, configuration);
        }
    }
}
