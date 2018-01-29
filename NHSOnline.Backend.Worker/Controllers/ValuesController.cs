using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace NHSOnline.Backend.Worker.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private const string StubUrl = "http://nhsonline.stubs.emis.im1/api/values";
        private static readonly HttpClient HttpClient = new HttpClient();

        [HttpGet]
        public async Task<IEnumerable<string>> Get()
        {
            var stubResponse = await HttpClient.GetStringAsync(StubUrl);
            var stubValues = JsonConvert.DeserializeObject<IEnumerable<string>>(stubResponse);

            return stubValues
                .Prepend("Backend Value 1")
                .Append("Backend Value 2");
        }
    }
}
