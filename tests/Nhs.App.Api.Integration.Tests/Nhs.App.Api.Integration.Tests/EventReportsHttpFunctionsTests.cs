using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Nhs.App.Api.Integration.Tests.Models;

namespace Nhs.App.Api.Integration.Tests
{
    [TestClass]
    public class EventReportsHttpFunctionsTests : CommunicationHttpFunctionBase
    {
        private static TestConfiguration _testConfiguration;

        [ClassInitialize]
        public static void ClassInitialise(TestContext context)
        {
            _testConfiguration = new TestConfiguration(context);

            TestClassSetup(_testConfiguration);
        }
        
        [DataTestMethod]
        public async Task EventReportTest_Post_Returns404NotFound()
        {
            // Arrange
            using var httpClient = CreateHttpClient();

            // Act
            var response = await httpClient.PostAsync(
                "communication/event-report/test",
                new StringContent(JsonConvert.SerializeObject(new EventReportCreateRequest
                {
                    SupplierId = "SupplierId",
                    ReportDate = DateTime.Today
                }))
            );

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
