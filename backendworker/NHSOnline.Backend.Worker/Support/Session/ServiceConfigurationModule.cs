using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace NHSOnline.Backend.Worker.Support.Session
{
    public class ServiceConfigurationModule : Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<ISessionCacheService, RedisSessionCacheService>();
            services.AddSingleton(x => new NamedConnectionMultiplexer(
                ConnectionMultiplexerName.Session,
                ConnectionMultiplexer.Connect(configuration["REDIS_SESSION_CONFIG"])));

            base.ConfigureServices(services, configuration);
        }
    }
}