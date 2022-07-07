using System;
using System.Net.Http;
using System.Threading.Tasks;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog;

namespace NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Http
{
    internal static class HttpEndpointCallersExtensions
    {
        internal static async Task<HttpResponseMessage> PostAppointmentBooking(
            this HttpEndpointCallers httpEndpointCallers,
            string startDateTime,
            string endDateTime)
        {
            var request = new { StartDateTime = startDateTime, EndDateTime = endDateTime };
            return await httpEndpointCallers.AppointmentBooking.PostJson(request);
        }

        internal static async Task<HttpResponseMessage> PostAppointmentCancelling(
            this HttpEndpointCallers httpEndpointCallers,
            string startDateTime,
            string endDateTime)
        {
            var request = new { StartDateTime = startDateTime, EndDateTime = endDateTime };
            return await httpEndpointCallers.AppointmentCancelling.PostJson(request);
        }

        internal static async Task<HttpResponseMessage> PostAppointmentBook(
            this HttpEndpointCallers httpEndpointCallers,
            string startDateTime,
            string endDateTime)
        {
            var request = new { StartDateTime = startDateTime, EndDateTime = endDateTime };
            return await httpEndpointCallers.AppointmentBook.PostJson(request);
        }

        internal static async Task<HttpResponseMessage> PostAppointmentCancel(
            this HttpEndpointCallers httpEndpointCallers,
            string startDateTime,
            string endDateTime)
        {
            var request = new { StartDateTime = startDateTime, EndDateTime = endDateTime };
            return await httpEndpointCallers.AppointmentCancel.PostJson(request);
        }

        internal static async Task<HttpResponseMessage> PostConsent(
            this HttpEndpointCallers httpEndpointCallers,
            string startDateTime,
            string endDateTime)
        {
            var request = new { StartDateTime = startDateTime, EndDateTime = endDateTime };
            return await httpEndpointCallers.Consent.PostJson(request);
        }

        internal static async Task<HttpResponseMessage> PostDevice(
            this HttpEndpointCallers httpEndpointCallers,
            string startDateTime,
            string endDateTime)
        {
            var request = new { StartDateTime = startDateTime, EndDateTime = endDateTime };
            return await httpEndpointCallers.Device.PostJson(request);
        }

        internal static async Task<HttpResponseMessage> PostLogin(
            this HttpEndpointCallers httpEndpointCallers,
            string startDateTime,
            string endDateTime)
        {
            var request = new { StartDateTime = startDateTime, EndDateTime = endDateTime };
            return await httpEndpointCallers.Login.PostJson(request);
        }

        internal static async Task<HttpResponseMessage> PostLoginPatientIdentifier(
            this HttpEndpointCallers httpEndpointCallers,
            string startDateTime,
            string endDateTime)
        {
            var request = new { StartDateTime = startDateTime, EndDateTime = endDateTime };
            return await httpEndpointCallers.LoginPatientIdentifier.PostJson(request);
        }

        internal static async Task<HttpResponseMessage> PostSilverIntegrationJumpOff(
            this HttpEndpointCallers httpEndpointCallers,
            string startDateTime,
            string endDateTime)
        {
            var request = new { StartDateTime = startDateTime, EndDateTime = endDateTime };
            return await httpEndpointCallers.SilverIntegrationJumpOff.PostJson(request);
        }

        internal static async Task<HttpResponseMessage> PostCommsHubCommunication(
            this HttpEndpointCallers httpEndpointCallers,
            string startDateTime,
            string endDateTime)
        {
            var request = new { StartDateTime = startDateTime, EndDateTime = endDateTime };
            return await httpEndpointCallers.CommsHubCommunication.PostJson(request);
        }

        internal static async Task<HttpResponseMessage> PostCommsHubTransmission(
            this HttpEndpointCallers httpEndpointCallers,
            string startDateTime,
            string endDateTime)
        {
            var request = new { StartDateTime = startDateTime, EndDateTime = endDateTime };
            return await httpEndpointCallers.CommsHubTransmission.PostJson(request);
        }

        internal static async Task<HttpResponseMessage> PostRequeue(
            this HttpEndpointCallers httpEndpointCallers,
            string queueName)
        {
            var request = new {};
            return await httpEndpointCallers.Requeue.PostJson(
                request,
                message => message.RequestUri = new Uri(message.RequestUri.OriginalString + "/" + queueName, UriKind.Relative));
        }

        internal static async Task<HttpResponseMessage> PostRequeueAll(
            this HttpEndpointCallers httpEndpointCallers)
        {
            var request = new { };
            return await httpEndpointCallers.RequeueAll.PostJson(request);
        }

        internal static async Task<HttpResponseMessage> PostCommsHubMessageRead(
            this HttpEndpointCallers httpEndpointCallers,
            string startDateTime,
            string endDateTime)
        {
            var request = new { StartDateTime = startDateTime, EndDateTime = endDateTime };
            return await httpEndpointCallers.CommsHubMessageRead.PostJson(request);
        }

        internal static async Task<HttpResponseMessage> PostAppGenMessageCreate(
            this HttpEndpointCallers httpEndpointCallers,
            string startDateTime,
            string endDateTime)
        {
            var request = new { StartDateTime = startDateTime, EndDateTime = endDateTime };
            return await httpEndpointCallers.AppGenMessageCreate.PostJson(request);
        }

        internal static async Task<HttpResponseMessage> PostAppGenMessageRead(
            this HttpEndpointCallers httpEndpointCallers,
            string startDateTime,
            string endDateTime)
        {
            var request = new { StartDateTime = startDateTime, EndDateTime = endDateTime };
            return await httpEndpointCallers.AppGenMessageRead.PostJson(request);
        }

        internal static async Task<HttpResponseMessage> PostAppGenMessageLinkClick(
            this HttpEndpointCallers httpEndpointCallers,
            string startDateTime,
            string endDateTime)
        {
            var request = new { StartDateTime = startDateTime, EndDateTime = endDateTime };
            return await httpEndpointCallers.AppGenMessageLinkClick.PostJson(request);
        }

        internal static async Task<HttpResponseMessage> PostClinicalCommissioningGroup(
            this HttpEndpointCallers httpEndpointCallers,
            string sinceDate)
        {
            var request = new { SinceDate = sinceDate };
            return await httpEndpointCallers.ClinicalCommissioningGroup.PostJson(request);
        }

        internal static async Task<HttpResponseMessage> PostGeneralPractice(
            this HttpEndpointCallers httpEndpointCallers,
            string sinceDate)
        {
            var request = new { SinceDate = sinceDate };
            return await httpEndpointCallers.GeneralPractice.PostJson(request);
        }

        internal static async Task<HttpResponseMessage> PostNominatedPharmacyCreate(
            this HttpEndpointCallers httpEndpointCallers,
            string startDateTime,
            string endDateTime)
        {
            var request = new { StartDateTime = startDateTime, EndDateTime = endDateTime };
            return await httpEndpointCallers.NominatedPharmacyCreate.PostJson(request);
        }

        internal static async Task<HttpResponseMessage> PostNominatedPharmacyUpdate(
            this HttpEndpointCallers httpEndpointCallers,
            string startDateTime,
            string endDateTime)
        {
            var request = new { StartDateTime = startDateTime, EndDateTime = endDateTime };
            return await httpEndpointCallers.NominatedPharmacyUpdate.PostJson(request);
        }

        internal static async Task<HttpResponseMessage> PostOrganDonationRegistrationCreate(
            this HttpEndpointCallers httpEndpointCallers,
            string startDateTime,
            string endDateTime)
        {
            var request = new { StartDateTime = startDateTime, EndDateTime = endDateTime };
            return await httpEndpointCallers.OrganDonationRegistrationCreate.PostJson(request);
        }

        internal static async Task<HttpResponseMessage> PostOrganDonationRegistrationGet(
            this HttpEndpointCallers httpEndpointCallers,
            string startDateTime,
            string endDateTime)
        {
            var request = new { StartDateTime = startDateTime, EndDateTime = endDateTime };
            return await httpEndpointCallers.OrganDonationRegistrationGet.PostJson(request);
        }

        internal static async Task<HttpResponseMessage> PostOrganDonationRegistrationUpdate(
            this HttpEndpointCallers httpEndpointCallers,
            string startDateTime,
            string endDateTime)
        {
            var request = new { StartDateTime = startDateTime, EndDateTime = endDateTime };
            return await httpEndpointCallers.OrganDonationRegistrationUpdate.PostJson(request);
        }

        internal static async Task<HttpResponseMessage> PostOrganDonationRegistrationWithdraw(
            this HttpEndpointCallers httpEndpointCallers,
            string startDateTime,
            string endDateTime)
        {
            var request = new { StartDateTime = startDateTime, EndDateTime = endDateTime };
            return await httpEndpointCallers.OrganDonationRegistrationWithdraw.PostJson(request);
        }

        internal static async Task<HttpResponseMessage> PostRepeatPrescriptionOrders(
            this HttpEndpointCallers httpEndpointCallers,
            string startDateTime,
            string endDateTime)
        {
            var request = new { StartDateTime = startDateTime, EndDateTime = endDateTime };
            return await httpEndpointCallers.RepeatPrescriptionOrder.PostJson(request);
        }

        internal static async Task<HttpResponseMessage> PostMedicalRecordView(
            this HttpEndpointCallers httpEndpointCallers,
            string startDateTime,
            string endDateTime)
        {
            var request = new { StartDateTime = startDateTime, EndDateTime = endDateTime };
            return await httpEndpointCallers.MedicalRecordView.PostJson(request);
        }

        internal static async Task<HttpResponseMessage> PostRefreshMaterializedViews(
            this HttpEndpointCallers httpEndpointCallers)
        {
            var request = new { };
            return await httpEndpointCallers.RefreshMaterializedViews.PostJson(request);
        }

        internal static async Task<HttpResponseMessage> PostPatientsEnabled(
            this HttpEndpointCallers httpEndpointCallers,
            string path)
        {
            var request = new { Path = path };
            return await httpEndpointCallers.PatientsEnabled.PostJson(request);
        }

        internal static async Task<HttpResponseMessage> PostPatientList(this HttpEndpointCallers httpEndpointCallers,
            string path)
        {
            var request = new { Path = path };
            return await httpEndpointCallers.PatientList.PostJson(request);
        }

        internal static async Task<HttpResponseMessage> PostFirstLogins(
            this HttpEndpointCallers httpEndpointCallers,
            string startDateTime,
            string endDateTime)
        {
            var request = new { StartDateTime = startDateTime, EndDateTime = endDateTime };
            return await httpEndpointCallers.FirstLogins.PostJson(request);
        }

        internal static async Task<HttpResponseMessage> PostFirstLoginsAudit(
            this HttpEndpointCallers httpEndpointCallers,
            string processName,
            string startDateTime,
            string endDateTime)
        {
            var request = new { ProcessName = processName, StartDateTime = startDateTime, EndDateTime = endDateTime };
            return await httpEndpointCallers.FirstLoginsAudit.PostJson(request);
        }

        internal static async Task<HttpResponseMessage> PostIntegratedPartners(
            this HttpEndpointCallers httpEndpointCallers,
            string startDateTime,
            string endDateTime)
        {
            var request = new { StartDateTime = startDateTime, EndDateTime = endDateTime };
            return await httpEndpointCallers.IntegratedPartners.PostJson(request);
        }

        internal static async Task<HttpResponseMessage> PostFirstLoginMessages(
            this HttpEndpointCallers httpEndpointCallers,
            string startDateTime,
            string endDateTime)
        {
            var request = new { StartDateTime = startDateTime, EndDateTime = endDateTime };
            return await httpEndpointCallers.FirstLoginMessages.PostJson(request);
        }

        internal static async Task<HttpResponseMessage> PostCommsHubSummary(
            this HttpEndpointCallers httpEndpointCallers,
            string startDateTime,
            string endDateTime)
        {
            var request = new { StartDateTime = startDateTime, EndDateTime = endDateTime };
            return await httpEndpointCallers.CommsHubSummary.PostJson(request);
        }

        internal static async Task<HttpResponseMessage> PostCommsHub(
            this HttpEndpointCallers httpEndpointCallers,
            string startDateTime,
            string endDateTime)
        {
            var request = new { StartDateTime = startDateTime, EndDateTime = endDateTime };
            return await httpEndpointCallers.CommsHub.PostJson(request);
        }

        internal static async Task<HttpResponseMessage> PostWeeklyUsageAudit(
            this HttpEndpointCallers httpEndpointCallers,
            string processName,
            string startDateTime,
            string endDateTime)
        {
            var request = new { ProcessName = processName, StartDateTime = startDateTime, EndDateTime = endDateTime };
            return await httpEndpointCallers.WeeklyUsageAudit.PostJson(request);
        }

        internal static async Task<HttpResponseMessage> PostMonthlyUsageAudit(
            this HttpEndpointCallers httpEndpointCallers,
            string processName,
            string startDateTime,
            string endDateTime)
        {
            var request = new { ProcessName = processName, StartDateTime = startDateTime, EndDateTime = endDateTime };
            return await httpEndpointCallers.MonthlyUsageAudit.PostJson(request);
        }

        internal static async Task<HttpResponseMessage> PostTestDataGenerator(
            this HttpEndpointCallers httpEndpointCallers,
            string startDateTime,
            string endDateTime)
        {
            var request = new { StartDateTime = startDateTime, EndDateTime = endDateTime };
            return await httpEndpointCallers.TestDataGenerator.PostJson(request);
        }

        internal static async Task<HttpResponseMessage> PostUpliftStarted(
            this HttpEndpointCallers httpEndpointCallers,
            string startDateTime,
            string endDateTime)
        {
            var request = new { StartDateTime = startDateTime, EndDateTime = endDateTime };
            return await httpEndpointCallers.UpliftStarted.PostJson(request);
        }

        internal static async Task<HttpResponseMessage> PostHighLoginFailureServiceAlert(
            this HttpEndpointCallers httpEndpointCallers,
            string startDateTime,
            string endDateTime)
        {
            var request = new { StartDateTime = startDateTime, EndDateTime = endDateTime };
            return await httpEndpointCallers.HighLoginFailureServiceAlert.PostJson(request);
        }

        internal static async Task<HttpResponseMessage> PostDeviceInfo(this HttpEndpointCallers httpEndpointCallers,
            string startDateTime,
            string endDateTime)
        {
            var request = new { StartDateTime = startDateTime, EndDateTime = endDateTime };
            return await httpEndpointCallers.DeviceInfo.PostJson(request);
        }

        internal static async Task<HttpResponseMessage> PostCommsHubSummaryAudit(
            this HttpEndpointCallers httpEndpointCallers,
            string processName,
            string startDateTime,
            string endDateTime)
        {
            var request = new { ProcessName = processName, StartDateTime = startDateTime, EndDateTime = endDateTime };
            return await httpEndpointCallers.CommsHubSummaryAudit.PostJson(request);
        }

        internal static async Task<HttpResponseMessage> PostCommsHubConsumer(
            this HttpEndpointCallers httpEndpointCallers,
            params string[] parameters)
        {
            return await httpEndpointCallers.CommsHubConsumer.PostJson(parameters);
        }

        internal static async Task<HttpResponseMessage> PostAuditLogConsumer(
            this HttpEndpointCallers httpEndpointCallers,
            params AuditRecord[] parameters)
        {
            return await httpEndpointCallers.AuditLogConsumer.PostJson(parameters);
        }

        internal static async Task<HttpResponseMessage> PostReferrerLogin(this HttpEndpointCallers httpEndpointCallers,
            string startDateTime,
            string endDateTime)
        {
            var request = new { StartDateTime = startDateTime, EndDateTime = endDateTime };
            return await httpEndpointCallers.ReferrerLogin.PostJson(request);
        }

        internal static async Task<HttpResponseMessage> PostDailyDeviceReferralUsage(
            this HttpEndpointCallers httpEndpointCallers,
            string startDateTime,
            string endDateTime)
        {
            var request = new { StartDateTime = startDateTime, EndDateTime = endDateTime };

            return await httpEndpointCallers.DailyDeviceReferralUsage.PostJson(request);
        }

        internal static async Task<HttpResponseMessage> PostReferrerServiceJourney(
            this HttpEndpointCallers httpEndpointCallers,
            string startDateTime,
            string endDateTime)
        {
            var request = new { StartDateTime = startDateTime, EndDateTime = endDateTime };
            return await httpEndpointCallers.ReferrerServiceJourney.PostJson(request);
        }

        internal static async Task<HttpResponseMessage> PostWayfinder(
            this HttpEndpointCallers httpEndpointCallers,
            string startDateTime,
            string endDateTime)
        {
            var request = new { StartDateTime = startDateTime, EndDateTime = endDateTime };
            return await httpEndpointCallers.Wayfinder.PostJson(request);
        }
    }
}
