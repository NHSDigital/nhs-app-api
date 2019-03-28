using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using NHSOnline.Backend.Support.ResponseParsers;
using RichardSzalay.MockHttp;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Appointments;
using NHSOnline.Backend.Support;
using UnitTestHelper;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Microtest
{
    [TestClass]
    public sealed class MicrotestClientTests : IDisposable
    {
        public static readonly Uri BaseUri = new Uri("http://microtest_base_url/");

        private IMicrotestClient _systemUnderTest;
        private MockHttpMessageHandler _mockHttpHandler;
        private Mock<IMicrotestConfig> _configMock;
        private MicrotestHttpClient _httpClient;
        private IFixture _fixture;
        private string _nhsNumber;
        private string _odsCode;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _fixture.Register<IJsonResponseParser>(() => new JsonResponseParser());
             
            _mockHttpHandler = new MockHttpMessageHandler();
            
            _configMock = new Mock<IMicrotestConfig>();
            _configMock.SetupGet(x => x.BaseUrl).Returns(BaseUri);

            _httpClient = new MicrotestHttpClient(new HttpClient(_mockHttpHandler), _configMock.Object);

            _fixture.Inject(_configMock);
            _fixture.Inject(_httpClient);
            
            _odsCode = _fixture.Create<string>();
            _nhsNumber = _fixture.CreateNhsNumberFormatted();

            _systemUnderTest = _fixture.Create<MicrotestClient>();
        }
        
        [TestMethod]
        public async Task AppointmentSlotsGet_ReturnsAppointmentsResponse_WhenValidlyRequested()
        {
            // Arrange
            var fromDate = new DateTime(2000, 1, 1);
            var toDate = new DateTime(2000, 1, 2);

            var expectedResponse = _fixture.Create<AppointmentSlotsGetResponse>();

            MockMicrotestHttpRequest(HttpMethod.Get, 
                "patient/appointment-slots?fromDate=2000-01-01&toDate=2000-01-02", 
                JsonConvert.SerializeObject(expectedResponse));

            var dateRange = new AppointmentSlotsDateRange(fromDate, toDate);

            // Act
            var response = await _systemUnderTest.AppointmentSlotsGet(_odsCode, _nhsNumber, dateRange);

            // Assert
            response.Body.Should().BeEquivalentTo(expectedResponse);
            response.StatusCode.Should().Be(200);
        }

        [TestMethod]
        public async Task AppointmentSlotsGet_ReturnsInternalServerError_WhenResponseIsNotJson()
        {
            // Arrange
            var fromDate = new DateTime(2000, 1, 1);
            var toDate = new DateTime(2000, 1, 2);

            var nonJsonResponse = _fixture.Create<string>();

            MockMicrotestHttpRequest(HttpMethod.Get, 
                "patient/appointment-slots?fromDate=2000-01-01&toDate=2000-01-02", 
                nonJsonResponse);

            var dateRange = new AppointmentSlotsDateRange(fromDate, toDate);

            // Act
            var response = await _systemUnderTest.AppointmentSlotsGet(_odsCode, _nhsNumber, dateRange);

            // Assert
            response.StatusCode.Should().Be(500);
        }

        private void MockMicrotestHttpRequest(HttpMethod httpMethod, string path, string response)
        {
            var httpHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(MicrotestClient.HeaderNhsNumber, _nhsNumber.RemoveWhiteSpace()),
                new KeyValuePair<string, string>(MicrotestClient.HeaderOdsCode, _odsCode)
            };

            _mockHttpHandler
                .WhenMicrotest(httpMethod, path)
                .WithHeaders(httpHeaders)
                .Respond("application/json", response);
        }

        public void Dispose()
        {
            _mockHttpHandler.Dispose();
        }
    }
}
