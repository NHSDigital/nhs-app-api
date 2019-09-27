using System.Security.Authentication;
using MongoDB.Driver;

namespace NHSOnline.Backend.Support.Repository
{
    public class ApiMongoClient : MongoClient
    {
        public ApiMongoClient(IMongoConfiguration mongoConfiguration)
            : base(BuildSettings(mongoConfiguration))
        {
        }

        private static MongoClientSettings BuildSettings(IMongoConfiguration mongoConfiguration)
        {
            var username = mongoConfiguration.Username;
            var settings = new MongoClientSettings
            {
                Server = new MongoServerAddress(mongoConfiguration.Host, mongoConfiguration.Port)
            };

            if (string.IsNullOrEmpty(username)) return settings;
            
            settings.UseSsl = true;
            settings.SslSettings = new SslSettings { EnabledSslProtocols = SslProtocols.Tls12 };

            var identity = new MongoInternalIdentity(mongoConfiguration.DatabaseName, username);
            var evidence = new PasswordEvidence(mongoConfiguration.Password);
            settings.Credential = new MongoCredential("SCRAM-SHA-1", identity, evidence);

            return settings;
        }
    }
}