using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.Worker.HealthCheck.Redis;

namespace NHSOnline.Backend.Worker.HealthCheck
{
    public class Module: Support.DependencyInjection.Module
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IHealthCheckService, HealthCheckService>();
            services.AddTransient<IRedisHealthCheckFactory, RedisHealthCheckFactory>();
            
            base.ConfigureServices(services, configuration);
        }
    }
}