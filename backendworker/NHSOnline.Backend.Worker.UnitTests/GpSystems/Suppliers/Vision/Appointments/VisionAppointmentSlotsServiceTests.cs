using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.GpSystems.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Session;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Vision.Appointments
{
    [TestClass]
    public class VisionAppointmentSlotsServiceTests
    {
        private IFixture _fixture;
        private Mock<IVisionClient> _mockVisionClient;
        private VisionUserSession _visionUserSession;
        private UserSession _userSession;
        private VisionAppointmentSlotsService _systemUnderTest;
        private VisionResponse<AvailableAppointmentsResponse> _visionClientSlotsResponse;
        private VisionResponse<PatientConfigurationResponse> _visionClientConfigResponse;
        private AppointmentSlotsDateRange _dateRange;
        private Mock<IAvailableAppointmentsResponseMapper> _mockAppointmentsMapper;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            
            _visionUserSession = _fixture.Create<VisionUserSession>();
            _visionUserSession.IsAppointmentsEnabled = true;
            
            _fixture.Customize<UserSession>(c => c
                .With(u => u.GpUserSession, _visionUserSession));
            
            _userSession = _fixture.Create<UserSession>();
            
            _mockVisionClient = _fixture.Freeze<Mock<IVisionClient>>();
            _visionClientSlotsResponse = _fixture.Create<VisionResponse<AvailableAppointmentsResponse>>();
            _visionClientConfigResponse = _fixture.Create<VisionResponse<PatientConfigurationResponse>>();

            _dateRange = _fixture.Create<AppointmentSlotsDateRange>();
            
            var slotsResponse = new VisionPFSClient.VisionApiObjectResponse<AvailableAppointmentsResponse>(HttpStatusCode.OK)
            {
                RawResponse = new VisionResponseEnvelope<AvailableAppointmentsResponse>
                {
                    Body = new VisionResponseBody<AvailableAppointmentsResponse>
                    {
                        VisionResponse = _visionClientSlotsResponse
                    }
                }
            };
            
            MockVisionClientAppointmentSlotsGetMethod(slotsResponse);

            var configResponse = new VisionPFSClient.VisionApiObjectResponse<PatientConfigurationResponse>(HttpStatusCode.OK)
            {
                RawResponse = new VisionResponseEnvelope<PatientConfigurationResponse>
                {
                    Body = new VisionResponseBody<PatientConfigurationResponse>
                    {
                        VisionResponse = _visionClientConfigResponse
                    }
                }
            };

            MockVisionClientConfigurationGetMethod(configResponse);

            _mockAppointmentsMapper = _fixture.Freeze<Mock<IAvailableAppointmentsResponseMapper>>();
            
            _systemUnderTest = new VisionAppointmentSlotsService(
                _mockVisionClient.Object,
                _fixture.Create<ILogger<VisionAppointmentSlotsService>>(),
                _mockAppointmentsMapper.Object);
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
            _visionUserSession.IsAppointmentsEnabled = false;
            
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
                    _visionUserSession,
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
            _mockAppointmentsMapper.Setup(x => x.Map(It.IsAny<AvailableAppointmentsResponse>(),
                    It.IsAny<PatientConfigurationResponse>(),
                    _visionUserSession))
                .Throws<Exception>();

            // Act
            var result = await _systemUnderTest.GetSlots(_userSession, _dateRange);

            // Assert
            result.Should().BeAssignableTo<AppointmentSlotsResult.InternalServerError>();
        }
        
        private void MockVisionClientAppointmentSlotsGetMethod(
            VisionPFSClient.VisionApiObjectResponse<AvailableAppointmentsResponse> response)
        {   
            _mockVisionClient.Setup(x => x.GetAvailableAppointments(
                    _visionUserSession,
                    _dateRange
                ))
                .ReturnsAsync(response);
        }

        private void MockVisionClientConfigurationGetMethod(
            VisionPFSClient.VisionApiObjectResponse<PatientConfigurationResponse> response)
        {
            _mockVisionClient.Setup(x => x.GetConfiguration(
                    _visionUserSession
                ))
                .ReturnsAsync(response);
        }
    }
}