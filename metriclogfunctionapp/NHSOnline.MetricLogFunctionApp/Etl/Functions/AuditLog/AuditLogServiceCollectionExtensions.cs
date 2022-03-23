using Microsoft.Extensions.DependencyInjection;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.RegistrationAndLogin.Consent;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.RegistrationAndLogin.Login;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.RegistrationAndLogin.WebIntegrationReferrals;

namespace NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog
{
    internal static class AuditLogServiceCollectionExtensions
    {
        internal static void AddAuditLog(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IAuditLogEtl<ConsentMetric>, ConsentMetricEtl>();
            serviceCollection.AddTransient<IAuditLogParser<ConsentMetric>, ConsentEventParser>();

            serviceCollection.AddTransient<IAuditLogEtl<LoginMetric>, LoginMetricEtl>();
            serviceCollection.AddTransient<IAuditLogParser<LoginMetric>, LoginEventParser>();

            serviceCollection.AddTransient<IAuditLogEtl<WebIntegrationReferralsMetric>, WebIntegrationReferralsMetricEtl>();
            serviceCollection.AddTransient<IAuditLogParser<WebIntegrationReferralsMetric>, WebIntegrationReferralsEventParser>();
        }
    }
}
