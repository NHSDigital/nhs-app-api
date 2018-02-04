using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NHSOnline.Backend.Worker.Controllers;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace NHSOnline.Backend.Worker.ApiHandlers
{
    internal class ValuesApiHandler : IValuesApiHandler
    {
        private const string StubUrl = "http://nhsonline.stubs.emis.im1/api/values";
        private static readonly HttpClient HttpClient = new HttpClient();

        public async Task<IActionResult> ValuesGet()
        {
            var stubResponse = await HttpClient.GetStringAsync(StubUrl);
            var stubValues = JsonConvert.DeserializeObject<IEnumerable<string>>(stubResponse);

            var values = stubValues
                .Prepend("Backend Value 1 (Generated)")
                .Append("Backend Value 2 (Generated)");

            return new OkObjectResult(values);
        }
    }
}
