using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NHSOnline.MetricLogFunctionApp.Compute.QueueRequests;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.Appointment.Book;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.Appointment.Cancel;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.BiometricsToggle;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.MedicalRecord.MedicalRecordView;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.MedicalRecord.SectionView;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.NominatedPharmacy.Create;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.NominatedPharmacy.Update;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.RegistrationAndLogin.Consent;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.RegistrationAndLogin.Login;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.RegistrationAndLogin.WebIntegrationReferrals;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.Wayfinder.SecondaryCareSummary;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.Notification.Toggle;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.Notification.InitialPrompt;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.OrganDonationRegistration.Get;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.OrganDonationRegistration.Create;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.OrganDonationRegistration.Withdraw;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.OrganDonationRegistration.Update;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.RepeatPrescription;
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
            serviceCollection.AddTransient<IRequestQueueOrchestrator<AuditReportRequest>, RequestQueueOrchestrator<AuditReportRequest>>();
            serviceCollection.AddTransient<IAuditLogParser<WebIntegrationReferralsMetric>, WebIntegrationReferralsEventParser>();

            serviceCollection.AddTransient<IAuditLogEtl<SecondaryCareSummaryMetric>, SecondaryCareSummaryMetricEtl>();
            serviceCollection.AddTransient<IAuditLogParser<SecondaryCareSummaryMetric>, SecondaryCareSummaryEventParser>();

            serviceCollection.AddTransient<IAuditLogEtl<MedicalRecordViewMetric>, MedicalRecordViewMetricEtl>();
            serviceCollection.AddTransient<IAuditLogParser<MedicalRecordViewMetric>, MedicalRecordViewEventParser>();

            serviceCollection.AddTransient<IAuditLogEtl<NotificationToggleMetric>, NotificationToggleMetricEtl>();
            serviceCollection.AddTransient<IAuditLogParser<NotificationToggleMetric>, NotificationToggleEventParser>();

            serviceCollection.AddTransient<IAuditLogEtl<AppointmentCancelMetric>, AppointmentCancelMetricEtl>();
            serviceCollection.AddTransient<IAuditLogParser<AppointmentCancelMetric>, AppointmentCancelEventParser>();

            serviceCollection.AddTransient<IAuditLogEtl<AppointmentBookMetric>, AppointmentBookMetricEtl>();
            serviceCollection.AddTransient<IAuditLogParser<AppointmentBookMetric>, AppointmentBookEventParser>();

            serviceCollection.AddTransient<IAuditLogEtl<OrganDonationRegistrationGetMetric>, OrganDonationRegistrationGetMetricEtl>();
            serviceCollection.AddTransient<IAuditLogParser<OrganDonationRegistrationGetMetric>, OrganDonationRegistrationGetEventParser>();

            serviceCollection.AddTransient<IAuditLogEtl<OrganDonationRegistrationCreateMetric>, OrganDonationRegistrationCreateMetricEtl>();
            serviceCollection.AddTransient<IAuditLogParser<OrganDonationRegistrationCreateMetric>, OrganDonationRegistrationCreateEventParser>();

            serviceCollection.AddTransient<IAuditLogEtl<OrganDonationRegistrationWithdrawMetric>, OrganDonationRegistrationWithdrawMetricEtl>();
            serviceCollection.AddTransient<IAuditLogParser<OrganDonationRegistrationWithdrawMetric>, OrganDonationRegistrationWithdrawEventParser>();

            serviceCollection.AddTransient<IAuditLogEtl<RepeatPrescriptionMetric>, RepeatPrescriptionMetricEtl>();
            serviceCollection.AddTransient<IAuditLogParser<RepeatPrescriptionMetric>, RepeatPrescriptionEventParser>();

            serviceCollection.AddTransient<IAuditLogEtl<OrganDonationRegistrationUpdateMetric>, OrganDonationRegistrationUpdateMetricEtl>();
            serviceCollection.AddTransient<IAuditLogParser<OrganDonationRegistrationUpdateMetric>, OrganDonationRegistrationUpdateEventParser>();

            serviceCollection.AddTransient<IAuditLogEtl<NominatedPharmacyCreateMetric>, NominatedPharmacyCreateMetricEtl>();
            serviceCollection.AddTransient<IAuditLogParser<NominatedPharmacyCreateMetric>, NominatedPharmacyCreateEventParser >();
            
            serviceCollection.AddTransient<IAuditLogEtl<NominatedPharmacyUpdateMetric>, NominatedPharmacyUpdateMetricEtl>();
            serviceCollection.AddTransient<IAuditLogParser<NominatedPharmacyUpdateMetric>, NominatedPharmacyUpdateEventParser >();

            serviceCollection.AddTransient(typeof(IRequestQueueOrchestrator<>), typeof(RequestQueueOrchestrator<>));
            serviceCollection.AddTransient(typeof(RequestQueueOrchestrator<>));

            serviceCollection.AddTransient<IAuditLogEtl<InitialPromptMetric>, InitialPromptMetricEtl>();
            serviceCollection.AddTransient<IAuditLogParser<InitialPromptMetric>, InitialPromptEventParser>();

            serviceCollection.AddTransient<IAuditLogEtl<BiometricsToggleMetric>, BiometricsToggleMetricEtl>();
            serviceCollection.AddTransient<IAuditLogParser<BiometricsToggleMetric>, BiometricsToggleEventParser>();

            serviceCollection.AddTransient<IAuditLogEtl<MedicalRecordSectionViewMetric>, MedicalRecordSectionViewMetricEtl>();
            serviceCollection.AddTransient<IAuditLogParser<MedicalRecordSectionViewMetric>, MedicalRecordSectionViewEventParser>();
        }
    }
}
