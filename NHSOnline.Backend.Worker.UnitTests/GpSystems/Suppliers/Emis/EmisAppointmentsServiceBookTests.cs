using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.Areas.Appointments.Models;
using NHSOnline.Backend.Worker.GpSystems.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Emis
{
    [TestClass]
    public class EmisAppointmentsServiceBookTests
    {
        private const string BookingReason = "I caught a cold!";
        private const string SlotId = "2862517";
        
        private IFixture _fixture;
        private Mock<IEmisClient> _mockEmisClient;
        private IAppointmentsService _systemUnderTest;
        private AppointmentBookRequest _request;
        private EmisUserSession _userSession;
        
        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _mockEmisClient = _fixture.Freeze<Mock<IEmisClient>>();

            _userSession = _fixture.Create<EmisUserSession>();

            _systemUnderTest = _fixture.Create<EmisAppointmentsService>();
            
            _request = new AppointmentBookRequest
            {
                BookingReason = BookingReason,
                SlotId = SlotId,
            };
        }

        [TestMethod]
        public async Task Book_HappyPath_ReturnsSuccessfullyBookedResponse()
        {
            // Arrange
            var response = new EmisClient.EmisApiObjectResponse<BookAppointmentSlotPostResponse>(HttpStatusCode.OK)
            {
                Body = new BookAppointmentSlotPostResponse { BookingCreated = true },
                ErrorResponse = null,
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
            var errorResponse = _fixture.Create<ErrorResponse>();
            errorResponse.Exceptions.First().Message = EmisApiErrorMessages.AppointmentsPost_NotFound;
            
            var response = new EmisClient.EmisApiObjectResponse<BookAppointmentSlotPostResponse>(HttpStatusCode
                .InternalServerError) {ErrorResponse = errorResponse};
            
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
            var errorResponse = _fixture.Create<ErrorResponse>();
            errorResponse.Exceptions.First().Message = EmisApiErrorMessages.AppointmentsPost_InThePast;

            var response = new EmisClient.EmisApiObjectResponse<BookAppointmentSlotPostResponse>(HttpStatusCode
                .InternalServerError) { ErrorResponse = errorResponse };
            
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
        public async Task Book_WhenEmisReturnsForbidden_ReturnsSlotNotAvailable()
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
            var errorResponse = _fixture.Create<ErrorResponse>();
            errorResponse.Exceptions.First().Message = "Extra info: " + EmisApiErrorMessages.EmisService_NotEnabledForUser;
            
            //Arrange
            var response = new EmisClient.EmisApiObjectResponse<BookAppointmentSlotPostResponse>(HttpStatusCode
                .InternalServerError) { ErrorResponse = errorResponse };
            
            MockEmisClientAppointmentPostMethod(response);
            
            // Act            
            var result = await _systemUnderTest.Book(_userSession, _request);

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentBookResult.InsufficientPermissions>();
        }

        private void MockEmisClientAppointmentPostMethod(EmisClient.EmisApiObjectResponse<BookAppointmentSlotPostResponse> response)
        {
            _mockEmisClient.Setup(x => x.AppointmentsPost(
                    It.Is<EmisHeaderParameters>(p =>
                        p.EndUserSessionId == _userSession.EndUserSessionId && p.SessionId == _userSession.SessionId),
                    It.Is<BookAppointmentSlotPostRequest>(p =>
                        p.BookingReason == BookingReason
                        && p.SlotId == Convert.ToInt64(SlotId)
                        && p.UserPatientLinkToken == _userSession.UserPatientLinkToken
                    )
                )
            ).Returns(
                Task.FromResult(
                    response
                )
            ).Verifiable();;    
        }
    }
}
