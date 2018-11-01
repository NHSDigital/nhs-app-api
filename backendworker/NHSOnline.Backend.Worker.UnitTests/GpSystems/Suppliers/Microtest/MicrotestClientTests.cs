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
using NHSOnline.Backend.Worker.ResponseParsers;
using RichardSzalay.MockHttp;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Microtest;
using NHSOnline.Backend.Worker.GpSystems.Appointments;
using NHSOnline.Backend.Worker.Support.Temporal;
using NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Microtest.Models.Appointments;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Microtest
{
    [TestClass]
    public sealed class MicrotestClientTests : IDisposable
    {
        public static readonly Uri BaseUri = new Uri("http://microtest_base_url/");

        private IMicrotestClient _sut;
        private MockHttpMessageHandler _mockHttpHandler;
        private Mock<IMicrotestConfig> _configMock;
        private MicrotestHttpClient _httpClient;
        private IFixture _fixture;
        private Mock<IDateTimeOffsetProvider> _dateTimeOffsetProvider;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _fixture.Register<IJsonResponseParser>(() => new JsonResponseParser());
            
            _mockHttpHandler = new MockHttpMessageHandler();
            
            _configMock = new Mock<IMicrotestConfig>();
            _configMock.SetupGet(x => x.BaseUrl).Returns(BaseUri);

            _httpClient = new MicrotestHttpClient(new HttpClient(_mockHttpHandler), _configMock.Object);

            _dateTimeOffsetProvider = new Mock<IDateTimeOffsetProvider>();

            _fixture.Inject(_configMock);
            _fixture.Inject(_httpClient);

            _sut = _fixture.Create<MicrotestClient>();
        }
        
        [TestMethod]
        public async Task AppointmentSlotsGet_ReturnsAppointmentsResponse_WhenValidlyRequested()
        {
            var odsCode = _fixture.Create<string>();
            var nhsNumber = _fixture.Create<string>();
            var fromDate = new DateTime(2000, 1, 1);
            var toDate = new DateTime(2000, 1, 2);

            var expectedResponse = _fixture.Create<AppointmentSlotsGetResponse>();

            var additionalHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(MicrotestClient.HeaderNhsNumber, nhsNumber),
                new KeyValuePair<string, string>(MicrotestClient.HeaderOdsCode, odsCode),
            };
            
            _mockHttpHandler
                .WhenMicrotest(HttpMethod.Get,
                    $"patient/appointment-slots?fromDate=2000-01-01&toDate=2000-01-02")
                .WithHeaders(additionalHeaders)
                .Respond("application/json", JsonConvert.SerializeObject(expectedResponse));

            var dateRange = new AppointmentSlotsDateRange(_dateTimeOffsetProvider.Object)
            {
                FromDate = fromDate,
                ToDate = toDate
            };

            var response = await _sut.AppointmentSlotsGet(odsCode, nhsNumber, dateRange);

            response.Body.Should().BeEquivalentTo(expectedResponse);
            response.StatusCode.Should().Be(200);
        }

        public void Dispose()
        {
            _mockHttpHandler.Dispose();
        }
    }
}
