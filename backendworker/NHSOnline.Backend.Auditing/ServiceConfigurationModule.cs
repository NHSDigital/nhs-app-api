using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.Repository;
using System;

namespace NHSOnline.Backend.Auditing
{
    public class ServiceConfigurationModule : Support.DependencyInjection.ServiceConfigurationModule
    {
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
                    // TODO: Event hub audit sink to go in here
                    break;
            }
        }

        private static AuditSinkType GetAuditSinkType(IConfiguration configuration)
        {
            var auditTypeString = configuration["AUDIT_SINK_TYPE"] ?? AuditSinkType.Console.ToString();
            var parseSuccess = Enum.TryParse<AuditSinkType>(auditTypeString, true, out var auditSinkType);
            return parseSuccess ? auditSinkType : AuditSinkType.Console;
        }
    }
}