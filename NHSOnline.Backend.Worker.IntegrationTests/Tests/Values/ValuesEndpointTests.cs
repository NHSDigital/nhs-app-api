using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using NHSOnline.Backend.Worker.IntegrationTests.Delegates.Values;
using NHSOnline.Backend.Worker.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using WireMock.Server;
using WireMock.Settings;

namespace NHSOnline.Backend.Worker.IntegrationTests.Tests.Values
{
    [TestClass]
    public class ValuesEndpointTests
    {
        private static TestServer _server;
        private static HttpClient _client;
        private static string _stubsEndpointUrl = "http://localhost:32768";
        private static FluentMockServer _stubMockServer;
        private static IWebHostBuilder _webHostBuilder;

        [ClassInitialize]
        public static void Setup(TestContext testContext)
        {
            _webHostBuilder =
                new WebHostBuilder()
                    .UseKestrel()
                    .UseStartup<Startup>()
                    .UseSetting("STUBS_ENDPOINT_URL", _stubsEndpointUrl);

            _stubMockServer = FluentMockServer.Start(new FluentMockServerSettings
            {
                Urls = new[] { _stubsEndpointUrl }
            });

            _server = new TestServer(_webHostBuilder);
            _client = _server.CreateClient();
        }

        [ClassCleanup]
        public static void CleanUp()
        {
            _stubMockServer.Stop();
        }

        [TestInitialize]
        public void ResetMockServer()
        {
            _stubMockServer.Reset();
        }

        [TestMethod]
        public async Task When_EmisValuesReturnsCorrectResponse_Then_BackendWorkedResponseIsCorrect()
        {
            ValuesEndpointDelegate.RespondsWithCorrectValues(_stubMockServer);

            var response = await _client.GetAsync("/values");
            var results = response.Content.ReadAsStringAsync().Result;
            var valuesList = JsonConvert.DeserializeObject<IEnumerable<Value>>(results);
            var firstValueExists = valuesList.Any(v => v.Name.Equals("Stubbed Value 1"));
            var secondValueExists = valuesList.Any(v => v.Name.Equals("Stubbed Value 2"));

            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK, "Backend Worked did not respond with correct response.");
            Assert.IsTrue(firstValueExists);
            Assert.IsTrue(secondValueExists);
        }
    }
}