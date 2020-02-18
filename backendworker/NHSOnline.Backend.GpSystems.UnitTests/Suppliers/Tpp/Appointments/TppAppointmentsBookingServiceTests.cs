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
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Appointments;
using NHSOnline.Backend.Support;

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
        private IAppointmentsService _systemUnderTest;
        private AppointmentBookRequest _request;
        private TppUserSession _tppUserSession;
        private Guid _patientId;
        private GpLinkedAccountModel _gpLinkedAccountModel;
        private Mock<ITppClientRequest<(TppUserSession, BookAppointment), BookAppointmentReply>> _mockBookAppointmentSlot;

        [TestInitialize]
        public void TestInitialize()
        {
            _patientId = Guid.NewGuid();
                
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _mockBookAppointmentSlot = _fixture
                .Freeze<Mock<ITppClientRequest<(TppUserSession, BookAppointment ), BookAppointmentReply>>>(); 

            _tppUserSession = _fixture.Create<TppUserSession>();

            _gpLinkedAccountModel = new GpLinkedAccountModel(_tppUserSession, _patientId);
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
            var response = new TppApiObjectResponse<BookAppointmentReply>(HttpStatusCode.OK)
            {
                Body = new BookAppointmentReply(),
                ErrorResponse = null
            };

            MockTppClientAppointmentPostMethod(response);

            // Act            
            var result = await _systemUnderTest.Book(_gpLinkedAccountModel, _request);

            // Assert
            _mockBookAppointmentSlot.Verify();
            result.Should().BeAssignableTo<AppointmentBookResult.Success>();
        }

        [TestMethod]
        public async Task Book_TppClientThrowsHttpRequestExceptionFromAppointments_ReturnsBadGateway()
        {
            // Arrange
            _mockBookAppointmentSlot.Setup(x => x.Post(It.IsAny<(TppUserSession, BookAppointment)>())).
                Throws<HttpRequestException>()
                .Verifiable();

            // Act            
            var result = await _systemUnderTest.Book(_gpLinkedAccountModel, _request);

            // Assert
            _mockBookAppointmentSlot.Verify();
            result.Should().BeAssignableTo<AppointmentBookResult.BadGateway>();
        }

        [TestMethod]
        public async Task Book_WhenNotFoundAppointment_ReturnsSlotNotAvailable()
        {
            // Arrange
            var response = new TppApiObjectResponse<BookAppointmentReply>(HttpStatusCode.OK)
            {
                Body = null,
                ErrorResponse = new Error { ErrorCode = TppApiErrorCodes.SlotNotFound }
            };

            MockTppClientAppointmentPostMethod(response);

            // Act            
            var result = await _systemUnderTest.Book(_gpLinkedAccountModel, _request);

            // Assert
            _mockBookAppointmentSlot.Verify();
            result.Should().BeAssignableTo<AppointmentBookResult.SlotNotAvailable>();
        }

        [TestMethod]
        public async Task Book_WhenAppointmentsIsInThePast_ReturnsSlotNotAvailable()
        {
            // Arrange
            var response = new TppApiObjectResponse<BookAppointmentReply>(HttpStatusCode.OK)
            {
                Body = null,
                ErrorResponse = new Error { ErrorCode = TppApiErrorCodes.StartDateInPast }
            };

            MockTppClientAppointmentPostMethod(response);

            // Act            
            var result = await _systemUnderTest.Book(_gpLinkedAccountModel, _request);

            // Assert
            _mockBookAppointmentSlot.Verify();
            result.Should().BeAssignableTo<AppointmentBookResult.SlotNotAvailable>();
        }

        [TestMethod]
        public async Task Book_WhenAppointmentsLimitIsReached_ReturnsAppointmentLimitReached()
        {
            // Arrange
            var response = new TppApiObjectResponse<BookAppointmentReply>(HttpStatusCode.OK)
            {
                Body = null,
                ErrorResponse = new Error { ErrorCode = TppApiErrorCodes.AppointmentLimitReached }
            };

            MockTppClientAppointmentPostMethod(response);

            // Act            
            var result = await _systemUnderTest.Book(_gpLinkedAccountModel, _request);

            // Assert
            _mockBookAppointmentSlot.Verify();
            result.Should().BeAssignableTo<AppointmentBookResult.AppointmentLimitReached>();
        }

        [TestMethod]
        public async Task Book_WhenAppointmentsHasBeenAlreadyBooked_ReturnsSlotNotAvailable()
        {
            // Arrange
            var response = new TppApiObjectResponse<BookAppointmentReply>(HttpStatusCode.OK)
            {
                Body = null,
                ErrorResponse = new Error { ErrorCode = TppApiErrorCodes.SlotAlreadyBooked }
            };

            MockTppClientAppointmentPostMethod(response);

            // Act            
            var result = await _systemUnderTest.Book(_gpLinkedAccountModel, _request);

            // Assert
            _mockBookAppointmentSlot.Verify();
            result.Should().BeAssignableTo<AppointmentBookResult.SlotNotAvailable>();
        }

        [TestMethod]
        public async Task Book_WhenPatientDoesNotHaveNecessaryPermissions_ReturnsSlotNotAvailable()
        {
            // Arrange
            var response = new TppApiObjectResponse<BookAppointmentReply>(HttpStatusCode.OK)
            {
                Body = null,
                ErrorResponse = new Error { ErrorCode = TppApiErrorCodes.NoAccess }
            };

            MockTppClientAppointmentPostMethod(response);

            // Act            
            var result = await _systemUnderTest.Book(_gpLinkedAccountModel, _request);

            // Assert
            _mockBookAppointmentSlot.Verify();
            result.Should().BeAssignableTo<AppointmentBookResult.Forbidden>();
        }

        [TestMethod]
        public async Task Book_TppReturnsUnknownError_ReturnsBadGateway()
        {
            // Arrange
            var response = new TppApiObjectResponse<BookAppointmentReply>(HttpStatusCode
                    .InternalServerError);

            MockTppClientAppointmentPostMethod(response);

            // Act            
            var result = await _systemUnderTest.Book(_gpLinkedAccountModel, _request);

            // Assert
            _mockBookAppointmentSlot.Verify();
            result.Should().BeAssignableTo<AppointmentBookResult.BadGateway>();
        }
        
        [TestMethod]
        public async Task Booking_StartDateIsNull_ReturnsBadRequest()
        {
            // Arrange
            _request.StartTime = null;

            // Act
            var result = await _systemUnderTest.Book(_gpLinkedAccountModel, _request);

            // Assert
            result.Should().BeAssignableTo<AppointmentBookResult.BadRequest>();
        }

        [TestMethod]
        public async Task Booking_EndDateIsNull_ReturnsBadRequest()
        {
            // Arrange
            _request.StartTime = null;

            // Act
            var result = await _systemUnderTest.Book(_gpLinkedAccountModel, _request);

            // Assert
            result.Should().BeAssignableTo<AppointmentBookResult.BadRequest>();
        }


        private void MockTppClientAppointmentPostMethod(TppApiObjectResponse<BookAppointmentReply> response)
        {
            _mockBookAppointmentSlot.Setup(x => x.Post(
                    It.Is<(TppUserSession, BookAppointment )>(tuple =>
                        tuple.Item2.Notes.Equals(BookingReason, StringComparison.Ordinal)
                        && tuple.Item2.SessionId.Equals(SlotId, StringComparison.Ordinal))
                )
            ).Returns(
                Task.FromResult(
                    response
                )
            ).Verifiable();
        }
    }
}
