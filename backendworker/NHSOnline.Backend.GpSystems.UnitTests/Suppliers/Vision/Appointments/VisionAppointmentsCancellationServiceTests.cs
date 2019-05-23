using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Vision;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Session;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Vision.Appointments
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
        public async Task Cancel_HappyPath_ReturnsSuccessResponse()
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
            result.Should().BeAssignableTo<AppointmentCancelResult.Success>();
        }
        
        [TestMethod]
        public async Task Cancel_VisionClientThrowsHttpRequestException_ReturnsBadGateway()
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
            result.Should().BeAssignableTo<AppointmentCancelResult.BadGateway>();
        }
        
        [TestMethod]
        public async Task Cancel_AppointmentsDisabled_ReturnsForbidden()
        {
            // Arrange
            _userSession.IsAppointmentsEnabled = false;

            // Act            
            var result = await _systemUnderTest.Cancel(_userSession, _request);
            
            // Assert
            result.Should().BeAssignableTo<AppointmentCancelResult.Forbidden>();
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
        public async Task Cancel_VisionClientReturnsAccessDenied_ReturnsForbidden()
        {
            // Arrange
            var response = VisionApiObjectResponseBuilder
                .BuildUnsuccessfulResponseWithErrorCode<CancelledAppointmentResponse>("-35");
            MockVisionClientCancelAppointmentMethod(response);

            // Act
            var result = await _systemUnderTest.Cancel(_userSession, _request);

            // Assert
            _mockVisionClient.Verify();
            result.Should().BeAssignableTo<AppointmentCancelResult.Forbidden>();
        }
        
        [TestMethod]
        public async Task Cancel_VisionClientReturnsUnexpectedErrorCode_ReturnsBadGateway()
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
            result.Should().BeAssignableTo<AppointmentCancelResult.BadGateway>();
        }

        [TestMethod]
        public async Task Cancel_VisionClientReturnsUnparsableMessage_ReturnsInternalServerError_ReturnsInternalServerError()
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
                },
                UnparsableResultMessage = "Could not parse"
            };
            MockVisionClientCancelAppointmentMethod(response);

            // Act
            var result = await _systemUnderTest.Cancel(_userSession, _request);

            // Assert
            _mockVisionClient.Verify();
            result.Should().BeAssignableTo<AppointmentCancelResult.InternalServerError>();
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
