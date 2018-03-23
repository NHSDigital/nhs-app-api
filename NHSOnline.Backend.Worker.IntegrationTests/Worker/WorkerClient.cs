using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NHSOnline.Backend.Worker.IntegrationTests.Mocking;
using NHSOnline.Backend.Worker.IntegrationTests.Mocking.Nhso.Models.Patient;

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
        }

        public async Task<Im1ConnectionResponse> GetIm1Connection(string connectionToken, string odsCode)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, WorkerPaths.PatientIm1Connection);
            request.Headers.Add(WorkerHeaders.ConnectionToken, connectionToken);
            request.Headers.Add(WorkerHeaders.OdsCode, odsCode);
            var response = await _client.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                // Exception is thrown here to ensure that the tests fail at the appropriate location and not further down the line
                // when values are not as expected.  This makes it easier to debug.
                throw new NhsoHttpException(response.StatusCode, json);
            }

            var result = JsonConvert.DeserializeObject<Im1ConnectionResponse>(json, _jsonSerializerSettings);
            return result;
        }

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            var response = await _client.SendAsync(request);

            return response;
        }
    }
}
