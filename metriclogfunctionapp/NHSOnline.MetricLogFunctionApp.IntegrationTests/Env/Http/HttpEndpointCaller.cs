using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Http
{
    internal sealed class HttpEndpointCaller
    {
        private readonly HttpClient _httpClient;
        private readonly string _path;

        public HttpEndpointCaller(TestLogs logs, HttpClient httpClient, string path)
        {
            Logs = logs;
            _httpClient = httpClient;
            _path = path;
        }

        private TestLogs Logs { get; }

        internal async Task<HttpResponseMessage> PostJson(object body, Action<HttpRequestMessage> customise = null)
        {
            var json = JsonConvert.SerializeObject(body, Formatting.None);
            Logs.Info("Posting {0} to {1}", json, _path);

            var message = CreateMessage(json);
            return await Send(message, customise);
        }

        internal async Task<HttpResponseMessage> PostJson(string json)
        {
            var message = CreateMessage(json);
            return await Send(message, null);
        }

        internal HttpRequestMessage CreateMessage(string json)
        {
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var message = new HttpRequestMessage(HttpMethod.Post, _path)
            {
                Content = content
            };
            return message;
        }

        private async Task<HttpResponseMessage> Send(HttpRequestMessage message, Action<HttpRequestMessage> customise)
        {
            customise?.Invoke(message);
            return await _httpClient.SendAsync(message);
        }
    }
}