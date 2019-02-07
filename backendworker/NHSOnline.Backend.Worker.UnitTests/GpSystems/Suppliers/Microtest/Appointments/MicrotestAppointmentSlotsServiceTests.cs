using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.GpSystems.Appointments.Models;
using NHSOnline.Backend.Worker.GpSystems.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Microtest;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Microtest.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Microtest.Models.Appointments;
using NHSOnline.Backend.Worker.Support.Temporal;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Microtest.Appointments
{
    [TestClass]
    public class MicrotestAppointmentSlotsServiceTests
    {   
        private IFixture _fixture;
        private Mock<IMicrotestClient> _mockMicrotestClient;
        private MicrotestUserSession _microtestUserSession;
        private AppointmentSlotsDateRange _dateRange;
        private DateTimeOffset _fromDateTimeOffset;
        private DateTimeOffset _toDateTimeOffset;
        private IAppointmentSlotsService _systemUnderTest;
        private Mock<IAppointmentSlotsResponseMapper> _mockResponseMapper;

        [TestInitialize]
        public void TestInitialize()
        {
            IConfigurationBuilder configBuilder = new ConfigurationBuilder();
            configBuilder.AddInMemoryCollection(new[] { new KeyValuePair<string, string>("TIMEZONE", TimeZoneResolver.GetTimeZoneNameForCurrentOperatingSystemPlatform()) });
            var timeZoneInfoProvider = new TimeZoneInfoProvider(new Mock<ILogger<TimeZoneInfoProvider>>().Object, configBuilder.Build());
            var dateTimeOffsetProvider = new DateTimeOffsetProvider(timeZoneInfoProvider);

            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _mockMicrotestClient = _fixture.Freeze<Mock<IMicrotestClient>>();
            _mockResponseMapper = _fixture.Freeze<Mock<IAppointmentSlotsResponseMapper>>();

            _microtestUserSession = new MicrotestUserSession
            {
                OdsCode = "TestOdsCode",
                NhsNumber = "TestNhsNumber",
            };
            
            _fromDateTimeOffset = dateTimeOffsetProvider.CreateDateTimeOffset();
            _toDateTimeOffset = dateTimeOffsetProvider.CreateDateTimeOffset();

            _dateRange = new AppointmentSlotsDateRange(dateTimeOffsetProvider)
            {
                FromDate = _fromDateTimeOffset,
                ToDate = _toDateTimeOffset
            };

            _systemUnderTest = new MicrotestAppointmentSlotsService(
                _mockMicrotestClient.Object,
                new LoggerFactory().CreateLogger<MicrotestAppointmentSlotsService>(),
                _mockResponseMapper.Object);
        }

        [TestMethod]
        public async Task GetSlots_VisionClientThrowsHttpRequestExceptionFromAppointmentSlots_ReturnsSupplierSystemUnavailable()
        {
            // Arrange
            _mockMicrotestClient
                .Setup(x => x.AppointmentSlotsGet(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<AppointmentSlotsDateRange>()))
                .Throws<HttpRequestException>()
                .Verifiable();

            // Act
            var result = await GetAppointmentSlotsResult();

            // Assert
            _mockMicrotestClient.Verify();
            result.Should().BeAssignableTo<AppointmentSlotsResult.SupplierSystemUnavailable>();
        }
        
        [TestMethod]
        public async Task GetSlots_MicrotestClientGetAppointmentSlotsUnsuccessful_ReturnsSupplierSystemUnavailable()
        {
            // Arrange
            var unsuccessfulSlotResponse = _fixture
                .Build<MicrotestClient.MicrotestApiObjectResponse<AppointmentSlotsGetResponse>>()
                .With(x => x.StatusCode, HttpStatusCode.InternalServerError)
                .With(x => x.Body, null)
                .Create();
            
            MockEmisClientAppointmentSlotGetMethod(unsuccessfulSlotResponse);

            // Act
            var result = await GetAppointmentSlotsResult();

            // Assert
            _mockMicrotestClient.Verify();
            result.Should().BeAssignableTo<AppointmentSlotsResult.SupplierSystemUnavailable>();
        }
        
        [TestMethod]
        public async Task GetSlots_ReturnsSupplierSystemUnavailable_WhenSomethingGoesWrongDuringMappingResponse()
        {
            // Arrange
            var slotResponse = new MicrotestClient.MicrotestApiObjectResponse<AppointmentSlotsGetResponse>(HttpStatusCode.OK)
            {
                Body = new AppointmentSlotsGetResponse(),
            };

            MockEmisClientAppointmentSlotGetMethod(slotResponse);

            _mockResponseMapper.Setup(x => x.Map(slotResponse.Body))
                .Throws<Exception>();

            // Act
            var result = await GetAppointmentSlotsResult();

            // Assert
            _mockMicrotestClient.Verify();
            result.Should().BeAssignableTo<AppointmentSlotsResult.InternalServerError>();
        }
        
        [TestMethod]
        public async Task GetSlots_HappyPath_ReturnsAppointmentSlots()
        {
            // Arrange
            var slotResponse = new MicrotestClient.MicrotestApiObjectResponse<AppointmentSlotsGetResponse>(HttpStatusCode.OK)
            {
                Body = new AppointmentSlotsGetResponse(),
            };
            
            MockEmisClientAppointmentSlotGetMethod(slotResponse);

            var expectedResponse = _fixture.Create<AppointmentSlotsResponse>();

            _mockResponseMapper.Setup(x => x.Map(slotResponse.Body))
                .Returns(expectedResponse);

            // Act
            var result = await GetAppointmentSlotsResult();

            // Assert
            _mockMicrotestClient.Verify();
            result.Should().BeAssignableTo<AppointmentSlotsResult.SuccessfullyRetrieved>();
        }

        private void MockEmisClientAppointmentSlotGetMethod(
            MicrotestClient.MicrotestApiObjectResponse<AppointmentSlotsGetResponse> response)
        {
            _mockMicrotestClient.Setup(x => x.AppointmentSlotsGet(
                _microtestUserSession.OdsCode,
                _microtestUserSession.NhsNumber,
                _dateRange)
            )
            .ReturnsAsync(response)
            .Verifiable();     
        }
        
        private async Task<AppointmentSlotsResult> GetAppointmentSlotsResult()
        {
            return await _systemUnderTest.GetSlots(_microtestUserSession, _dateRange);
        }
    }
}
