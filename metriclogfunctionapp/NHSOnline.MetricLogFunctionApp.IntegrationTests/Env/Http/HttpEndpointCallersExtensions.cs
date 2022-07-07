using System;
using System.Net.Http;
using System.Threading.Tasks;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog;

namespace NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Http
{
    internal static class HttpEndpointCallersExtensions
    {
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
