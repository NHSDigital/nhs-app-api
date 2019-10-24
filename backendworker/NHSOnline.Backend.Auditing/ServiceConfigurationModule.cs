using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.Support.Repository;
using NHSOnline.Backend.Support.Settings;

namespace NHSOnline.Backend.Auditing
{
    public class ServiceConfigurationModule : Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient(AuditorFactory.BuildAuditor);
            services.AddSingleton<IAuditorFactory, AuditorFactory>();

            var sinkType = configuration["AUDIT_SINK_TYPE"];

            if (sinkType == null)
            {
                throw new ConfigurationNotFoundException("AUDIT_SINK_TYPE");
            }
            
            switch (sinkType.ToUpperInvariant())
            {
                case "FILE":
                    var filePath = configuration["AUDIT_FILE"] ?? "audit.txt";
                    services.AddSingleton<IAuditSink>(
                        new StreamAuditSink(new System.IO.FileStream(filePath, System.IO.FileMode.Append)));
                    break;
                case "MONGO":
                    services.AddSingleton(typeof(IApiMongoClient<>), typeof(ApiMongoClient<>));
                    services.AddSingleton<MongoDbAuditSinkConfiguration>();
                    services.AddSingleton<IAuditSink, MongoDbAuditorSink>();
                    break;
                default:
                    services.AddSingleton<IAzureCosmosDbAuditorSinkConfig, AzureCosmosDbAuditorSinkConfig>();
                    services.AddSingleton<IAuditSink, AzureCosmosDbAuditorSink>();
                    break;
            }
        }
    }
}