using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using NHSOnline.Backend.Worker.Suppliers.Emis;
using NHSOnline.Backend.Worker.Suppliers.Emis.Models;
using NHSOnline.Backend.Worker.UnitTests.Suppliers.Emis.Helpers;
using RichardSzalay.MockHttp;

namespace NHSOnline.Backend.Worker.UnitTests.Suppliers.Emis
{
    [TestClass]
    public class EmisClientTests
    {
        public const string DefaultEmisVersion = "2.1.0.0";
        public const string DefaultEmisApplicationId = "D66BA979-60D2-49AA-BE82-AEC06356E41F";

        public static readonly Uri BaseUri = new Uri("http://185.13.72.81/PFS/");
        private IEmisClient _emisClient;
        private MockHttpMessageHandler _mockHttpHandler;
        private Mock<IEmisConfig> _configMock;
        private HttpClient _httpClient;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockHttpHandler = new MockHttpMessageHandler();

            _configMock = new Mock<IEmisConfig>();
            _configMock.SetupGet(x => x.BaseUrl).Returns(BaseUri);
            _configMock.SetupGet(x => x.Version).Returns(DefaultEmisVersion);
            _configMock.SetupGet(x => x.ApplicationId).Returns(DefaultEmisApplicationId);
            _httpClient = new HttpClient(_mockHttpHandler);
            _emisClient = new EmisClient(_httpClient, _configMock.Object);
        }

        [TestMethod]
        public async Task EndUserSessionAsync_ReturnsAnEndUserSessionId_WhenValidlyRequested()
        {
            const string expected = "DW3EUerDy8VEZi2gvJ5esg";
            var endUserSessionResponse = new CreateEndUserSessionResponseModel
            {
                EndUserSessionId = expected
            };

            _mockHttpHandler
                .WhenEmis(HttpMethod.Post, "sessions/endusersession")
                .WithEmisHeaders()
                .Respond("application/json", JsonConvert.SerializeObject(endUserSessionResponse));

            var response = await _emisClient.EndUserSessionAsync();

            Assert.AreEqual(expected, response.EndUserSessionId);
        }

        [TestMethod]
        public async Task SessionsAsync_ReturnsASessionResponse_WhenValidlyRequested()
        {
            const string endUserSessionId = "end user session id";
            const string connectionToken = "connection token";
            const string odsCode = "ods code";
            var expectedResponse = new CreateSessionResponseModel
            {
                SessionId = "foo",
                UserPatientLinks = new []
                {
                    new UserPatientLinkModel { UserPatientLinkToken = "link1" },
                }
            };

            var expectedContent = new CreateSessionRequestModel
            {
                AccessIdentityGuid = connectionToken,
                NationalPracticeCode = odsCode
            };

            var additionalHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(EmisClient.HeaderEndUserSessionId, endUserSessionId)
            };

            _mockHttpHandler
                .WhenEmis(HttpMethod.Post, "sessions")
                .WithEmisHeaders(additionalHeaders)
                .WithContent(JsonConvert.SerializeObject(expectedContent))
                .Respond("application/json", JsonConvert.SerializeObject(expectedResponse));

            var response = await _emisClient.SessionsAsync(endUserSessionId, connectionToken, odsCode);
            var expectedJson = JsonConvert.SerializeObject(expectedResponse);
            var actualJson = JsonConvert.SerializeObject(response);
            Assert.AreEqual(expectedJson, actualJson);
        }

        [TestMethod]
        public async Task DemographicsAsync_ReturnsADemographicsResponse_WhenValidlyRequested()
        {
            const string nhsNumber = "AB1234";
            var userPatientLinkToken = "user link token";
            var sessionId = "session id";
            var endUserSessionId = "end user session id";
            var expectedResponse = new DemographicsResponse
            {
                PatientIdentifiers = new[] { new PatientIdentifier { IdentifierValue = nhsNumber }}
            };

            var additionalHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(EmisClient.HeaderEndUserSessionId, endUserSessionId),
                new KeyValuePair<string, string>(EmisClient.HeaderSessionId, sessionId),
                new KeyValuePair<string, string>(EmisClient.HeaderUserPatientLinkToken, userPatientLinkToken)
            };

            _mockHttpHandler
                .WhenEmis(HttpMethod.Get, "demographics")
                .WithEmisHeaders(additionalHeaders)
                .Respond("application/json", JsonConvert.SerializeObject(expectedResponse));

            var response = await _emisClient.DemographicsAsync(userPatientLinkToken, sessionId, endUserSessionId);

            var expectedJson = JsonConvert.SerializeObject(expectedResponse);
            var actualJson = JsonConvert.SerializeObject(response);
            Assert.AreEqual(expectedJson, actualJson);
        }
    }
}
