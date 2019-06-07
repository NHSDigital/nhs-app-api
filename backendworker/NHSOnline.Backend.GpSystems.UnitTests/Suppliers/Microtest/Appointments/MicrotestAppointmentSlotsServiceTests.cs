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
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Demographics;
using NHSOnline.Backend.Support.Temporal;
using UnitTestHelper;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Microtest.Appointments
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
        private Mock<ICurrentDateTimeProvider> _mockCurrentDateTimeProvider;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            
            _mockCurrentDateTimeProvider = _fixture.Freeze<Mock<ICurrentDateTimeProvider>>();
            _mockCurrentDateTimeProvider.SetupGet(x => x.UtcNow)
                .Returns(DateTime.UtcNow);
            
            IConfigurationBuilder configBuilder = new ConfigurationBuilder();
            configBuilder.AddInMemoryCollection(new[] { new KeyValuePair<string, string>("TIMEZONE", TimeZoneResolver.GetTimeZoneNameForCurrentOperatingSystemPlatform()) });
            var timeZoneInfoProvider = new TimeZoneInfoProvider(new Mock<ILogger<TimeZoneInfoProvider>>().Object, configBuilder.Build());
            var dateTimeOffsetProvider = new DateTimeOffsetProvider(timeZoneInfoProvider, _mockCurrentDateTimeProvider.Object);

            _mockMicrotestClient = _fixture.Freeze<Mock<IMicrotestClient>>();
            _mockResponseMapper = _fixture.Freeze<Mock<IAppointmentSlotsResponseMapper>>();

            _microtestUserSession = new MicrotestUserSession
            {
                OdsCode = "TestOdsCode",
                NhsNumber = "TestNhsNumber"
            };
            
            _fromDateTimeOffset = dateTimeOffsetProvider.CreateDateTimeOffset();
            _toDateTimeOffset = dateTimeOffsetProvider.CreateDateTimeOffset();

            _dateRange = new AppointmentSlotsDateRange(_fromDateTimeOffset,
                _toDateTimeOffset);

            _systemUnderTest = new MicrotestAppointmentSlotsService(
                _mockMicrotestClient.Object,
                new LoggerFactory().CreateLogger<MicrotestAppointmentSlotsService>(),
                _mockResponseMapper.Object);
        }

        [TestMethod]
        public async Task GetSlots_MicrotestClientThrowsHttpRequestExceptionFromAppointmentSlots_ReturnsBadGateway()
        {
            // Arrange
            var demographicsResponse = new MicrotestClient.MicrotestApiObjectResponse<DemographicsGetResponse>(HttpStatusCode.OK)
            {
                Body = new DemographicsGetResponse()
            };
            
            MockMicrotestClientDemographicsGetMethod(demographicsResponse);
            
            _mockMicrotestClient
                .Setup(x => x.AppointmentSlotsGet(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<AppointmentSlotsDateRange>()))
                .Throws<HttpRequestException>()
                .Verifiable();

            // Act
            var result = await GetAppointmentSlotsResult();

            // Assert
            _mockMicrotestClient.Verify();
            result.Should().BeAssignableTo<AppointmentSlotsResult.BadGateway>();
        }
        
        [TestMethod]
        public async Task GetSlots_MicrotestClientGetAppointmentSlotsUnsuccessful_ReturnsBadGateway()
        {
            // Arrange
            var demographicsResponse = new MicrotestClient.MicrotestApiObjectResponse<DemographicsGetResponse>(HttpStatusCode.OK)
            {
                Body = new DemographicsGetResponse()
            };
            
            MockMicrotestClientDemographicsGetMethod(demographicsResponse);
            
            var unsuccessfulSlotResponse = _fixture
                .Build<MicrotestClient.MicrotestApiObjectResponse<AppointmentSlotsGetResponse>>()
                .With(x => x.StatusCode, HttpStatusCode.InternalServerError)
                .With(x => x.Body, () => null)
                .Create();
            
            MockMicrotestClientAppointmentSlotGetMethod(unsuccessfulSlotResponse);

            // Act
            var result = await GetAppointmentSlotsResult();

            // Assert
            _mockMicrotestClient.Verify();
            result.Should().BeAssignableTo<AppointmentSlotsResult.BadGateway>();
        }
        
        [TestMethod]
        public async Task GetSlots_MicrotestClientGetAppointmentSlotsReturnsForbidden_ReturnsForbiddenResponse()
        {
            // Arrange
            var demographicsResponse = new MicrotestClient.MicrotestApiObjectResponse<DemographicsGetResponse>(HttpStatusCode.OK)
            {
                Body = new DemographicsGetResponse()
            };
            
            MockMicrotestClientDemographicsGetMethod(demographicsResponse);
            
            var forbiddenSlotsResponse = _fixture
                .Build<MicrotestClient.MicrotestApiObjectResponse<AppointmentSlotsGetResponse>>()
                .With(x => x.StatusCode, HttpStatusCode.Forbidden)
                .With(x => x.Body, () => null)
                .Create();
            
            MockMicrotestClientAppointmentSlotGetMethod(forbiddenSlotsResponse);

            // Act
            var result = await GetAppointmentSlotsResult();

            // Assert
            _mockMicrotestClient.Verify();
            result.Should().BeAssignableTo<AppointmentSlotsResult.Forbidden>();
        }
        
        [TestMethod]
        public async Task GetSlots_ReturnsInternalServerError_WhenSomethingGoesWrongDuringMappingResponse()
        {
            // Arrange
            var demographicsResponse = new MicrotestClient.MicrotestApiObjectResponse<DemographicsGetResponse>(HttpStatusCode.OK)
            {
                Body = new DemographicsGetResponse()
            };
            
            MockMicrotestClientDemographicsGetMethod(demographicsResponse);

            var slotResponse = new MicrotestClient.MicrotestApiObjectResponse<AppointmentSlotsGetResponse>(HttpStatusCode.OK)
            {
                Body = new AppointmentSlotsGetResponse(),
            };

            MockMicrotestClientAppointmentSlotGetMethod(slotResponse);

            _mockResponseMapper.Setup(x => x.Map(slotResponse.Body, demographicsResponse.Body))
                .Throws<Exception>();

            // Act
            var result = await GetAppointmentSlotsResult();

            // Assert
            _mockMicrotestClient.Verify();
            result.Should().BeAssignableTo<AppointmentSlotsResult.InternalServerError>();
        }

        [TestMethod]
        public async Task GetSlots_ExceptionRetrievingDemographicsData_MapperInvokedAnyway()
        {
            // Arrange
            _mockMicrotestClient
                .Setup(x => x.DemographicsGet(It.IsAny<string>(), It.IsAny<string>()))
                .Throws<Exception>()
                .Verifiable();
            
            var slotResponse = new MicrotestClient.MicrotestApiObjectResponse<AppointmentSlotsGetResponse>(HttpStatusCode.OK)
            {
                Body = new AppointmentSlotsGetResponse()
            };
            
            MockMicrotestClientAppointmentSlotGetMethod(slotResponse);

            var expectedResponse = _fixture.Create<AppointmentSlotsResponse>();

            _mockResponseMapper.Setup(x => x.Map(slotResponse.Body, null))
                .Returns(expectedResponse);

            // Act
            var result = await GetAppointmentSlotsResult();

            // Assert
            _mockMicrotestClient.Verify();
            result.Should().BeAssignableTo<AppointmentSlotsResult.Success>(); 
        }
        
        [TestMethod]
        public async Task GetSlots_HappyPath_ReturnsAppointmentSlots()
        {
            // Arrange
            var demographicsResponse = new MicrotestClient.MicrotestApiObjectResponse<DemographicsGetResponse>(HttpStatusCode.OK)
            {
                Body = new DemographicsGetResponse()
            };
            
            MockMicrotestClientDemographicsGetMethod(demographicsResponse);
            
            var slotResponse = new MicrotestClient.MicrotestApiObjectResponse<AppointmentSlotsGetResponse>(HttpStatusCode.OK)
            {
                Body = new AppointmentSlotsGetResponse()
            };
            
            MockMicrotestClientAppointmentSlotGetMethod(slotResponse);

            var expectedResponse = _fixture.Create<AppointmentSlotsResponse>();

            _mockResponseMapper.Setup(x => x.Map(slotResponse.Body, demographicsResponse.Body))
                .Returns(expectedResponse);

            // Act
            var result = await GetAppointmentSlotsResult();

            // Assert
            _mockMicrotestClient.Verify();
            result.Should().BeAssignableTo<AppointmentSlotsResult.Success>();
        }

        private void MockMicrotestClientAppointmentSlotGetMethod(
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
        
        private void MockMicrotestClientDemographicsGetMethod(
            MicrotestClient.MicrotestApiObjectResponse<DemographicsGetResponse> response)
        {
            _mockMicrotestClient.Setup(x => x.DemographicsGet(
                _microtestUserSession.OdsCode,
                _microtestUserSession.NhsNumber)
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
