using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.Repository;
using NHSOnline.Backend.Support.Settings;

namespace NHSOnline.Backend.Auditing
{
    public class ServiceConfigurationModule : Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient(AuditorFactory.BuildAuditor);
            services.AddSingleton<AuditorFactory>();

            var sinkType = configuration["AUDIT_SINK_TYPE"];

            if (sinkType == null)
            {
                throw new ConfigurationNotFoundException("AUDIT_SINK_TYPE");
            }

            switch (sinkType.ToUpperInvariant())
            {
                case "FILE":
                    var filePath = configuration["AUDIT_FILE"] ?? "audit.txt";
                    services.AddSingleton<IAuditSink>(_ => new StreamAuditSink(new FileStream(filePath, FileMode.Append)));
                    break;
                case "MONGO":
                    services.AddSingleton(typeof(IApiMongoClient<>), typeof(ApiMongoClient<>));
                    services.AddSingleton<MongoDbAuditSinkConfiguration>();
                    services.AddSingleton<IAuditSink, DbAuditorSink>();
                    services.AddSingleton<IRepository<AuditRecord>, MongoRepositoryBase<MongoDbAuditSinkConfiguration, AuditRecord>>();
                    break;
                default:
                    services.AddSingleton<IAzureCosmosDbAuditorSinkConfig, AzureCosmosDbAuditorSinkConfig>();
                    services.AddSingleton<IAuditSink, AzureCosmosDbAuditorSink>();
                    break;
            }
        }
    }
}