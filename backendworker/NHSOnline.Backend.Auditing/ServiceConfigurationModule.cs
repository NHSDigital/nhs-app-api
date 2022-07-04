using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Repository;
using NHSOnline.Backend.Support;
using System;
using Azure.Messaging.EventHubs.Producer;

namespace NHSOnline.Backend.Auditing
{
    public class ServiceConfigurationModule : Support.DependencyInjection.ServiceConfigurationModule
    {
        private readonly ILogger _logger;

        public ServiceConfigurationModule(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ServiceConfigurationModule>();
        }

        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient(AuditorFactory.BuildAuditor);
            services.AddSingleton<AuditorFactory>();

            var auditSinkType = GetAuditSinkType(configuration);
            switch (auditSinkType)
            {
                case AuditSinkType.Console:
                    services.AddSingleton<IAuditSink>(_ => new StreamAuditSink(Console.Out));
                    break;
                case AuditSinkType.CosmosDb:
                    services.AddSingleton<IAuditSink, DbAuditorSink>();
                    services.RegisterRepository<AuditRecord, RepositoryDbAuditSinkConfiguration>(configuration);
                    break;
                case AuditSinkType.EventHub:
                    var eventHubAuditSinkConfiguration = GetEventHubAuditSinkConfiguration(configuration);
                    services.AddSingleton( _ =>
                        new EventHubProducerClient(eventHubAuditSinkConfiguration.ConnectionString,
                            eventHubAuditSinkConfiguration.EventHubName));
                    services.AddSingleton<IAuditSink, EventHubAuditorSink>();
                    break;
            }
        }

        private static AuditSinkType GetAuditSinkType(IConfiguration configuration)
        {
            var auditTypeString = configuration["AUDIT_SINK_TYPE"] ?? AuditSinkType.Console.ToString();
            var parseSuccess = Enum.TryParse<AuditSinkType>(auditTypeString, true, out var auditSinkType);
            return parseSuccess ? auditSinkType : AuditSinkType.Console;
        }

        private EventHubAuditSinkConfiguration GetEventHubAuditSinkConfiguration(IConfiguration configuration)
        {
            var connectionString = configuration.GetOrThrow("AUDIT_EVENT_HUB_CONNECTION_STRING", _logger);
            var eventHubName = configuration.GetOrThrow("AUDIT_EVENT_HUB_NAME", _logger);
            return new EventHubAuditSinkConfiguration(connectionString, eventHubName);
        }
    }
}