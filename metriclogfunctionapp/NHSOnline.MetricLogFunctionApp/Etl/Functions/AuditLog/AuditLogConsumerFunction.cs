using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.Appointment.Book;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.Appointment.Cancel;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.MedicalRecord.MedicalRecordView;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.RegistrationAndLogin.Consent;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.RegistrationAndLogin.Login;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.RegistrationAndLogin.WebIntegrationReferrals;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.Wayfinder.SecondaryCareSummary;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.Notification.Toggle;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.Notification.InitialPrompt;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.OrganDonationRegistration.Get;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.OrganDonationRegistration.Create;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.OrganDonationRegistration.Withdraw;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.RepeatPrescription;
using NHSOnline.MetricLogFunctionApp.Etl.Logging;

namespace NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog
{
    public class AuditLogConsumerFunction
    {
        private readonly IAuditLogEtl<ConsentMetric> _consentEtl;
        private readonly IAuditLogEtl<LoginMetric> _loginEtl;
        private readonly IAuditLogEtl<WebIntegrationReferralsMetric> _webIntegrationReferralEtl;
        private readonly IAuditLogEtl<SecondaryCareSummaryMetric> _secondaryCareSummaryEtl;
        private readonly IAuditLogEtl<MedicalRecordViewMetric> _medicalRecordViewEtl;
        private readonly IAuditLogEtl<NotificationToggleMetric> _notificationToggleEtl;
        private readonly IAuditLogEtl<InitialPromptMetric> _initialPromptEtl;
        private readonly IAuditLogEtl<AppointmentCancelMetric> _appointmentCancelEtl;
        private readonly IAuditLogEtl<AppointmentBookMetric> _appointmentBookEtl;
        private readonly IAuditLogEtl<OrganDonationRegistrationGetMetric> _organDonationRegistrationGetEtl;
        private readonly IAuditLogEtl<OrganDonationRegistrationCreateMetric> _organDonationRegistrationCreateEtl;
        private readonly IAuditLogEtl<OrganDonationRegistrationWithdrawMetric> _organDonationRegistrationWithdrawEtl;
        private readonly IAuditLogEtl<RepeatPrescriptionMetric> _repeatPrescriptionEtl;
        private readonly IEtlLogger<AuditLogConsumerFunction> _logger;
        private readonly ILogger _queueLogger;

        public AuditLogConsumerFunction(
            IAuditLogEtl<ConsentMetric> consentEtl,
            IAuditLogEtl<LoginMetric> loginEtl,
            IAuditLogEtl<WebIntegrationReferralsMetric> webIntegrationReferralEtl,
            IAuditLogEtl<SecondaryCareSummaryMetric> secondaryCareSummaryEtl,
            IAuditLogEtl<MedicalRecordViewMetric> medicalRecordViewEtl,
            IAuditLogEtl<NotificationToggleMetric> notificationToggleEtl,
            IAuditLogEtl<InitialPromptMetric> initialPromptEtl,
            IAuditLogEtl<AppointmentCancelMetric> appointmentCancelEtl,
            IAuditLogEtl<AppointmentBookMetric> appointmentBookEtl,
            IAuditLogEtl<OrganDonationRegistrationGetMetric> organDonationRegistrationGetEtl,
            IAuditLogEtl<OrganDonationRegistrationCreateMetric> organDonationRegistrationCreateEtl,
            IAuditLogEtl<OrganDonationRegistrationWithdrawMetric> organDonationRegistrationWithdrawEtl,
            IAuditLogEtl<RepeatPrescriptionMetric> repeatPrescriptionEtl,
            IEtlLogger<AuditLogConsumerFunction> logger,
            ILogger<AuditLogConsumerFunction> queueLogger)
        {
            _consentEtl = consentEtl;
            _loginEtl = loginEtl;
            _webIntegrationReferralEtl = webIntegrationReferralEtl;
            _secondaryCareSummaryEtl = secondaryCareSummaryEtl;
            _medicalRecordViewEtl = medicalRecordViewEtl;
            _notificationToggleEtl = notificationToggleEtl;
            _initialPromptEtl = initialPromptEtl;
            _appointmentCancelEtl = appointmentCancelEtl;
            _appointmentBookEtl = appointmentBookEtl;
            _organDonationRegistrationGetEtl = organDonationRegistrationGetEtl;
            _organDonationRegistrationCreateEtl = organDonationRegistrationCreateEtl;
            _organDonationRegistrationWithdrawEtl = organDonationRegistrationWithdrawEtl;
            _repeatPrescriptionEtl = repeatPrescriptionEtl;
            _logger = logger;
            _queueLogger = queueLogger;
        }

        [FunctionName("AuditLog_Etl_EventHub")]
        [ExponentialBackoffRetry(4, "00:00:15", "00:02:00")]
        public async Task AuditLog_Etl_EventHub(
            [EventHubTrigger(
                "%AuditEventHubName%",
                Connection = "AuditEventHubConnectionDataReceiver",
                ConsumerGroup = "%AuditEventHubAnalyticsConsumerGroup%")]
            AuditRecord[] events, ExecutionContext context)
        {
            await Execute(events, context);
        }

        [FunctionName("AuditLog_Etl_Http")]
        [ExponentialBackoffRetry(2, "00:00:01", "00:00:01")]
        public async Task<HttpResponseMessage> AuditLog_Etl_Http(
            [HttpTrigger(AuthorizationLevel.Function, "post")]
            HttpRequestMessage req, ExecutionContext context)
        {
            var body = await req.Content.ReadAsStringAsync();
            var events = JsonConvert.DeserializeObject<AuditRecord[]>(body);

            await Execute(events, context);

            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        private async Task Execute(AuditRecord[] events, ExecutionContext context)
        {
            try
            {
                _logger.StartedTriggered("AuditLogEtl", "");

                await _consentEtl.ExecuteDependentEvent(_queueLogger,events);
                await _loginEtl.ExecuteDependentEvent(_queueLogger,events);
                await _secondaryCareSummaryEtl.Execute(events);
                await _notificationToggleEtl.Execute(events);
                await _initialPromptEtl.Execute(events);
                await _appointmentCancelEtl.Execute(events);
                await _webIntegrationReferralEtl.Execute(events);
                await _medicalRecordViewEtl.Execute(events);
                await _appointmentBookEtl.Execute(events);
                await _organDonationRegistrationGetEtl.Execute(events);
                await _organDonationRegistrationCreateEtl.Execute(events);
                await _organDonationRegistrationWithdrawEtl.Execute(events);
                await _repeatPrescriptionEtl.Execute(events);
            }
            catch (Exception)
            {
                var retryCount = context.RetryContext.RetryCount;
                var maxRetries = context.RetryContext.MaxRetryCount;

                if (retryCount >= maxRetries)
                {
                    _logger.Failed($"Encountered error while processing events. Max retries reached: {retryCount}");
                    _logger.Information("To replay these events reset the checkpoint to the first entry in range.");
                }
                else
                {
                    _logger.Failed($"Encountered error while processing events. Current retry count: {retryCount}.  Max retry count: {maxRetries}. Processing will be retried");
                }

                throw;
            }
        }
    }
}
