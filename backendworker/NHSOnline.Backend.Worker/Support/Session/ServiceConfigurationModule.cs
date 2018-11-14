using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using NHSOnline.Backend.Worker.Settings;
using StackExchange.Redis;

namespace NHSOnline.Backend.Worker.Support.Session
{
    public class ServiceConfigurationModule : Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            var databaseType = configuration["SESSION_DATABASE_TYPE"];

            if (databaseType == null)
            {
                throw new ConfigurationNotFoundException("SESSION_DATABASE_TYPE");
            }

            switch (databaseType.ToUpperInvariant())
            {
                case "MONGO":
                    services.AddSingleton<IMongoSessionCacheServiceConfig, MongoSessionCacheServiceConfig>();
                    services.AddSingleton<ISessionCacheService, MongoSessionCacheService>();
                    break;
                
                default:
                    services.AddSingleton<ISessionCacheService, RedisSessionCacheService>();
                    break;
            }
            //TODO: the implementation of NHSO-2856 will allow this Singleton to be moved inside the switch above
            services.AddSingleton(x => new NamedConnectionMultiplexer(
                ConnectionMultiplexerName.Session,
                ConnectionMultiplexer.Connect(configuration["REDIS_SESSION_CONFIG"])));
            
            base.ConfigureServices(services, configuration);
        }
    }
}