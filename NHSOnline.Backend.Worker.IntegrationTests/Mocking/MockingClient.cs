using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NHSOnline.Backend.Worker.IntegrationTests.Mocking.Models;

namespace NHSOnline.Backend.Worker.IntegrationTests.Mocking
{
    public class MockingClient
    {
        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        private readonly HttpClient _client;

        public MockingClient()
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri(Configuration.EmisBaseUrl)
            };
        }

        public async Task<IEnumerable<Mapping>> GetMappingsAsync()
        {
            return Deserialize<List<Mapping>>(await _client.GetStringAsync("mappings"));
        }

        public Task<Mapping> GetMappingAsync(string guid)
        {
            return GetMappingAsync(new Guid(guid));
        }

        public async Task<Mapping> GetMappingAsync(Guid guid)
        {
            return Deserialize<Mapping>(await _client.GetStringAsync($"mappings/{guid}"));
        }

        public async Task PostMappingAsync(Mapping mapping)
        {
            var json = Serialize(mapping);
            var request = new HttpRequestMessage(HttpMethod.Post, "mappings") { Content = new StringContent(json) };
            request.Content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");
            var response = await _client.SendAsync(request);
            await response.Content.ReadAsStringAsync();
        }

        public async Task ResetMappings()
        {
            await _client.DeleteAsync("mappings");
        }

        private static T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, SerializerSettings);
        }

        private static string Serialize(object toSerialize)
        {
            return JsonConvert.SerializeObject(toSerialize, SerializerSettings);
        }
    }
}
