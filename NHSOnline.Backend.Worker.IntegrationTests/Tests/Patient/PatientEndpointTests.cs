using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using NHSOnline.Backend.Worker.Models.Patient;

namespace NHSOnline.Backend.Worker.IntegrationTests.Tests.Patient
{
    [TestClass]
    public class PatientEndpointTests
    {
        public const string HeaderConnectionToken = "NHSO-Connection-Token";
        public const string HeaderOdsCode = "NHSO-ODS-Code";
        public const string EmisApplicationId = "D66BA979-60D2-49AA-BE82-AEC06356E41F";
        public const string EmisVersion = "2.1.0.0";
        public const string EmisBaseUrl = "http://127.0.0.1:8800/emis/";

        private static TestServer _server;
        private static HttpClient _client;
        private static IWebHostBuilder _webHostBuilder;

        [ClassInitialize]
        public static void Setup(TestContext testContext)
        {
            _webHostBuilder =
                new WebHostBuilder()
                    .UseKestrel()
                    .UseStartup<Startup>()
                    .UseSetting("EMIS_APPLICATION_ID", EmisApplicationId)
                    .UseSetting("EMIS_VERSION", EmisVersion)
                    .UseSetting("EMIS_BASE_URL", EmisBaseUrl)
                    .UseSetting("REDIS_ODSLOOKUP_CONFIG", "127.0.0.1:6379");

            _server = new TestServer(_webHostBuilder);
            _client = _server.CreateClient();
        }

        [ClassCleanup]
        public static void CleanUp()
        {
        }

        [TestInitialize]
        public void ResetMockServer()
        {
        }

        [TestMethod]
        public async Task When_PatientIm1ConnectionReturnsCorrectResponse_Then_BackendWorkedResponseIsCorrect()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "patient/im1connection");
            request.Headers.Add(HeaderConnectionToken, new[] { "LUKE_SKYWALKER" });
            request.Headers.Add(HeaderOdsCode, new[] { "Rebels" });

            var response = await _client.SendAsync(request);
            var results = response.Content.ReadAsStringAsync().Result;
            var patientIm1ConnectionResponse = JsonConvert.DeserializeObject<PatientIm1ConnectionResponse>(results);
            var nhsNumbers = patientIm1ConnectionResponse.NhsNumbers.ToArray();
            var nhsNumberExists = nhsNumbers.Any(v => v.NhsNumber.Equals("9434765919"));

            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK, "Backend Worked did not respond with correct response.");
            Assert.IsTrue(nhsNumberExists);
        }
    }
}
