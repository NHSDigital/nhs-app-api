using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NHSOnline.Backend.Worker.IntegrationTests.Mocking.Nhso.Models.Patient;

namespace NHSOnline.Backend.Worker.IntegrationTests.Worker
{
    public class PatientClient
    {
        private readonly WorkerClient _client;
        private readonly JsonSerializerSettings _jsonSerializerSettings;

        public PatientClient(WorkerClient client, JsonSerializerSettings jsonSerializerSettings)
        {
            _client = client;
            _jsonSerializerSettings = jsonSerializerSettings;
        }

        public async Task<Im1ConnectionResponse> GetIm1Connection(string connectionToken, string odsCode)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, WorkerPaths.PatientIm1Connection);
            request.Headers.Add(WorkerHeaders.ConnectionToken, connectionToken);
            request.Headers.Add(WorkerHeaders.OdsCode, odsCode);
            var response = await _client.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Im1ConnectionResponse>(json, _jsonSerializerSettings);
            return result;
        }
    }
}
