using System.Security.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using ServiceConfigurationModule = NHSOnline.Backend.Worker.Support.DependencyInjection.ServiceConfigurationModule;

namespace NHSOnline.Backend.Worker
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

            var host = configuration.GetOrThrow("SESSION_MONGO_DATABASE_HOST", logger);
            var port = configuration.GetIntOrThrow("SESSION_MONGO_DATABASE_PORT", logger);
            var database = configuration.GetOrThrow("SESSION_MONGO_DATABASE_NAME", logger);
            var username = configuration.GetOrNull("SESSION_MONGO_DATABASE_USERNAME");
            var password = configuration.GetOrNull("SESSION_MONGO_DATABASE_PASSWORD");

            var settings = new MongoClientSettings
            {
                Server = new MongoServerAddress(host, port)
            };

            if (!string.IsNullOrEmpty(username))
            {
                settings.UseSsl = true;
                settings.SslSettings = new SslSettings { EnabledSslProtocols = SslProtocols.Tls12 };

                var identity = new MongoInternalIdentity(database, username);
                var evidence = new PasswordEvidence(password);
                settings.Credential = new MongoCredential("SCRAM-SHA-1", identity, evidence);
            }

            services.AddSingleton<IMongoClient>(new MongoClient(settings));

            base.ConfigureServices(services, configuration);
        }
    }
}
