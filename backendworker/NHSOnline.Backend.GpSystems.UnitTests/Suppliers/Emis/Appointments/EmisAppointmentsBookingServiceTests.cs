using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.GpSystems.SharedModels;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Strategies.ResponseSuccessOutcome;
using NHSOnline.Backend.Support;
using UnitTestHelper;
using Appointment = NHSOnline.Backend.GpSystems.Appointments.Models.Appointment;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Emis.Appointments
{
    [TestClass]
    public class EmisAppointmentsBookingServiceTests
    {
        private const string BookingReason = "I caught a cold!";
        private const string SlotId = "2862517";
        private const string TelephoneNumber = "07123456789";

        private const int ProvidedAppointmentSlotInPast = -1152;
        private const int RequiredFieldValueMissing = -1014;
        private const int OnlineUserMaxAppointmentBookCount = -1156;
        private Mock<ILogger<EmisAppointmentsBookingService>> _logger;
        private IFixture _fixture;
        private Mock<IEmisClient> _mockEmisClient;
        private IAppointmentsService _systemUnderTest;
        private AppointmentBookRequest _request;
        private EmisUserSession _emisUserSession;
        private Guid _patientId;
        private GpLinkedAccountModel _gpLinkedAccountModel;
        private List<HttpStatusCode> _sampleSuccessStatusCodes;

        [TestInitialize]
        public void TestInitialize()
        {
            _patientId = Guid.NewGuid();
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _emisUserSession = _fixture.Build<EmisUserSession>()
                .With(x => x.Id, _patientId)
                .Create();

            _emisUserSession.AppointmentBookingReasonNecessity = Necessity.Optional;

            _gpLinkedAccountModel = new GpLinkedAccountModel(_emisUserSession, _patientId);

            _mockEmisClient = _fixture.Freeze<Mock<IEmisClient>>();
            _logger = _fixture.Freeze<Mock<ILogger<EmisAppointmentsBookingService>>>();
            _systemUnderTest = _fixture.Create<EmisAppointmentsService>();
            _request = new AppointmentBookRequest
            {
                BookingReason = BookingReason,
                SlotId = SlotId,
                TelephoneNumber = TelephoneNumber
            };
            
            _sampleSuccessStatusCodes = new List<HttpStatusCode>()
            {
                HttpStatusCode.OK
            };
        }

        [TestMethod]
        public async Task Book_HappyPath_ReturnsSuccessResponse()
        {
            // Arrange
            var response = new EmisClient.EmisApiObjectResponse<BookAppointmentSlotPostResponse>(HttpStatusCode.OK, RequestsForSuccessOutcome.AppointmentsPost, _sampleSuccessStatusCodes)
            {
                Body = new BookAppointmentSlotPostResponse { BookingCreated = true },
                ExceptionErrorResponse = null,
                ErrorResponseBadRequest = null
            };

            MockEmisClientAppointmentPostMethod(response);
            // Act
            var result = await _systemUnderTest.Book(_gpLinkedAccountModel, _request);

            var moreThanOneCharacter = _request.BookingReason.Length >= 1;
            var expectedLogMessage =
                $"Appointments Booking Reason Info: More than one character in booking reason={moreThanOneCharacter} " +
                $"Characters entered in booking reason={_request.BookingReason.Length}";
            
            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentBookResult.Success>();
            _logger.VerifyLogger(LogLevel.Information, expectedLogMessage, Times.Once());
        }

        [TestMethod]
        public async Task Book_EmisClientThrowsHttpRequestExceptionFromAppointments_ReturnsBadGateway()
        {
            // Arrange
            _mockEmisClient.Setup(x => x.AppointmentsPost(
                    It.IsAny<EmisRequestParameters>(),
                    It.IsAny<AppointmentBookRequest>())).
                Throws<HttpRequestException>()
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Book(_gpLinkedAccountModel, _request);

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentBookResult.BadGateway>();
        }

        [TestMethod]
        public async Task Book_WhenNotFoundAppointment_ReturnsSlotNotAvailable()
        {
            // Arrange
            var errorResponse = _fixture.Create<StandardErrorResponse>();

            var response = new EmisClient.EmisApiObjectResponse<BookAppointmentSlotPostResponse>(HttpStatusCode
                .NotFound, RequestsForSuccessOutcome.AppointmentsPost, _sampleSuccessStatusCodes) {StandardErrorResponse = errorResponse};

            MockEmisClientAppointmentPostMethod(response);

            // Act
            var result = await _systemUnderTest.Book(_gpLinkedAccountModel, _request);

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentBookResult.SlotNotAvailable>();
        }

        [TestMethod]
        public async Task Book_WhenNotFoundAppointmentException_ReturnsSlotNotAvailable()
        {
            // Arrange
            var errorResponse = _fixture.Create<ExceptionErrorResponse>();
            errorResponse.Exceptions.First().Message = EmisApiErrorMessages.AppointmentsPost_NotFound;

            var response = new EmisClient.EmisApiObjectResponse<BookAppointmentSlotPostResponse>(HttpStatusCode
                .InternalServerError, RequestsForSuccessOutcome.AppointmentsPost, _sampleSuccessStatusCodes) {ExceptionErrorResponse = errorResponse};

            MockEmisClientAppointmentPostMethod(response);

            // Act
            var result = await _systemUnderTest.Book(_gpLinkedAccountModel, _request);

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentBookResult.SlotNotAvailable>();
        }

        [TestMethod]
        public async Task Book_WhenAppointmentsIsInThePast_ReturnsSlotNotAvailable()
        {
            // Arrange
            var errorResponse = _fixture.Create<StandardErrorResponse>();
            errorResponse.InternalResponseCode = ProvidedAppointmentSlotInPast;

            var response = new EmisClient.EmisApiObjectResponse<BookAppointmentSlotPostResponse>(HttpStatusCode
                .BadRequest, RequestsForSuccessOutcome.AppointmentsPost, _sampleSuccessStatusCodes) {StandardErrorResponse = errorResponse};

            MockEmisClientAppointmentPostMethod(response);

            // Act
            var result = await _systemUnderTest.Book(_gpLinkedAccountModel, _request);

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentBookResult.SlotNotAvailable>();
        }

        [TestMethod]
        public async Task Book_WhenAppointmentsIsInThePastException_ReturnsSlotNotAvailable()
        {
            // Arrange
            var errorResponse = _fixture.Create<ExceptionErrorResponse>();
            errorResponse.Exceptions.First().Message = EmisApiErrorMessages.AppointmentsPost_InThePast;

            var response = new EmisClient.EmisApiObjectResponse<BookAppointmentSlotPostResponse>(HttpStatusCode
                .InternalServerError, RequestsForSuccessOutcome.AppointmentsPost, _sampleSuccessStatusCodes) { ExceptionErrorResponse = errorResponse };

            MockEmisClientAppointmentPostMethod(response);

            // Act
            var result = await _systemUnderTest.Book(_gpLinkedAccountModel, _request);

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentBookResult.SlotNotAvailable>();
        }

        [TestMethod]
        public async Task Book_WhenAppointmentsHasBeenAlreadyBooked_ReturnsSlotNotAvailable()
        {
            // Arrange
            var response = new EmisClient.EmisApiObjectResponse<BookAppointmentSlotPostResponse>(HttpStatusCode
                .Conflict, RequestsForSuccessOutcome.AppointmentsPost, _sampleSuccessStatusCodes);

            MockEmisClientAppointmentPostMethod(response);

            // Act
            var result = await _systemUnderTest.Book(_gpLinkedAccountModel, _request);

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentBookResult.SlotNotAvailable>();
        }

        [DataTestMethod]
        [DataRow(-1154)]
        [DataRow(-1153)]
        public async Task Book_WhenEmisReturnsSlotOutsidePracticeDefinedDays_ReturnsSlotNotAvailable(int emisErrorCode)
        {
            // Arrange
            var errorResponse = _fixture.Create<StandardErrorResponse>();
            errorResponse.InternalResponseCode = emisErrorCode;

            var response = new EmisClient.EmisApiObjectResponse<BookAppointmentSlotPostResponse>(HttpStatusCode
                    .BadRequest, RequestsForSuccessOutcome.AppointmentsPost, _sampleSuccessStatusCodes)
                { StandardErrorResponse = errorResponse };

            MockEmisClientAppointmentPostMethod(response);

            // Act
            var result = await _systemUnderTest.Book(_gpLinkedAccountModel, _request);

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentBookResult.SlotNotAvailable>();
        }

        [TestMethod]
        public async Task Book_WhenEmisReturnsForbidden_ReturnsForbidden()
        {
            // Arrange
            var response = new EmisClient.EmisApiObjectResponse<BookAppointmentSlotPostResponse>(HttpStatusCode
                .Forbidden, RequestsForSuccessOutcome.AppointmentsPost, _sampleSuccessStatusCodes);

            MockEmisClientAppointmentPostMethod(response);

            // Act
            var result = await _systemUnderTest.Book(_gpLinkedAccountModel, _request);

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentBookResult.Forbidden>();
        }

        [TestMethod]
        public async Task Book_WhenPatientDoesNotHaveNecessaryPermissions_ReturnsSlotNotAvailable()
        {
            // Arrange
            var errorResponse = _fixture.Create<StandardErrorResponse>();
            var response = new EmisClient.EmisApiObjectResponse<BookAppointmentSlotPostResponse>(HttpStatusCode
                    .Forbidden, RequestsForSuccessOutcome.AppointmentsPost, _sampleSuccessStatusCodes)
                { StandardErrorResponse = errorResponse };

            MockEmisClientAppointmentPostMethod(response);

            // Act
            var result = await _systemUnderTest.Book(_gpLinkedAccountModel, _request);

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentBookResult.Forbidden>();
        }

        [TestMethod]
        public async Task Book_WhenPatientDoesNotHaveNecessaryPermissionsException_ReturnsSlotNotAvailable()
        {
            // Arrange
            var errorResponse = _fixture.Create<ExceptionErrorResponse>();
            errorResponse.Exceptions.First().Message = "Extra info: " + EmisApiErrorMessages.EmisService_NotEnabledForUser;

            var response = new EmisClient.EmisApiObjectResponse<BookAppointmentSlotPostResponse>(HttpStatusCode
                    .InternalServerError, RequestsForSuccessOutcome.AppointmentsPost, _sampleSuccessStatusCodes)
            { ExceptionErrorResponse = errorResponse };

            MockEmisClientAppointmentPostMethod(response);

            // Act
            var result = await _systemUnderTest.Book(_gpLinkedAccountModel, _request);

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentBookResult.Forbidden>();
        }

        [TestMethod]
        public async Task Book_WhenPatientHasReachedAppointmentLimit_ReturnsAppointmentLimitReached()
        {
            // Arrange
            var errorResponse = _fixture.Create<StandardErrorResponse>();
            errorResponse.InternalResponseCode = OnlineUserMaxAppointmentBookCount;

            var response = new EmisClient.EmisApiObjectResponse<BookAppointmentSlotPostResponse>(HttpStatusCode
                    .BadRequest, RequestsForSuccessOutcome.AppointmentsPost, _sampleSuccessStatusCodes)
                { StandardErrorResponse = errorResponse };

            MockEmisClientAppointmentPostMethod(response);

            // Act
            var result = await _systemUnderTest.Book(_gpLinkedAccountModel, _request);

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentBookResult.AppointmentLimitReached>();
        }

        [TestMethod]
        public async Task Book_WhenPatientHasReachedAppointmentLimitException_ReturnsAppointmentLimitReached()
        {
            // Arrange
            var errorResponse = _fixture.Create<ExceptionErrorResponse>();
            errorResponse.Exceptions.First().Message = $"{EmisApiErrorMessages.EmisService_BookedAppointmentLimit} to 35 by the practice";

            var response = new EmisClient.EmisApiObjectResponse<BookAppointmentSlotPostResponse>(HttpStatusCode
                    .InternalServerError, RequestsForSuccessOutcome.AppointmentsPost, _sampleSuccessStatusCodes)
            { ExceptionErrorResponse = errorResponse };

            MockEmisClientAppointmentPostMethod(response);

            // Act
            var result = await _systemUnderTest.Book(_gpLinkedAccountModel, _request);

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentBookResult.AppointmentLimitReached>();
        }

        [TestMethod]
        public async Task Book_EmisReturnsUnknownError_ReturnsBadGateway()
        {
            // Arrange
            var errorResponse = _fixture.Create<ExceptionErrorResponse>();
            errorResponse.Exceptions.First().Message = "Extra info: Unhandled Error";

            var response = new EmisClient.EmisApiObjectResponse<BookAppointmentSlotPostResponse>(HttpStatusCode
                    .InternalServerError, RequestsForSuccessOutcome.AppointmentsPost, _sampleSuccessStatusCodes)
                { ExceptionErrorResponse = errorResponse };

            MockEmisClientAppointmentPostMethod(response);

            // Act
            var result = await _systemUnderTest.Book(_gpLinkedAccountModel, _request);

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentBookResult.BadGateway>();
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        public async Task Book_BookingReasonMandatoryButNotProvided_ReturnsBadRequestResponse(string bookingReason)
        {
            // Arrange
            _emisUserSession.AppointmentBookingReasonNecessity = Necessity.Mandatory;
            _request.BookingReason = bookingReason;

            // Act
            var result = await _systemUnderTest.Book(_gpLinkedAccountModel, _request);

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentBookResult.BadRequest>();
        }

        [TestMethod]
        public async Task Book_BookingReasonNotAllowedButProvided_ReturnsBadRequestResponse()
        {
            // Arrange
            _emisUserSession.AppointmentBookingReasonNecessity = Necessity.NotAllowed;

            // Act
            var result = await _systemUnderTest.Book(_gpLinkedAccountModel, _request);

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentBookResult.BadRequest>();
        }

        [TestMethod]
        public async Task Book_WhenTelephoneNumberRequiredButNotProvided_ReturnsBadRequestResponse()
        {
            // Arrange
            var errorResponse = _fixture.Create<StandardErrorResponse>();
            errorResponse.InternalResponseCode = RequiredFieldValueMissing;

            var response = new EmisClient.EmisApiObjectResponse<BookAppointmentSlotPostResponse>(HttpStatusCode
                    .BadRequest, RequestsForSuccessOutcome.AppointmentsPost, _sampleSuccessStatusCodes)
                { StandardErrorResponse = errorResponse };

            MockEmisClientAppointmentPostMethod(response);

            // Act
            var result = await _systemUnderTest.Book(_gpLinkedAccountModel, _request);

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentBookResult.BadRequest>();
        }

        [TestMethod]
        public async Task Book_WhenTelephoneNumberRequiredException_ReturnsBadRequestResponse()
        {
            // Arrange
            var errorResponse = _fixture.Create<ExceptionErrorResponse>();
            errorResponse.Exceptions.First().Message = $"{EmisApiErrorMessages.EmisService_TelephoneNumberRequired}";

            var response = new EmisClient.EmisApiObjectResponse<BookAppointmentSlotPostResponse>(HttpStatusCode
                    .BadRequest, RequestsForSuccessOutcome.AppointmentsPost, _sampleSuccessStatusCodes)
                { ExceptionErrorResponse = errorResponse };

            MockEmisClientAppointmentPostMethod(response);

            // Act
            var result = await _systemUnderTest.Book(_gpLinkedAccountModel, _request);

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentBookResult.BadRequest>();
        }

        private void MockEmisClientAppointmentPostMethod(EmisClient.EmisApiObjectResponse<BookAppointmentSlotPostResponse> response)
        {
            _mockEmisClient.Setup(x => x.AppointmentsPost(
                    It.Is<EmisRequestParameters>(p =>
                        p.EndUserSessionId.Equals(_emisUserSession.EndUserSessionId, StringComparison.Ordinal) &&
                        p.SessionId.Equals(_emisUserSession.SessionId, StringComparison.Ordinal) &&
                        p.UserPatientLinkToken.Equals(_emisUserSession.UserPatientLinkToken, StringComparison.Ordinal)),
                    It.Is<AppointmentBookRequest>(p =>
                        p.BookingReason.Equals(BookingReason, StringComparison.Ordinal)
                        && p.TelephoneNumber.Equals(TelephoneNumber, StringComparison.Ordinal)
                        && p.SlotId.Equals(SlotId, StringComparison.Ordinal)
                    )
                )
            ).Returns(
                Task.FromResult(
                    response
                )
            ).Verifiable();
        }
    }
}
