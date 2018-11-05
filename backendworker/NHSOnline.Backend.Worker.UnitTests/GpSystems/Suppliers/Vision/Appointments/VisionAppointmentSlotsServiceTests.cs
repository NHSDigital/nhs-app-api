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
using NHSOnline.Backend.Worker.GpSystems.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Session;
using NHSOnline.Backend.Worker.Support.Temporal;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Vision.Appointments
{
    [TestClass]
    public class VisionAppointmentSlotsServiceTests
    {
        private IFixture _fixture;
        private Mock<IVisionClient> _mockVisionClient;
        private VisionUserSession _userSession;
        private VisionAppointmentSlotsService _systemUnderTest;
        private VisionResponse<AvailableAppointmentsResponse> _visionClientGetResponse;
        private AppointmentSlotsDateRange _dateRange;
        
        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _userSession = _fixture.Create<VisionUserSession>();
            _userSession.IsAppointmentsEnabled = true;
            _mockVisionClient = _fixture.Freeze<Mock<IVisionClient>>();
            _visionClientGetResponse = _fixture.Create<VisionResponse<AvailableAppointmentsResponse>>();
            _dateRange = _fixture.Create<AppointmentSlotsDateRange>();
            
            var response = new VisionClient.VisionApiObjectResponse<AvailableAppointmentsResponse>(HttpStatusCode.OK)
            {
                RawResponse = new VisionResponseEnvelope<AvailableAppointmentsResponse>
                {
                    Body = new VisionResponseBody<AvailableAppointmentsResponse>
                    {
                        VisionResponse = _visionClientGetResponse
                    }
                }
            };
            
            MockVisionClientAppointmentSlotsGetMethod(response);
            
            IConfigurationBuilder configBuilder = new ConfigurationBuilder();
            configBuilder.AddInMemoryCollection(new[] { new KeyValuePair<string, string>("TIMEZONE", TimeZoneResolver.GetTimeZoneNameForCurrentOS()) });
            var timeZoneInfoProvider = new TimeZoneInfoProvider(new Mock<ILogger<TimeZoneInfoProvider>>().Object, configBuilder.Build());
            var dateTimeOffsetProvider = new DateTimeOffsetProvider(timeZoneInfoProvider);
            
            var appointmentMapper = new AvailableAppointmentsMapper(dateTimeOffsetProvider);
            
            _systemUnderTest = new VisionAppointmentSlotsService(
                _mockVisionClient.Object,
                _fixture.Create<ILogger<VisionAppointmentSlotsService>>(),
                new AvailableAppointmentsResponseMapper(appointmentMapper));
        }
        
        [TestMethod]
        public async Task GetSlots_HappyPath_ReturnsSuccessfullyRetrievedResponse()
        {
            // Arrange

            // Act
            var result = await _systemUnderTest.GetSlots(_userSession, _dateRange);

            // Assert
            var response = result.Should().BeAssignableTo<AppointmentSlotsResult.SuccessfullyRetrieved>().Subject.Response;

            response.Slots.Should().BeEmpty();
            _mockVisionClient.VerifyAll();
        }

        [TestMethod]
        public async Task GetSlots_WhenPatientDoesNotHaveNecessaryPermissions_ReturnsCannotBookAppointments()
        {
            // Arrange
            _userSession.IsAppointmentsEnabled = false;
            
            // Act
            var result = await _systemUnderTest.GetSlots(_userSession, _dateRange);
            
            // Assert
            result.Should().BeAssignableTo<AppointmentSlotsResult.CannotBookAppointments>();
        }
        
        [TestMethod]
        public async Task GetSlots_VisionClientReturnsAccessDenied_ReturnsCannotBookAppointments()
        {
            // Arrange
            var response = VisionApiObjectResponseBuilder
                .BuildUnsuccessfulResponseWithErrorCode<AvailableAppointmentsResponse>("-35");
            MockVisionClientAppointmentSlotsGetMethod(response);

            // Act
            var result = await _systemUnderTest.GetSlots(_userSession, _dateRange);

            // Assert
            _mockVisionClient.Verify();
            result.Should().BeAssignableTo<AppointmentSlotsResult.CannotBookAppointments>();
        }
        
        [TestMethod]
        public async Task GetSlots_VisionClientThrows_ReturnsSupplierSystemUnavailable()
        {
            // Arrange
            _mockVisionClient.Setup(x => x.GetAvailableAppointments(
                    _userSession,
                    _dateRange
                ))
                .Throws<HttpRequestException>()
                .Verifiable();

            // Act
            var result = await _systemUnderTest.GetSlots(_userSession, _dateRange);

            // Assert
            result.Should().BeAssignableTo<AppointmentSlotsResult.SupplierSystemUnavailable>();
        }
        
        [TestMethod]
        public async Task GetSlots_MapperThrows_ReturnsInternalServerError()
        {
            // Arrange
            _mockVisionClient.Setup(x => x.GetAvailableAppointments(
                    _userSession,
                    _dateRange
                ))
                .Throws<Exception>();

            // Act
            var result = await _systemUnderTest.GetSlots(_userSession, _dateRange);

            // Assert
            result.Should().BeAssignableTo<AppointmentSlotsResult.InternalServerError>();
        }
        
        private void MockVisionClientAppointmentSlotsGetMethod(
            VisionClient.VisionApiObjectResponse<AvailableAppointmentsResponse> response)
        {   
            _mockVisionClient.Setup(x => x.GetAvailableAppointments(
                    _userSession,
                    _dateRange
                ))
                .ReturnsAsync(response);
        }
    }
}