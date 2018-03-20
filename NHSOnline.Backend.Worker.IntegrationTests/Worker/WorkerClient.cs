using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NHSOnline.Backend.Worker.IntegrationTests.Mocking;

namespace NHSOnline.Backend.Worker.IntegrationTests.Worker
{
    public class WorkerClient
    {
        private readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        private readonly HttpClient _client;

        public WorkerClient()
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri(Configuration.BackendBaseUrl)
            };

            Patient = new PatientClient(this, _jsonSerializerSettings);
        }

        public PatientClient Patient { get; }

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            var response = await _client.SendAsync(request);

            return response;
        }
    }
}
