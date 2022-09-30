using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Queue;

namespace NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Storage
{
    internal sealed class StorageQueuesWrapper
    {
        internal StorageQueuesWrapper(TestLogs logs)
        {
            Logs = logs;
        }

        private TestLogs Logs { get; }

        private CloudQueueClient Client { get; set; }

        internal QueueWrapper AppleDownloads => CreateQueueWrapper("apple-downloads-dev-local");
        internal QueueWrapper AppointmentBooking => CreateQueueWrapper("appointment-booking-dev-local");
        internal QueueWrapper AppointmentCancelling => CreateQueueWrapper("appointment-cancelling-dev-local");
        internal QueueWrapper AppointmentBook => CreateQueueWrapper("appointment-book-dev-local");
        internal QueueWrapper AppointmentCancel => CreateQueueWrapper("appointment-cancel-dev-local");
        internal QueueWrapper ClinicalCommissioningGroup => CreateQueueWrapper("clinical-commissioning-group-dev-local");
        internal QueueWrapper CommsHubCommunication => CreateQueueWrapper("commshub-communication-dev-local");
        internal QueueWrapper Consent => CreateQueueWrapper("consent-dev-local");
        internal QueueWrapper DeviceInfo => CreateQueueWrapper("device-info-dev-local");
        internal QueueWrapper Device => CreateQueueWrapper("device-dev-local");
        internal QueueWrapper GeneralPractice => CreateQueueWrapper("general-practice-dev-local");
        internal QueueWrapper GoogleDownloads => CreateQueueWrapper("google-downloads-dev-local");
        internal QueueWrapper Login => CreateQueueWrapper("login-dev-local");
        internal QueueWrapper LoginPatientIdentifier => CreateQueueWrapper("login-patient-identifier-dev-local");
        internal QueueWrapper MedicalRecordView => CreateQueueWrapper("medical-record-view-dev-local");
        internal QueueWrapper CommsHubMessageRead => CreateQueueWrapper("commshub-message-read-dev-local");
        internal QueueWrapper AppGenMessageCreate => CreateQueueWrapper("app-gen-message-create-dev-local");
        internal QueueWrapper AppGenMessageRead => CreateQueueWrapper("app-gen-message-read-dev-local");
        internal QueueWrapper AppGenMessageLinkClick => CreateQueueWrapper("app-gen-message-linkclick-dev-local");
        internal QueueWrapper NominatedPharmacyCreate => CreateQueueWrapper("nominated-pharmacy-create-dev-local");
        internal QueueWrapper NominatedPharmacyUpdate => CreateQueueWrapper("nominated-pharmacy-update-dev-local");
        internal QueueWrapper OrganDonationRegistrationCreate => CreateQueueWrapper("organ-donation-registration-create-dev-local");
        internal QueueWrapper OrganDonationRegistrationGet => CreateQueueWrapper("organ-donation-registration-get-dev-local");
        internal QueueWrapper OrganDonationRegistrationUpdate => CreateQueueWrapper("organ-donation-registration-update-dev-local");
        internal QueueWrapper OrganDonationRegistrationWithdraw => CreateQueueWrapper("organ-donation-registration-withdraw-dev-local");
        internal QueueWrapper PatientsEnabled => CreateQueueWrapper("patients-enabled-dev-local");
        internal QueueWrapper PatientList => CreateQueueWrapper("patient-list-dev-local");
        internal QueueWrapper PrescriptionOrders => CreateQueueWrapper("prescription-orders-dev-local");
        internal QueueWrapper RefreshMaterializedViews => CreateQueueWrapper("refresh-materialized-views-dev-local");
        internal QueueWrapper SilverIntegrationJumpOff => CreateQueueWrapper("silver-integration-jump-off-dev-local");
        internal QueueWrapper CommsHubTransmission => CreateQueueWrapper("commshub-transmission-dev-local");
        internal QueueWrapper UpliftStarted => CreateQueueWrapper("uplift-started-dev-local");

        internal QueueWrapper AppointmentBookWeeklyTransactionReport => CreateQueueWrapper("appt-book-weekly-transaction-reports-dev-local");
        internal QueueWrapper AppointmentCancelWeeklyTransactionReport => CreateQueueWrapper("appt-cancel-weekly-transaction-reports-dev-local");
        internal QueueWrapper LoginWeeklyTransactionReport => CreateQueueWrapper("login-weekly-transaction-reports-dev-local");
        internal QueueWrapper MyRecordViewWeeklyTransactionReport => CreateQueueWrapper("my-record-view-weekly-transaction-reports-dev-local");
        internal QueueWrapper OrganDonationCreateWeeklyTransactionReport => CreateQueueWrapper("od-create-weekly-transaction-reports-dev-local");
        internal QueueWrapper OrganDonationGetWeeklyTransactionReport => CreateQueueWrapper("od-get-weekly-transaction-reports-dev-local");
        internal QueueWrapper OrganDonationUpdateWeeklyTransactionReport => CreateQueueWrapper("od-update-weekly-transaction-reports-dev-local");
        internal QueueWrapper OrganDonationWithdrawWeeklyTransactionReport => CreateQueueWrapper("od-withdraw-weekly-transaction-reports-dev-local");
        internal QueueWrapper PrescriptionsOrderWeeklyTransactionReport => CreateQueueWrapper("prescriptions-order-weekly-transaction-reports-dev-local");

        internal QueueWrapper FirstLogins => CreateQueueWrapper("first-logins-metric-dev-local");
        internal QueueWrapper FirstLoginsAudit => CreateQueueWrapper("first-logins-audit-dev-local");
        internal QueueWrapper DeviceInfoAudit => CreateQueueWrapper("device-info-audit-dev-local");
        internal QueueWrapper IntegratedPartners => CreateQueueWrapper("integrated-partners-dev-local");
        internal QueueWrapper DailyUserTransactions => CreateQueueWrapper("daily-user-transactions-dev-local");
        internal QueueWrapper DailyOdsTransactions => CreateQueueWrapper("daily-ods-transactions-dev-local");
        internal QueueWrapper DailyOdsUsage => CreateQueueWrapper("daily-ods-usage-dev-local");
        internal QueueWrapper DailyUsage => CreateQueueWrapper("daily-usage-dev-local");
        internal QueueWrapper WeeklyUsage => CreateQueueWrapper("weekly-usage-dev-local");
        internal QueueWrapper WeeklyUsageAudit => CreateQueueWrapper("weekly-usage-audit-dev-local");
        internal QueueWrapper MonthlyUsage => CreateQueueWrapper("monthly-usage-dev-local");
        internal QueueWrapper MonthlyUsageAudit => CreateQueueWrapper("monthly-usage-audit-dev-local");
        internal QueueWrapper HighLoginFailureServiceAlert => CreateQueueWrapper("failure-alert-dev-local");
        internal QueueWrapper UnknownErrorsServiceAlert => CreateQueueWrapper("unknownerrors-alert-dev-local");
        internal QueueWrapper ThirdPartyResponsesAlert => CreateQueueWrapper("third-party-failure-alert-dev-local");
        internal QueueWrapper DailyUpdate => CreateQueueWrapper("daily-update-dev-local");
        internal QueueWrapper FirstLoginMessages => CreateQueueWrapper("first-login-messages-dev-local");
        internal QueueWrapper CommsHub => CreateQueueWrapper("comms-hub-dev-local");
        internal QueueWrapper CommsHubSummary => CreateQueueWrapper("comms-hub-summary-dev-local");
        internal QueueWrapper CommsHubSummaryAudit => CreateQueueWrapper("comms-hub-summary-audit-dev-local");

        internal QueueWrapper TestDataGenerate => CreateQueueWrapper("testdata-generate-dev-local");
        internal QueueWrapper ReferrerLogin => CreateQueueWrapper("referrer-login-dev-local");
        internal QueueWrapper ReferrerServiceJourney => CreateQueueWrapper("referrer-servicejourney-dev-local");
        internal QueueWrapper DailyDeviceReferralUsage => CreateQueueWrapper("daily-device-referrals-metric-dev-local");
        internal QueueWrapper Wayfinder => CreateQueueWrapper("wayfinder-dev-local");
        internal QueueWrapper GPHealthRecord => CreateQueueWrapper("gp-health-record-dev-local");

        internal async Task Initialise()
        {
            var storageAccount = CloudStorageAccount.Parse(
                "DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;");

            Client = storageAccount.CreateCloudQueueClient();
            await ClearQueues();
        }

        private async Task ClearQueues()
        {
            var clearedQueues = new List<string>();

            try
            {
                foreach (var queue in Client.ListQueues())
                {
                    clearedQueues.Add(queue.Name);

                    await queue.ClearAsync();

                    var messages = await queue.PeekMessagesAsync(32);
                    messages.Should().BeEmpty("queue {0} was cleared", queue.Name);
                }
            }
            finally
            {
                Logs.Info("Cleared Queues: {0}", string.Join(", ", clearedQueues));
            }
        }

        private QueueWrapper CreateQueueWrapper(string id)
        {
            return new QueueWrapper(Logs, Client.GetQueueReference(id));
        }
    }
}
