using System.Net.Http;
using System.Threading.Tasks;

namespace NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Http
{
    internal static class HttpEndpointCallersTransactionReportExtensions
    {
        internal static async Task<HttpResponseMessage> PostLoginTransactionReportRequest(
            this HttpEndpointCallers httpEndpointCallers,
            string startDateTime)
        {
            var request = new { StartDateTime = startDateTime };
            return await httpEndpointCallers.LoginWeeklyTransactionReport.PostJson(request);
        }

        internal static async Task<HttpResponseMessage> PostAppointmentBookTransactionReportRequest(
            this HttpEndpointCallers httpEndpointCallers,
            string startDateTime)
        {
            var request = new { StartDateTime = startDateTime };
            return await httpEndpointCallers.AppointmentBookWeeklyTransactionReport.PostJson(request);
        }

        internal static async Task<HttpResponseMessage> PostAppointmentCancelTransactionReportRequest(
            this HttpEndpointCallers httpEndpointCallers,
            string startDateTime)
        {
            var request = new { StartDateTime = startDateTime };
            return await httpEndpointCallers.AppointmentCancelWeeklyTransactionReport.PostJson(request);
        }

        internal static async Task<HttpResponseMessage> PostMyRecordViewTransactionReportRequest(
            this HttpEndpointCallers httpEndpointCallers,
            string startDateTime)
        {
            var request = new { StartDateTime = startDateTime };
            return await httpEndpointCallers.MyRecordViewWeeklyTransactionReport.PostJson(request);
        }

        internal static async Task<HttpResponseMessage> PostOrganDonationCreateTransactionReportRequest(
            this HttpEndpointCallers httpEndpointCallers,
            string startDateTime)
        {
            var request = new { StartDateTime = startDateTime };
            return await httpEndpointCallers.OrganDonationCreateWeeklyTransactionReport.PostJson(request);
        }

        internal static async Task<HttpResponseMessage> PostOrganDonationGetTransactionReportRequest(
            this HttpEndpointCallers httpEndpointCallers,
            string startDateTime)
        {
            var request = new { StartDateTime = startDateTime };
            return await httpEndpointCallers.OrganDonationGetWeeklyTransactionReport.PostJson(request);
        }

        internal static async Task<HttpResponseMessage> PostOrganDonationUpdateTransactionReportRequest(
            this HttpEndpointCallers httpEndpointCallers,
            string startDateTime)
        {
            var request = new { StartDateTime = startDateTime };
            return await httpEndpointCallers.OrganDonationUpdateWeeklyTransactionReport.PostJson(request);
        }

        internal static async Task<HttpResponseMessage> PostOrganDonationWithdrawTransactionReportRequest(
            this HttpEndpointCallers httpEndpointCallers,
            string startDateTime)
        {
            var request = new { StartDateTime = startDateTime };
            return await httpEndpointCallers.OrganDonationWithdrawWeeklyTransactionReport.PostJson(request);
        }

        internal static async Task<HttpResponseMessage> PostPrescriptionsOrderTransactionReportRequest(
            this HttpEndpointCallers httpEndpointCallers,
            string startDateTime)
        {
            var request = new { StartDateTime = startDateTime };
            return await httpEndpointCallers.PrescriptionsOrderWeeklyTransactionReport.PostJson(request);
        }

    }
}