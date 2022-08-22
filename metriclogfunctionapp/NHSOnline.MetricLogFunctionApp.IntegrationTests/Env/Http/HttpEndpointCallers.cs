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
        internal HttpEndpointCaller AuditLogConsumer => new HttpEndpointCaller(Logs, _httpClient, "AuditLog_Etl_Http");
        internal HttpEndpointCaller DailyDeviceReferralUsage => new HttpEndpointCaller(Logs, _httpClient, "DailyDeviceReferralUsage_Compute_Http");
        internal HttpEndpointCaller DeviceInfo => new HttpEndpointCaller(Logs, _httpClient, "DeviceInfo_Compute_Http");
        internal HttpEndpointCaller FirstLogins => new HttpEndpointCaller(Logs, _httpClient, "FirstLogins_Compute_Http");
        internal HttpEndpointCaller ReferrerLogin => new HttpEndpointCaller(Logs, _httpClient, "ReferrerLogin_Compute_Http");
        internal HttpEndpointCaller ReferrerServiceJourney => new HttpEndpointCaller(Logs, _httpClient, "ReferrerServiceJourney_Compute_Http");
        internal HttpEndpointCaller Wayfinder => new HttpEndpointCaller(Logs, _httpClient, "Wayfinder_Compute_Http");

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
