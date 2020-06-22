using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using NHSOnline.Backend.Support;
using ServiceConfigurationModule = NHSOnline.Backend.Support.DependencyInjection.ServiceConfigurationModule;

namespace NHSOnline.Backend.GpSystems
{
    public class MongoServiceConfigurationModule : ServiceConfigurationModule
    {
        private readonly ILoggerFactory _loggerFactory;

        public MongoServiceConfigurationModule(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            var logger = _loggerFactory.CreateLogger<MongoServiceConfigurationModule>();

            var connectionString = configuration.GetOrThrow("AUDIT_MONGO_CONNECTION_STRING", logger);

            var mongoUrl = new MongoUrl(connectionString);
            var mongoClientSettings = MongoClientSettings.FromUrl(mongoUrl);
            var mongoClient= new MongoClient(mongoClientSettings);

            services.AddSingleton<IMongoClient>(mongoClient);

            base.ConfigureServices(services, configuration);
        }
    }
}
