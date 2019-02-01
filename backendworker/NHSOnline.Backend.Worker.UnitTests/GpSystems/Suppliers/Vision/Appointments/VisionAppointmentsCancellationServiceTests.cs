using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.GpSystems.Appointments.Models;
using NHSOnline.Backend.Worker.GpSystems.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Session;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Vision.Appointments
{
    [TestClass]
    public class VisionAppointmentsCancellationServiceTests
    {
        private IFixture _fixture;
        private VisionUserSession _userSession;
        private Mock<IVisionClient> _mockVisionClient;
        private VisionAppointmentsCancellationService _systemUnderTest;
        private AppointmentCancelRequest _request;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _userSession = _fixture.Create<VisionUserSession>();
            _userSession.IsAppointmentsEnabled = true;

            _mockVisionClient = _fixture.Freeze<Mock<IVisionClient>>();

            _systemUnderTest = _fixture.Create<VisionAppointmentsCancellationService>();

            _request = _fixture.Create<AppointmentCancelRequest>(); 
        }
        
        [TestMethod]
        public async Task Cancel_HappyPath_ReturnsSuccessfullyCancelledResponse()
        {
            // Arrange         
            var response = new VisionPFSClient.VisionApiObjectResponse<CancelledAppointmentResponse>(HttpStatusCode.OK)
            {
                RawResponse = new VisionResponseEnvelope<CancelledAppointmentResponse>
                {
                    Body = new VisionResponseBody<CancelledAppointmentResponse>
                    {
                        VisionResponse = _fixture.Create<VisionResponse<CancelledAppointmentResponse>>()
                    }
                }
            };

            MockVisionClientCancelAppointmentMethod(response);

            // Act
            var result = await _systemUnderTest.Cancel(_userSession, _request);

            // Assert
            _mockVisionClient.Verify();
            result.Should().BeAssignableTo<AppointmentCancelResult.SuccessfullyCancelled>();
        }
        
        [TestMethod]
        public async Task Cancel_VisionClientThrowsHttpRequestException_ReturnsSupplierSystemUnavailable()
        {
            // Arrange
            _mockVisionClient.Setup(x => x.CancelAppointment(
                It.Is<VisionUserSession>(p => 
                    p.RosuAccountId.Equals(_userSession.RosuAccountId, StringComparison.Ordinal)
                    && p.ApiKey.Equals(_userSession.ApiKey, StringComparison.Ordinal)
                    && p.OdsCode.Equals(_userSession.OdsCode, StringComparison.Ordinal)),
                It.Is<CancelAppointmentRequest>(p => 
                    p.PatientId.Equals(_userSession.PatientId, StringComparison.Ordinal)
                    && p.ReasonId.Equals(_request.CancellationReasonId, StringComparison.Ordinal)
                    && p.SlotId.Equals(_request.AppointmentId, StringComparison.Ordinal))
            ))
            .Throws<HttpRequestException>()
            .Verifiable();

            // Act            
            var result = await _systemUnderTest.Cancel(_userSession, _request);

            // Assert
            _mockVisionClient.Verify();
            result.Should().BeAssignableTo<AppointmentCancelResult.SupplierSystemUnavailable>();
        }
        
        [TestMethod]
        public async Task Cancel_AppointmentsDisabled_ReturnsInsufficientPermissions()
        {
            // Arrange
            _userSession.IsAppointmentsEnabled = false;

            // Act            
            var result = await _systemUnderTest.Cancel(_userSession, _request);
            
            // Assert
            result.Should().BeAssignableTo<AppointmentCancelResult.InsufficientPermissions>();
        }

        [TestMethod]
        public async Task Cancel_VisionClientReturnsAppointmentSlotNotBookedToCurrentUserError_ReturnsAppointmentNotCancellable()
        {
            // Arrange
            var response = VisionApiObjectResponseBuilder
                .BuildUnsuccessfulResponseWithErrorCode<CancelledAppointmentResponse>("-100");
            MockVisionClientCancelAppointmentMethod(response);

            // Act
            var result = await _systemUnderTest.Cancel(_userSession, _request);

            // Assert
            _mockVisionClient.Verify();
            result.Should().BeAssignableTo<AppointmentCancelResult.AppointmentNotCancellable>();
        }
        
        [TestMethod]
        public async Task Cancel_VisionClientReturnsAppointmentSlotNotFoundError_ReturnsAppointmentNotCancellable()
        {
            // Arrange
            var response = VisionApiObjectResponseBuilder
                .BuildUnsuccessfulResponseWithErrorCode<CancelledAppointmentResponse>("-21");
            MockVisionClientCancelAppointmentMethod(response);

            // Act
            var result = await _systemUnderTest.Cancel(_userSession, _request);

            // Assert
            _mockVisionClient.Verify();
            result.Should().BeAssignableTo<AppointmentCancelResult.AppointmentNotCancellable>();
        }
        
        [TestMethod]
        public async Task Cancel_VisionClientReturnsAccessDenied_ReturnsInsufficientPermissions()
        {
            // Arrange
            var response = VisionApiObjectResponseBuilder
                .BuildUnsuccessfulResponseWithErrorCode<CancelledAppointmentResponse>("-35");
            MockVisionClientCancelAppointmentMethod(response);

            // Act
            var result = await _systemUnderTest.Cancel(_userSession, _request);

            // Assert
            _mockVisionClient.Verify();
            result.Should().BeAssignableTo<AppointmentCancelResult.InsufficientPermissions>();
        }
        
        [TestMethod]
        public async Task Cancel_VisionClientReturnsUnexpectedErrorCode_ReturnsSupplierSystemUnavailable()
        {
            // Arrange
            var unexpectedErrorCode = _fixture.Create<string>();
            var response = VisionApiObjectResponseBuilder
                .BuildUnsuccessfulResponseWithErrorCode<CancelledAppointmentResponse>(unexpectedErrorCode);
            MockVisionClientCancelAppointmentMethod(response);
            
            // Act
            var result = await _systemUnderTest.Cancel(_userSession, _request);
            
            // Assert
            _mockVisionClient.Verify();
            result.Should().BeAssignableTo<AppointmentCancelResult.SupplierSystemUnavailable>();
        }
        
        private void MockVisionClientCancelAppointmentMethod(
            VisionPFSClient.VisionApiObjectResponse<CancelledAppointmentResponse> response)
        {
            _mockVisionClient.Setup(x => x.CancelAppointment(
                It.Is<VisionUserSession>(p => 
                    p.RosuAccountId.Equals(_userSession.RosuAccountId, StringComparison.Ordinal)
                    && p.ApiKey.Equals(_userSession.ApiKey, StringComparison.Ordinal)
                    && p.OdsCode.Equals(_userSession.OdsCode, StringComparison.Ordinal)),
                It.Is<CancelAppointmentRequest>(p => 
                    p.PatientId.Equals(_userSession.PatientId, StringComparison.Ordinal)
                    && p.ReasonId.Equals(_request.CancellationReasonId, StringComparison.Ordinal)
                    && p.SlotId.Equals(_request.AppointmentId, StringComparison.Ordinal))
            ))
            .ReturnsAsync(response)
            .Verifiable();
        }
    }
}