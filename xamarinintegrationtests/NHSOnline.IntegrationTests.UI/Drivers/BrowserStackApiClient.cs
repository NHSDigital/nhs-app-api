using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NHSOnline.IntegrationTests.UI.Drivers.Native;
using OpenQA.Selenium.Remote;

namespace NHSOnline.IntegrationTests.UI.Drivers
{
    internal class BrowserStackApiClient
    {
        private readonly string _authorisation;

        private static readonly Lazy<HttpClient> _client = new Lazy<HttpClient>(() =>
            new HttpClient(new SocketsHttpHandler
            {
                PooledConnectionLifetime = TimeSpan.FromMinutes(20)
            }));

        public BrowserStackApiClient(BrowserStackConfig browserStackConfig)
        {
            _authorisation = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{browserStackConfig.User}:{browserStackConfig.GetKey()}"));
        }

        private Uri SessionUri(SessionId sessionId) =>
            new Uri($"https://api-cloud.browserstack.com/app-automate/sessions/{sessionId}.json");

        private JsonSerializerSettings SerialiserSettings => new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            }
        };

        public void UpdateStatus(SessionId sessionId, string status, string statusReason)
        {
            using var request = new HttpRequestMessage(HttpMethod.Put, SessionUri(sessionId));

            var jsonBody = JsonConvert.SerializeObject(new
            {
                status,
                reason = statusReason
            });

            using var content = new StringContent(jsonBody, Encoding.Default, "application/json");
            request.Content = content;
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", _authorisation);

            using var _ = _client.Value.SendAsync(request).Result;
        }

        public BrowserStackSessionDetails? GetSessionDetails(SessionId sessionId)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, SessionUri(sessionId));

            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", _authorisation);

            using var httpResponse = _client.Value.SendAsync(request).Result;

            var response = JsonConvert.DeserializeObject<BrowserStackSessionResponse>(
                httpResponse.Content.ReadAsStringAsync().Result,
                SerialiserSettings);

            return response.AutomationSession;
        }

        private record BrowserStackSessionResponse(BrowserStackSessionDetails? AutomationSession);
    }
}