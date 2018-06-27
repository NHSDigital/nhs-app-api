using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.Worker.Support.Date
{
    public class Module : Support.DependencyInjection.Module
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
