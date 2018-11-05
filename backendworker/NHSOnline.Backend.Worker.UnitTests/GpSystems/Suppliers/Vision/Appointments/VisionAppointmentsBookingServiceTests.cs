using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.Areas.Appointments.Models;
using NHSOnline.Backend.Worker.Areas.SharedModels;
using NHSOnline.Backend.Worker.GpSystems.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Session;


namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Vision.Appointments
{
    [TestClass]
    public class VisionAppointmentsBookingServiceTests
    {
        private IFixture _fixture;
        private Mock<IVisionClient> _mockVisionClient;
        private IAppointmentsService _systemUnderTest;
        private AppointmentBookRequest _request;
        private VisionUserSession _userSession;
        private VisionResponse<BookAppointmentResponse> _visionClientGetResponse;
        
        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _mockVisionClient = _fixture.Freeze<Mock<IVisionClient>>();
            _visionClientGetResponse = _fixture.Create<VisionResponse<BookAppointmentResponse>>();

            _userSession = _fixture.Create<VisionUserSession>();
            _userSession.IsAppointmentsEnabled = true;
            _userSession.AppointmentBookingReasonNecessity = Necessity.Optional;

            _systemUnderTest = _fixture.Create<VisionAppointmentsService>();

            _request = _fixture.Create<AppointmentBookRequest>();
        }
        
        [TestMethod]
        public async Task Book_HappyPath_ReturnsSuccessfullyBookedResponse()
        {
            // Arrange
            var response = new VisionClient.VisionApiObjectResponse<BookAppointmentResponse>(HttpStatusCode.OK)
            {
                RawResponse = new VisionResponseEnvelope<BookAppointmentResponse>
                {
                    Body = new VisionResponseBody<BookAppointmentResponse>
                    {
                        VisionResponse = _visionClientGetResponse
                    }
                }
            };

            MockVisionClientAppointmentPostMethod(response);
            
            // Act            
            var result = await _systemUnderTest.Book(_userSession, _request);

            // Assert
            _mockVisionClient.Verify();
            result.Should().BeAssignableTo<AppointmentBookResult.SuccessfullyBooked>();
        }
        
        [TestMethod]
        public async Task Book_WhenPatientDoesNotHaveNecessaryPermissions_ReturnsInsufficientPermissions()
        {
            // Arrange
            _userSession.IsAppointmentsEnabled = false;
            
            // Act            
            var result = await _systemUnderTest.Book(_userSession, _request);

            // Assert
            result.Should().BeAssignableTo<AppointmentBookResult.InsufficientPermissions>();
        }

        [TestMethod]
        public async Task Book_VisionClientThrowsHttpRequestException_ReturnsSupplierSystemUnavailable()
        {
            // Arrange
            _mockVisionClient
                .Setup(x => x.BookAppointment(
                    It.IsAny<VisionUserSession>(),
                    It.IsAny<BookAppointmentRequest>()
                ))
                .Throws<HttpRequestException>()
                .Verifiable();
            
            // Act
            var result = await _systemUnderTest.Book(_userSession, _request);

            // Assert
            _mockVisionClient.Verify();
            result.Should().BeAssignableTo<AppointmentBookResult.SupplierSystemUnavailable>();
        }

        [TestMethod]
        public async Task Book_VisionClientReturnsSlotAlreadyBooked_ReturnsSlotNotAvailable()
        {
            // Arrange
            var response = VisionApiObjectResponseBuilder
                .BuildUnsuccessfulResponseWithErrorCode<BookAppointmentResponse>("-100");
            MockVisionClientAppointmentPostMethod(response);
            
            // Act
            var result = await _systemUnderTest.Book(_userSession, _request);
            
            // Assert
            _mockVisionClient.Verify();
            result.Should().BeAssignableTo<AppointmentBookResult.SlotNotAvailable>();
        }
        
        [TestMethod]
        public async Task Book_VisionClientReturnsSlotNotFound_ReturnsSlotNotAvailable()
        {
            // Arrange
            var response = VisionApiObjectResponseBuilder
                .BuildUnsuccessfulResponseWithErrorCode<BookAppointmentResponse>("-21");
            MockVisionClientAppointmentPostMethod(response);
            
            // Act
            var result = await _systemUnderTest.Book(_userSession, _request);
            
            // Assert
            _mockVisionClient.Verify();
            result.Should().BeAssignableTo<AppointmentBookResult.SlotNotAvailable>();
        }
        
        [TestMethod]
        public async Task Book_VisionClientReturnsAppointmentBookingLimitReached_ReturnsAppointmentLimitReached()
        {
            // Arrange
            var response = VisionApiObjectResponseBuilder
                .BuildUnsuccessfulResponseWithErrorCode<BookAppointmentResponse>("-25");
            MockVisionClientAppointmentPostMethod(response);
            
            // Act
            var result = await _systemUnderTest.Book(_userSession, _request);
            
            // Assert
            _mockVisionClient.Verify();
            result.Should().BeAssignableTo<AppointmentBookResult.AppointmentLimitReached>();
        }
        
        [TestMethod]
        public async Task Book_VisionClientReturnsAccessDenied_ReturnsInsufficientPermissions()
        {
            // Arrange
            var response = VisionApiObjectResponseBuilder
                .BuildUnsuccessfulResponseWithErrorCode<BookAppointmentResponse>("-35");
            MockVisionClientAppointmentPostMethod(response);
            
            // Act
            var result = await _systemUnderTest.Book(_userSession, _request);
            
            // Assert
            _mockVisionClient.Verify();
            result.Should().BeAssignableTo<AppointmentBookResult.InsufficientPermissions>();
        }
        
        [TestMethod]
        public async Task Book_VisionClientReturnsUnexpectedErrorCode_ReturnsSupplierSystemUnavailable()
        {
            // Arrange
            var unexpectedErrorCode = _fixture.Create<string>();
            var response = VisionApiObjectResponseBuilder
                .BuildUnsuccessfulResponseWithErrorCode<BookAppointmentResponse>(unexpectedErrorCode);
            MockVisionClientAppointmentPostMethod(response);
            
            // Act
            var result = await _systemUnderTest.Book(_userSession, _request);
            
            // Assert
            _mockVisionClient.Verify();
            result.Should().BeAssignableTo<AppointmentBookResult.SupplierSystemUnavailable>();
        }

        [TestMethod]
        public async Task Book_BookingReasonNotAllowed_ReturnsBadRequest()
        {
            // Arrange
            _userSession.AppointmentBookingReasonNecessity = Necessity.NotAllowed;

            // Act
            var result = await _systemUnderTest.Book(_userSession, _request);

            // Assert
            result.Should().BeAssignableTo<AppointmentBookResult.BadRequest>();
        }
        
        private void MockVisionClientAppointmentPostMethod(VisionClient.VisionApiObjectResponse<BookAppointmentResponse> response)
        {
            _mockVisionClient.Setup(x => x.BookAppointment(
                    It.IsAny<VisionUserSession>(),
                    It.Is<BookAppointmentRequest>(p =>
                        p.Reason.Equals(_request.BookingReason, StringComparison.Ordinal)
                        && p.SlotId.Equals(_request.SlotId, StringComparison.Ordinal))
                )
            ).Returns(
                Task.FromResult(
                    response
                )
            ).Verifiable();
        }
    }
}