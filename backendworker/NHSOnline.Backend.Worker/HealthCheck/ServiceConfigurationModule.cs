using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.Worker.HealthCheck
{
    public class ServiceConfigurationModule: Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IHealthCheckService, HealthCheckService>();
            services.AddTransient<IRedisHealthCheckFactory, RedisHealthCheckFactory>();
            
            base.ConfigureServices(services, configuration);
        }
    }
}