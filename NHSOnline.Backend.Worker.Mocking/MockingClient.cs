using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NHSOnline.Backend.Worker.Mocking.Models;

namespace NHSOnline.Backend.Worker.Mocking
{
    public class MockingClient
    {
        internal static MockingConfiguration Configuration { get; private set; }

        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        private readonly HttpClient _client;

        public MockingClient(MockingConfiguration mockingConfiguration)
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri(mockingConfiguration.WiremockBaseUrl)
            };

            Configuration = mockingConfiguration;
        }

        public async Task PostMappingAsync(Mapping mapping)
        {
            var json = Serialize(mapping);
            var request = new HttpRequestMessage(HttpMethod.Post, "mappings") { Content = new StringContent(json) };
            request.Content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");

            var response = await _client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"PostMapping failed, response was {response.StatusCode}: {content}");
            }
        }

        public async Task ResetMappings()
        {
            await _client.DeleteAsync("mappings");
        }

        private static string Serialize(object toSerialize)
        {
            return JsonConvert.SerializeObject(toSerialize, SerializerSettings);
        }
    }
}
