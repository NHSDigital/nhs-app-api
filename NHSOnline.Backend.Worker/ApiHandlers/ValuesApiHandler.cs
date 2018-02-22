using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NHSOnline.Backend.Worker.Controllers;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using NHSOnline.Backend.Worker.Interfaces;
using NHSOnline.Backend.Worker.Models;

namespace NHSOnline.Backend.Worker.ApiHandlers
{
    internal class ValuesApiHandler : IValuesApiHandler
    {
        private readonly string _stubsEndpointUrl;
        private readonly HttpClient _client;

        public ValuesApiHandler(IConfig config, IHttpClientFactory httpClientFactory)
        {
            _stubsEndpointUrl = config.StubsEndpointUrl;
            _client = httpClientFactory.Create();
        }

        public async Task<IActionResult> GetValues()
        {
            var stubResponse = await _client.GetStringAsync($"{_stubsEndpointUrl}/api/values");
            var stubValues = JsonConvert.DeserializeObject<IEnumerable<string>>(stubResponse)
                .Select(name => new Value {Name = name});

            var values = stubValues
                .Prepend(new Value {Name = "Backend Value 1"})
                .Append(new Value {Name = "Backend Value 2"});

            return new OkObjectResult(values);
        }
    }
}
