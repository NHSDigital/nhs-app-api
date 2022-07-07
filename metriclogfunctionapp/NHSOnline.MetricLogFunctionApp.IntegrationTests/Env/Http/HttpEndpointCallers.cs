using System;
using System.Net.Http;

namespace NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Http
{
    internal sealed class HttpEndpointCallers: IDisposable
    {
        private readonly HttpClient _httpClient = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:7071/api/"),
            Timeout = TimeSpan.FromSeconds(5)
        };

        public HttpEndpointCallers(TestLogs logs)
        {
            Logs = logs;
        }

        private TestLogs Logs { get; }

        internal HttpEndpointCaller Requeue => new HttpEndpointCaller(Logs, _httpClient, "Resilience_Etl_Requeue");
        internal HttpEndpointCaller RequeueAll => new HttpEndpointCaller(Logs, _httpClient, "Resilience_Etl_RequeueAll");
        internal HttpEndpointCaller AppointmentBooking => new HttpEndpointCaller(Logs, _httpClient, "AppointmentBooking_Etl_Http");
        internal HttpEndpointCaller AppointmentCancelling => new HttpEndpointCaller(Logs, _httpClient, "AppointmentCancelling_Etl_Http");
        internal HttpEndpointCaller AppointmentBook => new HttpEndpointCaller(Logs, _httpClient, "AppointmentBook_Etl_Http");
        internal HttpEndpointCaller AppointmentCancel => new HttpEndpointCaller(Logs, _httpClient, "AppointmentCancel_Etl_Http");
        internal HttpEndpointCaller ClinicalCommissioningGroup => new HttpEndpointCaller(Logs, _httpClient, "ClinicalCommissioningGroup_Etl_Http");
        internal HttpEndpointCaller Consent => new HttpEndpointCaller(Logs, _httpClient, "Consent_Etl_Http");
        internal HttpEndpointCaller Device => new HttpEndpointCaller(Logs, _httpClient, "Device_Etl_Http");
        internal HttpEndpointCaller GeneralPractice => new HttpEndpointCaller(Logs, _httpClient, "GeneralPractice_Etl_Http");
        internal HttpEndpointCaller Login => new HttpEndpointCaller(Logs, _httpClient, "Login_Etl_Http");
        internal HttpEndpointCaller LoginPatientIdentifier => new HttpEndpointCaller(Logs, _httpClient, "LoginPatientIdentifier_Etl_Http");
        internal HttpEndpointCaller SilverIntegrationJumpOff => new HttpEndpointCaller(Logs, _httpClient, "SilverIntegrationJumpOff_Etl_Http");
        internal HttpEndpointCaller CommsHubCommunication => new HttpEndpointCaller(Logs, _httpClient, "CommsHub_Communication_Etl_Http");
        internal HttpEndpointCaller CommsHubTransmission => new HttpEndpointCaller(Logs, _httpClient, "CommsHub_Transmission_Etl_Http");
        internal HttpEndpointCaller CommsHubMessageRead => new HttpEndpointCaller(Logs, _httpClient, "CommsHub_MessageRead_Etl_Http");
        internal HttpEndpointCaller AppGenMessageCreate => new HttpEndpointCaller(Logs, _httpClient, "AppGenMessageCreate_Etl_Http");
        internal HttpEndpointCaller AppGenMessageRead => new HttpEndpointCaller(Logs, _httpClient, "AppGenMessageRead_Etl_Http");
        internal HttpEndpointCaller AppGenMessageLinkClick => new HttpEndpointCaller(Logs, _httpClient, "AppGenMessageLinkClick_Etl_Http");
        internal HttpEndpointCaller MedicalRecordView => new HttpEndpointCaller(Logs, _httpClient, "MedicalRecordView_Etl_Http");
        internal HttpEndpointCaller NominatedPharmacyCreate => new HttpEndpointCaller(Logs, _httpClient, "NominatedPharmacyCreate_Etl_Http");
        internal HttpEndpointCaller NominatedPharmacyUpdate => new HttpEndpointCaller(Logs, _httpClient, "NominatedPharmacyUpdate_Etl_Http");
        internal HttpEndpointCaller OrganDonationRegistrationCreate => new HttpEndpointCaller(Logs, _httpClient, "OrganDonationRegistrationCreate_Etl_Http");
        internal HttpEndpointCaller OrganDonationRegistrationGet => new HttpEndpointCaller(Logs, _httpClient, "OrganDonationRegistrationGet_Etl_Http");
        internal HttpEndpointCaller OrganDonationRegistrationUpdate => new HttpEndpointCaller(Logs, _httpClient, "OrganDonationRegistrationUpdate_Etl_Http");
        internal HttpEndpointCaller OrganDonationRegistrationWithdraw => new HttpEndpointCaller(Logs, _httpClient, "OrganDonationRegistrationWithdraw_Etl_Http");
        internal HttpEndpointCaller RepeatPrescriptionOrder => new HttpEndpointCaller(Logs, _httpClient, "PrescriptionOrders_Etl_Http");
        internal HttpEndpointCaller PatientsEnabled => new HttpEndpointCaller(Logs, _httpClient, "PatientsEnabled_Etl_Http");
        internal HttpEndpointCaller PatientList => new HttpEndpointCaller(Logs, _httpClient, "PatientList_Etl_Http");
        internal HttpEndpointCaller RefreshMaterializedViews => new HttpEndpointCaller(Logs, _httpClient, "RefreshMaterializedViews_Compute_Http");
        internal HttpEndpointCaller AppointmentBookWeeklyTransactionReport => new HttpEndpointCaller(Logs, _httpClient, "AppointmentBookWeeklyTransactionReport_Etl_Http");
        internal HttpEndpointCaller AppointmentCancelWeeklyTransactionReport => new HttpEndpointCaller(Logs, _httpClient, "AppointmentCancelWeeklyTransactionReport_Etl_Http");
        internal HttpEndpointCaller UpliftStarted => new HttpEndpointCaller(Logs, _httpClient, "UpliftStarted_Etl_Http");
        internal HttpEndpointCaller AppleDownloads => new HttpEndpointCaller(Logs, _httpClient, "AppleDownloads_Etl_Http");
        internal HttpEndpointCaller GoogleDownloads => new HttpEndpointCaller(Logs, _httpClient, "GoogleDownloads_Etl_Http");

        internal HttpEndpointCaller LoginWeeklyTransactionReport => new HttpEndpointCaller(Logs, _httpClient, "LoginWeeklyTransactionReport_Etl_Http");
        internal HttpEndpointCaller MyRecordViewWeeklyTransactionReport => new HttpEndpointCaller(Logs, _httpClient, "MyRecordViewWeeklyTransactionReport_Etl_Http");
        internal HttpEndpointCaller OrganDonationCreateWeeklyTransactionReport => new HttpEndpointCaller(Logs, _httpClient, "OrganDonationCreateWeeklyTransactionReport_Etl_Http");
        internal HttpEndpointCaller OrganDonationGetWeeklyTransactionReport => new HttpEndpointCaller(Logs, _httpClient, "OrganDonationGetWeeklyTransactionReport_Etl_Http");
        internal HttpEndpointCaller OrganDonationUpdateWeeklyTransactionReport => new HttpEndpointCaller(Logs, _httpClient, "OrganDonationUpdateWeeklyTransactionReport_Etl_Http");
        internal HttpEndpointCaller OrganDonationWithdrawWeeklyTransactionReport => new HttpEndpointCaller(Logs, _httpClient, "OrganDonationWithdrawWeeklyTransactionReport_Etl_Http");
        internal HttpEndpointCaller PrescriptionsOrderWeeklyTransactionReport => new HttpEndpointCaller(Logs, _httpClient, "PrescriptionsOrderWeeklyTransactionReport_Etl_Http");

        internal HttpEndpointCaller FirstLogins => new HttpEndpointCaller(Logs, _httpClient, "FirstLogins_Compute_Http");
        internal HttpEndpointCaller FirstLoginsAudit => new HttpEndpointCaller(Logs, _httpClient, "First_Logins_Audit_Http");
        internal HttpEndpointCaller DailyUserTransactions=> new HttpEndpointCaller(Logs, _httpClient, "DailyUserTransactions_Compute_Http");
        internal HttpEndpointCaller DailyOdsTransactions=> new HttpEndpointCaller(Logs, _httpClient, "DailyOdsTransactions_Compute_Http");
        internal HttpEndpointCaller IntegratedPartners => new HttpEndpointCaller(Logs, _httpClient, "IntegratedPartners_Compute_Http");
        internal HttpEndpointCaller DailyUsage => new HttpEndpointCaller(Logs, _httpClient, "DailyUsage_Compute_Http");
        internal HttpEndpointCaller WeeklyUsage => new HttpEndpointCaller(Logs, _httpClient, "WeeklyUsage_Compute_Http");
        internal HttpEndpointCaller MonthlyUsage => new HttpEndpointCaller(Logs, _httpClient, "MonthlyUsage_Compute_Http");
        internal HttpEndpointCaller MonthlyUsageAudit => new HttpEndpointCaller(Logs, _httpClient, "MonthlyUsage_Audit_Http");
        internal HttpEndpointCaller DailyOdsUsage => new HttpEndpointCaller(Logs, _httpClient, "DailyOdsUsage_Compute_Http");
        internal HttpEndpointCaller FirstLoginMessages => new HttpEndpointCaller(Logs, _httpClient, "FirstLoginMessages_Compute_Http");
        internal HttpEndpointCaller CommsHub => new HttpEndpointCaller(Logs, _httpClient, "CommsHub_Compute_Http");
        internal HttpEndpointCaller CommsHubSummary => new HttpEndpointCaller(Logs, _httpClient, "CommsHubSummary_Compute_Http");
        internal HttpEndpointCaller WeeklyUsageAudit => new HttpEndpointCaller(Logs, _httpClient, "WeeklyUsage_Audit_Http");
        internal HttpEndpointCaller DeviceInfo => new HttpEndpointCaller(Logs, _httpClient, "DeviceInfo_Compute_Http");
        internal HttpEndpointCaller CommsHubSummaryAudit => new HttpEndpointCaller(Logs, _httpClient, "CommsHubSummary_Audit_Http");

        internal HttpEndpointCaller HighLoginFailureServiceAlert => new HttpEndpointCaller(Logs, _httpClient, "HighLoginFailureServiceAlert_Etl_Http");
        internal HttpEndpointCaller UnknownErrorsServiceAlert => new HttpEndpointCaller(Logs, _httpClient, "UnknownErrorsServiceAlert_Etl_Http");
        internal HttpEndpointCaller ThirdPartyResponsesAlert => new HttpEndpointCaller(Logs, _httpClient, "ThirdPartyResponsesServiceAlert_Etl_Http");
        internal HttpEndpointCaller DailyUpdate => new HttpEndpointCaller(Logs, _httpClient, "DailyUpdate_Etl_Http");

        internal HttpEndpointCaller TestDataGenerator => new HttpEndpointCaller(Logs, _httpClient, "TestData_Generate_Http");

        internal HttpEndpointCaller CommsHubConsumer => new HttpEndpointCaller(Logs, _httpClient, "CommsHub_Etl_Http");
        internal HttpEndpointCaller AuditLogConsumer => new HttpEndpointCaller(Logs, _httpClient, "AuditLog_Etl_Http");
        internal HttpEndpointCaller ReferrerLogin => new HttpEndpointCaller(Logs, _httpClient, "ReferrerLogin_Compute_Http");
        internal HttpEndpointCaller DailyDeviceReferralUsage => new HttpEndpointCaller(Logs, _httpClient, "DailyDeviceReferralUsage_Compute_Http");
        internal HttpEndpointCaller ReferrerServiceJourney => new HttpEndpointCaller(Logs, _httpClient, "ReferrerServiceJourney_Compute_Http");
        internal HttpEndpointCaller Wayfinder => new HttpEndpointCaller(Logs, _httpClient, "Wayfinder_Compute_Http");

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
