using System;
using System.Globalization;
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
using NHSOnline.Backend.GpSystems.Suppliers.Tpp;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Appointments;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp.Appointments
{
    [TestClass]
    public class TppAppointmentsBookingServiceTests
    {
        private const string BookingReason = "I caught a cold!";
        private const string SlotId = "347ac20000000000";
        private const string StartTime = "2017-07-12T09:10:00+00:00";
        private const string EndTime = "2017-07-12T09:20:00+00:00";

        private IFixture _fixture;
        private Mock<ITppClient> _mockTppClient;
        private IAppointmentsService _systemUnderTest;
        private AppointmentBookRequest _request;
        private TppUserSession _tppUserSession;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            
            _mockTppClient = _fixture.Freeze<Mock<ITppClient>>();

            _tppUserSession = _fixture.Create<TppUserSession>();

            _systemUnderTest = _fixture.Create<TppAppointmentsService>();

            _request = new AppointmentBookRequest
            {
                BookingReason = BookingReason,
                SlotId = SlotId,
                StartTime = DateTimeOffset.Parse(StartTime, CultureInfo.InvariantCulture),
                EndTime = DateTimeOffset.Parse(EndTime, CultureInfo.InvariantCulture)
            };
        }

        [TestMethod]
        public async Task Book_HappyPath_ReturnsSuccessResponse()
        {
            // Arrange
            var response = new TppClient.TppApiObjectResponse<BookAppointmentReply>(HttpStatusCode.OK)
            {
                Body = new BookAppointmentReply(),
                ErrorResponse = null
            };

            MockTppClientAppointmentPostMethod(response);

            // Act            
            var result = await _systemUnderTest.Book(_tppUserSession, _request);

            // Assert
            _mockTppClient.Verify();
            result.Should().BeAssignableTo<AppointmentBookResult.Success>();
        }

        [TestMethod]
        public async Task Book_TppClientThrowsHttpRequestExceptionFromAppointments_ReturnsBadGateway()
        {
            // Arrange
            _mockTppClient.Setup(x => x.BookAppointmentSlotPost(It.IsAny<BookAppointment>(),
                    It.IsAny<TppUserSession>())).
                Throws<HttpRequestException>()
                .Verifiable();

            // Act            
            var result = await _systemUnderTest.Book(_tppUserSession, _request);

            // Assert
            _mockTppClient.Verify();
            result.Should().BeAssignableTo<AppointmentBookResult.BadGateway>();
        }

        [TestMethod]
        public async Task Book_WhenNotFoundAppointment_ReturnsSlotNotAvailable()
        {
            // Arrange
            var response = new TppClient.TppApiObjectResponse<BookAppointmentReply>(HttpStatusCode.OK)
            {
                Body = null,
                ErrorResponse = new Error { ErrorCode = TppApiErrorCodes.SlotNotFound }
            };

            MockTppClientAppointmentPostMethod(response);

            // Act            
            var result = await _systemUnderTest.Book(_tppUserSession, _request);

            // Assert
            _mockTppClient.Verify();
            result.Should().BeAssignableTo<AppointmentBookResult.SlotNotAvailable>();
        }

        [TestMethod]
        public async Task Book_WhenAppointmentsIsInThePast_ReturnsSlotNotAvailable()
        {
            // Arrange
            var response = new TppClient.TppApiObjectResponse<BookAppointmentReply>(HttpStatusCode.OK)
            {
                Body = null,
                ErrorResponse = new Error { ErrorCode = TppApiErrorCodes.StartDateInPast }
            };

            MockTppClientAppointmentPostMethod(response);

            // Act            
            var result = await _systemUnderTest.Book(_tppUserSession, _request);

            // Assert
            _mockTppClient.Verify();
            result.Should().BeAssignableTo<AppointmentBookResult.SlotNotAvailable>();
        }

        [TestMethod]
        public async Task Book_WhenAppointmentsLimitIsReached_ReturnsAppointmentLimitReached()
        {
            // Arrange
            var response = new TppClient.TppApiObjectResponse<BookAppointmentReply>(HttpStatusCode.OK)
            {
                Body = null,
                ErrorResponse = new Error { ErrorCode = TppApiErrorCodes.AppointmentLimitReached }
            };

            MockTppClientAppointmentPostMethod(response);

            // Act            
            var result = await _systemUnderTest.Book(_tppUserSession, _request);

            // Assert
            _mockTppClient.Verify();
            result.Should().BeAssignableTo<AppointmentBookResult.AppointmentLimitReached>();
        }

        [TestMethod]
        public async Task Book_WhenAppointmentsHasBeenAlreadyBooked_ReturnsSlotNotAvailable()
        {
            // Arrange
            var response = new TppClient.TppApiObjectResponse<BookAppointmentReply>(HttpStatusCode.OK)
            {
                Body = null,
                ErrorResponse = new Error { ErrorCode = TppApiErrorCodes.SlotAlreadyBooked }
            };

            MockTppClientAppointmentPostMethod(response);

            // Act            
            var result = await _systemUnderTest.Book(_tppUserSession, _request);

            // Assert
            _mockTppClient.Verify();
            result.Should().BeAssignableTo<AppointmentBookResult.SlotNotAvailable>();
        }

        [TestMethod]
        public async Task Book_WhenPatientDoesNotHaveNecessaryPermissions_ReturnsSlotNotAvailable()
        {
            // Arrange
            var response = new TppClient.TppApiObjectResponse<BookAppointmentReply>(HttpStatusCode.OK)
            {
                Body = null,
                ErrorResponse = new Error { ErrorCode = TppApiErrorCodes.NoAccess }
            };

            MockTppClientAppointmentPostMethod(response);

            // Act            
            var result = await _systemUnderTest.Book(_tppUserSession, _request);

            // Assert
            _mockTppClient.Verify();
            result.Should().BeAssignableTo<AppointmentBookResult.Forbidden>();
        }

        [TestMethod]
        public async Task Book_TppReturnsUnknownError_ReturnsBadGateway()
        {
            // Arrange
            var response = new TppClient.TppApiObjectResponse<BookAppointmentReply>(HttpStatusCode
                    .InternalServerError);

            MockTppClientAppointmentPostMethod(response);

            // Act            
            var result = await _systemUnderTest.Book(_tppUserSession, _request);

            // Assert
            _mockTppClient.Verify();
            result.Should().BeAssignableTo<AppointmentBookResult.BadGateway>();
        }
        
        [TestMethod]
        public async Task Booking_StartDateIsNull_ReturnsBadRequest()
        {
            // Arrange
            _request.StartTime = null;

            // Act
            var result = await _systemUnderTest.Book(_tppUserSession, _request);

            // Assert
            result.Should().BeAssignableTo<AppointmentBookResult.BadRequest>();
        }

        [TestMethod]
        public async Task Booking_EndDateIsNull_ReturnsBadRequest()
        {
            // Arrange
            _request.StartTime = null;

            // Act
            var result = await _systemUnderTest.Book(_tppUserSession, _request);

            // Assert
            result.Should().BeAssignableTo<AppointmentBookResult.BadRequest>();
        }


        private void MockTppClientAppointmentPostMethod(TppClient.TppApiObjectResponse<BookAppointmentReply> response)
        {
            _mockTppClient.Setup(x => x.BookAppointmentSlotPost(
                It.Is<BookAppointment>(p=>
                    p.Notes.Equals(BookingReason, StringComparison.Ordinal)
                    && p.SessionId.Equals(SlotId, StringComparison.Ordinal)),
                It.IsAny<TppUserSession>()
                )
            ).Returns(
                Task.FromResult(
                    response
                )
            ).Verifiable();
        }
    }
}
