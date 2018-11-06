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
    public class VisionAppointmentsRetrievalServiceTests
    {
        private IFixture _fixture;
        private Mock<IBookedAppointmentsResponseMapper> _mockBookedAppointmentsResponseMapper;
        private Mock<IVisionClient> _mockVisionClient;
        private VisionUserSession _userSession;
        private VisionAppointmentsRetrievalService _systemUnderTest;
        private VisionResponse<BookedAppointmentsResponse> _visionClientGetResponse;
        
        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _userSession = _fixture.Create<VisionUserSession>();
            _userSession.IsAppointmentsEnabled = true;
            _mockVisionClient = _fixture.Freeze<Mock<IVisionClient>>();
            _visionClientGetResponse = _fixture.Create<VisionResponse<BookedAppointmentsResponse>>();

            _userSession.IsAppointmentsEnabled = true;
            
            var response = new VisionClient.VisionApiObjectResponse<BookedAppointmentsResponse>(HttpStatusCode.OK)
            {
                RawResponse = new VisionResponseEnvelope<BookedAppointmentsResponse>
                {
                    Body = new VisionResponseBody<BookedAppointmentsResponse>
                    {
                        VisionResponse = _visionClientGetResponse
                    }
                }
            };
            
            MockVisionClientAppointmentsGetMethod(response);
            
            _mockBookedAppointmentsResponseMapper = _fixture.Freeze<Mock<IBookedAppointmentsResponseMapper>>();
   
            _systemUnderTest = new VisionAppointmentsRetrievalService(
                _fixture.Create<ILogger<VisionAppointmentsRetrievalService>>(),
                _mockVisionClient.Object,
                _mockBookedAppointmentsResponseMapper.Object);
        }
        
        [TestMethod]
        public async Task GetAppointments_HappyPath_ReturnsSuccessfullyRetrievedResponse()
        {
            // Arrange

            // Act
            var result = await _systemUnderTest.GetAppointments(_userSession);

            // Assert
            var response = result.Should().BeAssignableTo<AppointmentsResult.SuccessfullyRetrieved>().Subject.Response;

            response.Appointments.Should().BeEmpty();
            response.CancellationReasons.Should().BeEmpty();
            _mockVisionClient.VerifyAll();
        }

        [TestMethod]
        public async Task GetAppointments_WhenPatientDoesNotHaveNecessaryPermissions_ReturnsCannotBookAppointments()
        {
            // Arrange
            _userSession.IsAppointmentsEnabled = false;
            
            // Act
            var result = await _systemUnderTest.GetAppointments(_userSession);
            
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
            var result = await _systemUnderTest.GetAppointments(_userSession);
            
            // Assert
            _mockVisionClient.Verify();
            result.Should().BeAssignableTo<AppointmentsResult.CannotViewAppointments>();
        }
        
        [TestMethod]
        public async Task GetAppointments_VisionClientThrows_ReturnsSupplierSystemUnavailable()
        {
            // Arrange
            _mockVisionClient.Setup(x => x.GetExistingAppointments(
                    _userSession
                ))
                .Throws<HttpRequestException>()
                .Verifiable();

            // Act
            var result = await _systemUnderTest.GetAppointments(_userSession);

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
            var result = await _systemUnderTest.GetAppointments(_userSession);

            // Assert
            result.Should().BeAssignableTo<AppointmentsResult.InternalServerError>();
        }

        private void MockVisionClientAppointmentsGetMethod(
            VisionClient.VisionApiObjectResponse<BookedAppointmentsResponse> response)
        {   
            _mockVisionClient.Setup(x => x.GetExistingAppointments(
                    _userSession
                    ))
                .ReturnsAsync(response);
        }
    }
}