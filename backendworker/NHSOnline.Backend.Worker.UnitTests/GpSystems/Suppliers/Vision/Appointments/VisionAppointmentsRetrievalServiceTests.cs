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
using NHSOnline.Backend.Worker.GpSystems.SharedModels;
using NHSOnline.Backend.Worker.GpSystems.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Session;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Vision.Appointments
{
    [TestClass]
    public class VisionAppointmentsRetrievalServiceTests
    {
        private IFixture _fixture;
        private Mock<IBookedAppointmentsResponseMapper> _mockBookedAppointmentsResponseMapper;
        private Mock<IVisionClient> _mockVisionClient;
        private VisionUserSession _visionUserSession;
        private VisionAppointmentsRetrievalService _systemUnderTest;
        private VisionResponse<BookedAppointmentsResponse> _visionClientGetResponse;
        private ISessionCacheService _sessionCacheService;
        
        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            
            _visionUserSession = _fixture.Create<VisionUserSession>();
            _visionUserSession.IsAppointmentsEnabled = true;
            
            _mockVisionClient = _fixture.Freeze<Mock<IVisionClient>>();
            _visionClientGetResponse = _fixture.Create<VisionResponse<BookedAppointmentsResponse>>();
            _sessionCacheService = _fixture.Create<ISessionCacheService>();
            _visionUserSession.IsAppointmentsEnabled = true;

            var response = GetVisionResponse(_visionClientGetResponse);
            
            MockVisionClientAppointmentsGetMethod(response);
            
            _mockBookedAppointmentsResponseMapper = _fixture.Freeze<Mock<IBookedAppointmentsResponseMapper>>();
   
            _systemUnderTest = new VisionAppointmentsRetrievalService(
                _fixture.Create<ILogger<VisionAppointmentsRetrievalService>>(),
                _mockVisionClient.Object,
                _mockBookedAppointmentsResponseMapper.Object,
                _sessionCacheService);
        }
        
        [TestMethod]
        public async Task GetAppointments_HappyPath_ReturnsSuccessfullyRetrievedResponse()
        {
            // Arrange

            // Act
            var result = await _systemUnderTest.GetAppointments(_visionUserSession);

            // Assert
            var response = result.Should().BeAssignableTo<AppointmentsResult.SuccessfullyRetrieved>().Subject.Response;

            response.UpcomingAppointments.Should().BeEmpty();
            response.CancellationReasons.Should().BeEmpty();
            response.PastAppointmentsEnabled.Should().BeFalse();
            _mockVisionClient.VerifyAll();
        }

        [TestMethod]
        public async Task GetAppointments_WhenPatientDoesNotHaveNecessaryPermissions_ReturnsCannotBookAppointments()
        {
            // Arrange
            _visionUserSession.IsAppointmentsEnabled = false;
            
            // Act
            var result = await _systemUnderTest.GetAppointments(_visionUserSession);
            
            // Assert
            result.Should().BeAssignableTo<AppointmentsResult.CannotViewAppointments>();
        }

        [TestMethod]
        public async Task GetAppointments_VisionClientReturnsAccessDenied_ReturnsCannotBookAppointments()
        {
            // Arrange
            var response = VisionApiObjectResponseBuilder
                .BuildUnsuccessfulResponseWithErrorCode<BookedAppointmentsResponse>("-35");
            MockVisionClientAppointmentsGetMethod(response);
            
            // Act
            var result = await _systemUnderTest.GetAppointments(_visionUserSession);
            
            // Assert
            _mockVisionClient.Verify();
            result.Should().BeAssignableTo<AppointmentsResult.CannotViewAppointments>();
        }
        
        [TestMethod]
        public async Task GetAppointments_VisionClientThrows_ReturnsSupplierSystemUnavailable()
        {
            // Arrange
            _mockVisionClient.Setup(x => x.GetExistingAppointments(
                    _visionUserSession
                ))
                .Throws<HttpRequestException>()
                .Verifiable();

            // Act
            var result = await _systemUnderTest.GetAppointments(_visionUserSession);

            // Assert
            result.Should().BeAssignableTo<AppointmentsResult.SupplierSystemUnavailable>();
        }
 
        [TestMethod]
        public async Task GetAppointments_MapperThrows_ReturnsInternalServerError()
        {
            // Arrange
            _mockBookedAppointmentsResponseMapper.Setup(x=>x.Map(It.IsAny<BookedAppointmentsResponse>()))
            .Throws<Exception>();

            // Act
            var result = await _systemUnderTest.GetAppointments(_visionUserSession);

            // Assert
            result.Should().BeAssignableTo<AppointmentsResult.InternalServerError>();
        }

        [DataTestMethod]
        [DataRow(true, Necessity.Optional)]
        [DataRow(false, Necessity.NotAllowed)]
        public async Task GetAppointments_HappyPath_AppointmentBookingReasonNecessitySet(bool allowReason,
            Necessity expectedNecessity)
        {
            // Arrange
            _visionClientGetResponse.ServiceContent.Appointments.Settings.BookingReason.Add = allowReason;
            var visionResponse = GetVisionResponse(_visionClientGetResponse);

            MockVisionClientAppointmentsGetMethod(visionResponse);

            // Act
            var result = await _systemUnderTest.GetAppointments(_visionUserSession);

            // Assert
            result.Should().BeAssignableTo<AppointmentsResult.SuccessfullyRetrieved>();

            _visionUserSession.AppointmentBookingReasonNecessity.Should().Be(expectedNecessity);
        }

        private static VisionPFSClient.VisionApiObjectResponse<BookedAppointmentsResponse> GetVisionResponse(
            VisionResponse<BookedAppointmentsResponse> bookedAppointments)
        {
            return new VisionPFSClient.VisionApiObjectResponse<BookedAppointmentsResponse>(HttpStatusCode.OK)
            {
                RawResponse = new VisionResponseEnvelope<BookedAppointmentsResponse>
                {
                    Body = new VisionResponseBody<BookedAppointmentsResponse>
                    {
                        VisionResponse = bookedAppointments
                    }
                }
            };
        }

        private void MockVisionClientAppointmentsGetMethod(
            VisionPFSClient.VisionApiObjectResponse<BookedAppointmentsResponse> response)
        {   
            _mockVisionClient.Reset();
            _mockVisionClient.Setup(x => x.GetExistingAppointments(
                    _visionUserSession
                    ))
                .ReturnsAsync(response);
        }
    }
}