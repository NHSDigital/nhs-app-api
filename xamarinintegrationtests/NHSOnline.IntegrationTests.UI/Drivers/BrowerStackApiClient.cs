using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using NHSOnline.IntegrationTests.UI.Drivers.Native;
using OpenQA.Selenium.Remote;

namespace NHSOnline.IntegrationTests.UI.Drivers
{
    internal class BrowerStackApiClient
    {
        private readonly string _authorisation;

        private static readonly Lazy<HttpClient> _client = new Lazy<HttpClient>(() =>
            new HttpClient(new SocketsHttpHandler
            {
                PooledConnectionLifetime = TimeSpan.FromMinutes(20)
            }));

        public BrowerStackApiClient(BrowserStackConfig browserStackConfig)
        {
            _authorisation = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{browserStackConfig.User}:{browserStackConfig.GetKey()}"));
        }

        public void UpdateStatus(SessionId sessionId, string status, string statusReason)
        {
            using var request = new HttpRequestMessage(HttpMethod.Put,
                new Uri($"https://api-cloud.browserstack.com/app-automate/sessions/{sessionId}.json"));

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
    }
}