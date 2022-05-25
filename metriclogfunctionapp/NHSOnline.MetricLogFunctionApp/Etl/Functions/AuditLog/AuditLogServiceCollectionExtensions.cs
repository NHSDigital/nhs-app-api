using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NHSOnline.MetricLogFunctionApp.Compute.QueueRequests;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.MedicalRecord.MedicalRecordView;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.RegistrationAndLogin.Consent;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.RegistrationAndLogin.Login;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.RegistrationAndLogin.WebIntegrationReferrals;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.Wayfinder.SecondaryCareSummary;
using NHSOnline.MetricLogFunctionApp.Resilience;

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
            serviceCollection.AddTransient<IRequestQueueOrchestrator<AuditReportRequest>,RequestQueueOrchestrator<AuditReportRequest>>();
            serviceCollection.AddTransient<IAuditLogParser<WebIntegrationReferralsMetric>, WebIntegrationReferralsEventParser>();

            serviceCollection.AddTransient<IAuditLogEtl<SecondaryCareSummaryMetric>, SecondaryCareSummaryMetricEtl>();
            serviceCollection
                .AddTransient<IAuditLogParser<SecondaryCareSummaryMetric>, SecondaryCareSummaryEventParser>();

            serviceCollection.AddTransient<IAuditLogEtl<MedicalRecordViewMetric>, MedicalRecordViewMetricEtl>();
            serviceCollection.AddTransient<IAuditLogParser<MedicalRecordViewMetric>, MedicalRecordViewEventParser>();

            serviceCollection.AddTransient(typeof(IRequestQueueOrchestrator<>),typeof(RequestQueueOrchestrator<>));
            serviceCollection.AddTransient(typeof(RequestQueueOrchestrator<>));
        }
    }
}
