using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.Repository;

namespace NHSOnline.Backend.Auditing
{
    public class ServiceConfigurationModule : Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient(AuditorFactory.BuildAuditor);
            services.AddSingleton<IAuditSink, DbAuditorSink>();
            services.AddSingleton<AuditorFactory>();
            services.RegisterRepository<AuditRecord, RepositoryDbAuditSinkConfiguration>();
        }
    }
}