using System;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.Areas.Appointments.Models;
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
        private const string BookingReason = "I caught a cold!";
        private const string SlotId = "2862517";
        
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

            _systemUnderTest = _fixture.Create<VisionAppointmentsService>();
            
            _request = new AppointmentBookRequest
            {
                BookingReason = BookingReason,
                SlotId = SlotId,
            };
            
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
        }
        
        [TestMethod]
        public async Task Book_HappyPath_ReturnsSuccessfullyBookedResponse()
        {
            // Act            
            var result = await _systemUnderTest.Book(_userSession, _request);

            // Assert
            _mockVisionClient.Verify();
            result.Should().BeAssignableTo<AppointmentBookResult.SuccessfullyBooked>();
        }
        
        [TestMethod]
        public async Task Book_WhenPatientDoesNotHaveNecessaryPermissions_ReturnsSlotNotAvailable()
        {
            // Arrange
            _userSession.IsAppointmentsEnabled = false;
            
            // Act            
            var result = await _systemUnderTest.Book(_userSession, _request);

            // Assert
            result.Should().BeAssignableTo<AppointmentBookResult.InsufficientPermissions>();
        }
        
        private void MockVisionClientAppointmentPostMethod(VisionClient.VisionApiObjectResponse<BookAppointmentResponse> response)
        {
            _mockVisionClient.Setup(x => x.BookAppointment(
                    It.IsAny<VisionUserSession>(),
                    It.Is<BookAppointmentRequest>(p=>
                        p.Reason.Equals(BookingReason, StringComparison.Ordinal)
                        && p.SlotId.Equals(SlotId, StringComparison.Ordinal))
                )
            ).Returns(
                Task.FromResult(
                    response
                )
            ).Verifiable();
        }
    }
}