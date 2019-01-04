using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using Castle.Core.Internal;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.Areas.Appointments.Models;
using NHSOnline.Backend.Worker.Areas.SharedModels;
using NHSOnline.Backend.Worker.GpSystems.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Emis.Appointments
{
    [TestClass]
    public class EmisAppointmentsBookingServiceTests
    {
        private const string BookingReason = "I caught a cold!";
        private const string SlotId = "2862517";
        private const string TelephoneNumber = "07123456789";

        private IFixture _fixture;
        private Mock<IEmisClient> _mockEmisClient;
        private IAppointmentsService _systemUnderTest;
        private AppointmentBookRequest _request;
        private EmisUserSession _emisUserSession;
        private UserSession _userSession;
        
        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            
            _emisUserSession = _fixture.Create<EmisUserSession>();
            _emisUserSession.AppointmentBookingReasonNecessity = Necessity.Optional;
            
            _fixture.Customize<UserSession>(c => c
                .With(u => u.GpUserSession, _emisUserSession));

            _userSession = _fixture.Create<UserSession>();
            
            _mockEmisClient = _fixture.Freeze<Mock<IEmisClient>>();

            _systemUnderTest = _fixture.Create<EmisAppointmentsService>();
            
            _request = new AppointmentBookRequest
            {
                BookingReason = BookingReason,
                SlotId = SlotId,
                TelephoneNumber = TelephoneNumber
            };
        }

        [TestMethod]
        public async Task Book_HappyPath_ReturnsSuccessfullyBookedResponse()
        {
            // Arrange
            var response = new EmisClient.EmisApiObjectResponse<BookAppointmentSlotPostResponse>(HttpStatusCode.OK)
            {
                Body = new BookAppointmentSlotPostResponse { BookingCreated = true },
                ExceptionErrorResponse = null,
                ErrorResponseBadRequest = null
            };

            MockEmisClientAppointmentPostMethod(response);
            
            // Act            
            var result = await _systemUnderTest.Book(_userSession, _request);

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentBookResult.SuccessfullyBooked>();
        }

        [TestMethod]
        public async Task Book_EmisClientThrowsHttpRequestExceptionFromAppointments_ReturnsSupplierSystemUnavailable()
        {
            // Arrange
            _mockEmisClient.Setup(x => x.AppointmentsPost(It.IsAny<EmisHeaderParameters>(),
                    It.IsAny<BookAppointmentSlotPostRequest>())).
                Throws<HttpRequestException>()
                .Verifiable();
            
            // Act            
            var result = await _systemUnderTest.Book(_userSession, _request);

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentBookResult.SupplierSystemUnavailable>();
        }

        [TestMethod]
        public async Task Book_WhenNotFoundAppointment_ReturnsSlotNotAvailable()
        {
            // Arrange
            var errorResponse = _fixture.Create<StandardErrorResponse>();

            var response = new EmisClient.EmisApiObjectResponse<BookAppointmentSlotPostResponse>(HttpStatusCode
                .NotFound) {StandardErrorResponse = errorResponse};

            MockEmisClientAppointmentPostMethod(response);

            // Act
            var result = await _systemUnderTest.Book(_userSession, _request);

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
                .InternalServerError) {ExceptionErrorResponse = errorResponse};
            
            MockEmisClientAppointmentPostMethod(response);
            
            // Act            
            var result = await _systemUnderTest.Book(_userSession, _request);

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentBookResult.SlotNotAvailable>();
        }

        [TestMethod]
        public async Task Book_WhenAppointmentsIsInThePast_ReturnsSlotNotAvailable()
        {
            // Arrange
            var errorResponse = _fixture.Create<StandardErrorResponse>();
            errorResponse.InternalResponseCode = (int) EmisApiErrorCode.ProvidedAppointmentSlotInPast;

            var response = new EmisClient.EmisApiObjectResponse<BookAppointmentSlotPostResponse>(HttpStatusCode
                .BadRequest) {StandardErrorResponse = errorResponse};

            MockEmisClientAppointmentPostMethod(response);

            // Act
            var result = await _systemUnderTest.Book(_userSession, _request);

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
                .InternalServerError) { ExceptionErrorResponse = errorResponse };
            
            MockEmisClientAppointmentPostMethod(response);
            
            // Act            
            var result = await _systemUnderTest.Book(_userSession, _request);

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentBookResult.SlotNotAvailable>();
        }
        
        [TestMethod]
        public async Task Book_WhenAppointmentsHasBeenAlreadyBooked_ReturnsSlotNotAvailable()
        {
            //Arrange
            var response = new EmisClient.EmisApiObjectResponse<BookAppointmentSlotPostResponse>(HttpStatusCode
                .Conflict);
            
            MockEmisClientAppointmentPostMethod(response);
            
            // Act            
            var result = await _systemUnderTest.Book(_userSession, _request);

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentBookResult.SlotNotAvailable>();
        }


        
        [TestMethod]
        public async Task Book_WhenEmisReturnsForbidden_ReturnsInsufficientPermissions()
        {
            //Arrange
            var response = new EmisClient.EmisApiObjectResponse<BookAppointmentSlotPostResponse>(HttpStatusCode
                .Forbidden);
            
            MockEmisClientAppointmentPostMethod(response);
            
            // Act            
            var result = await _systemUnderTest.Book(_userSession, _request);

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentBookResult.InsufficientPermissions>();
        }

        [TestMethod]
        public async Task Book_WhenPatientDoesNotHaveNecessaryPermissions_ReturnsSlotNotAvailable()
        {
            var errorResponse = _fixture.Create<StandardErrorResponse>();


            //Arrange
            var response = new EmisClient.EmisApiObjectResponse<BookAppointmentSlotPostResponse>(HttpStatusCode
                    .Forbidden)
                { StandardErrorResponse = errorResponse };

            MockEmisClientAppointmentPostMethod(response);

            // Act
            var result = await _systemUnderTest.Book(_userSession, _request);

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentBookResult.InsufficientPermissions>();
        }

        [TestMethod]
        public async Task Book_WhenPatientDoesNotHaveNecessaryPermissionsException_ReturnsSlotNotAvailable()
        {
            var errorResponse = _fixture.Create<ExceptionErrorResponse>();
            errorResponse.Exceptions.First().Message = "Extra info: " + EmisApiErrorMessages.EmisService_NotEnabledForUser;


            //Arrange
            var response = new EmisClient.EmisApiObjectResponse<BookAppointmentSlotPostResponse>(HttpStatusCode
                .InternalServerError)
            { ExceptionErrorResponse = errorResponse };

            MockEmisClientAppointmentPostMethod(response);

            // Act            
            var result = await _systemUnderTest.Book(_userSession, _request);

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentBookResult.InsufficientPermissions>();
        }

        [TestMethod]
        public async Task Book_WhenPatientHasReachedAppointmentLimit_ReturnsAppointmentLimitReached()
        {
            var errorResponse = _fixture.Create<StandardErrorResponse>();
            errorResponse.InternalResponseCode = (int) EmisApiErrorCode.OnlineUserMaxAppointmentBookCount;

            //Arrange
            var response = new EmisClient.EmisApiObjectResponse<BookAppointmentSlotPostResponse>(HttpStatusCode
                    .BadRequest)
                { StandardErrorResponse = errorResponse };

            MockEmisClientAppointmentPostMethod(response);

            // Act
            var result = await _systemUnderTest.Book(_userSession, _request);

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentBookResult.AppointmentLimitReached>();
        }

        [TestMethod]
        public async Task Book_WhenPatientHasReachedAppointmentLimitException_ReturnsAppointmentLimitReached()
        {
            var errorResponse = _fixture.Create<ExceptionErrorResponse>();
            errorResponse.Exceptions.First().Message = $"{EmisApiErrorMessages.EmisService_BookedAppointmentLimit} to 35 by the practice";

            //Arrange
            var response = new EmisClient.EmisApiObjectResponse<BookAppointmentSlotPostResponse>(HttpStatusCode
                .InternalServerError)
            { ExceptionErrorResponse = errorResponse };

            MockEmisClientAppointmentPostMethod(response);

            // Act            
            var result = await _systemUnderTest.Book(_userSession, _request);

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentBookResult.AppointmentLimitReached>();
        }

        [TestMethod]
        public async Task Book_EmisReturnsUnknownError_ReturnsSupplierSystemUnavailable()
        {
            var errorResponse = _fixture.Create<ExceptionErrorResponse>();
            errorResponse.Exceptions.First().Message = "Extra info: Unhandled Error";

            //Arrange
            var response = new EmisClient.EmisApiObjectResponse<BookAppointmentSlotPostResponse>(HttpStatusCode
                    .InternalServerError)
                { ExceptionErrorResponse = errorResponse };

            MockEmisClientAppointmentPostMethod(response);

            // Act            
            var result = await _systemUnderTest.Book(_userSession, _request);

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentBookResult.SupplierSystemUnavailable>();
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
            var result = await _systemUnderTest.Book(_userSession, _request);

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
            var result = await _systemUnderTest.Book(_userSession, _request);

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentBookResult.BadRequest>();
        }
        
        [TestMethod]
        public async Task Book_WhenTelephoneNumberRequiredButNotProvided_ReturnsBadRequestResponse()
        {
            var errorResponse = _fixture.Create<StandardErrorResponse>();
            errorResponse.InternalResponseCode = (int) EmisApiErrorCode.RequiredFieldValueMissing;

            //Arrange
            
            var response = new EmisClient.EmisApiObjectResponse<BookAppointmentSlotPostResponse>(HttpStatusCode
                    .BadRequest)
                { StandardErrorResponse = errorResponse };

            MockEmisClientAppointmentPostMethod(response);

            // Act
            var result = await _systemUnderTest.Book(_userSession, _request);

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentBookResult.BadRequest>();
        }
        
        [TestMethod]
        public async Task Book_WhenTelephoneNumberRequiredException_ReturnsBadRequestResponse()
        {
            var errorResponse = _fixture.Create<ExceptionErrorResponse>();
            errorResponse.Exceptions.First().Message = $"{EmisApiErrorMessages.EmisService_TelephoneNumberRequired}";

            //Arrange
            var response = new EmisClient.EmisApiObjectResponse<BookAppointmentSlotPostResponse>(HttpStatusCode
                    .BadRequest)
                { ExceptionErrorResponse = errorResponse };

            MockEmisClientAppointmentPostMethod(response);

            // Act            
            var result = await _systemUnderTest.Book(_userSession, _request);

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentBookResult.BadRequest>();
        }        

        private void MockEmisClientAppointmentPostMethod(EmisClient.EmisApiObjectResponse<BookAppointmentSlotPostResponse> response)
        {
            _mockEmisClient.Setup(x => x.AppointmentsPost(
                    It.Is<EmisHeaderParameters>(p =>
                        p.EndUserSessionId.Equals(_emisUserSession.EndUserSessionId, StringComparison.Ordinal)
                        && p.SessionId.Equals(_emisUserSession.SessionId, StringComparison.Ordinal)),
                    It.Is<BookAppointmentSlotPostRequest>(p =>
                        p.BookingReason.Equals(BookingReason, StringComparison.Ordinal)
                        && p.TelephoneNumber.Equals(TelephoneNumber, StringComparison.Ordinal)
                        && p.SlotId == Convert.ToInt64(SlotId, CultureInfo.InvariantCulture)
                        && p.UserPatientLinkToken.Equals(_emisUserSession.UserPatientLinkToken, StringComparison.Ordinal)
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
