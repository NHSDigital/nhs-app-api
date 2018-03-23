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


    }
}
