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
using NHSOnline.Backend.Worker.Areas.Appointments.Models;
using NHSOnline.Backend.Worker.Bridges.Emis;
using NHSOnline.Backend.Worker.Bridges.Emis.Models;
using NHSOnline.Backend.Worker.Router.Appointments;

namespace NHSOnline.Backend.Worker.UnitTests.Bridges.Emis
{
    [TestClass]
    public class EmisAppointmentsServiceTests
    {
        private static IFixture _fixture;
        private Mock<IEmisClient> _mockEmisClient;
        private IAppointmentsService _sut;
        private AppointmentBookRequest _request;
        private EmisUserSession _userSession;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _mockEmisClient = _fixture.Freeze<Mock<IEmisClient>>();
            _userSession = _fixture.Create<EmisUserSession>();
            
            var loggerFactory = _fixture.Create<ILoggerFactory>();
            
            _sut = new EmisAppointmentsService(_mockEmisClient.Object, loggerFactory);
            
            _request = new AppointmentBookRequest
            {
                BookingReason = "I caught a cold!",
                SlotId = "2862517",
            };
        }

        [TestMethod]
        public async Task Book_HappyPath_ReturnsSuccessfullyBookedResponse()
        {
            // Arrange
            _mockEmisClient.Setup(x => x.AppointmentPost(It.IsAny<EmisHeaderParameters>(),
                It.IsAny<BookAppointmentSlotPostRequest>())).Returns(
                Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<BookAppointmentSlotPostResponse>(HttpStatusCode.OK)
                    {
                        Body = new BookAppointmentSlotPostResponse(),
                        ErrorResponse = null,
                        ErrorResponseBadRequest = null
                    }));
            
            // Act            
            var result = await _sut.Book(_userSession, _request);

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentBookResult.SuccessfullyBooked>();
        }

        [TestMethod]
        public async Task Book_EmisClientThrowsHttpRequestExceptionFromAppointments_ReturnsSupplierSystemUnavailable()
        {
            // Arrange
            _mockEmisClient.Setup(x => x.AppointmentPost(It.IsAny<EmisHeaderParameters>(),
                    It.IsAny<BookAppointmentSlotPostRequest>())).
                Throws<HttpRequestException>()
                .Verifiable();
            
            // Act            
            var result = await _sut.Book(_userSession, _request);

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
            
            _mockEmisClient
                .Setup(x => x.AppointmentPost(It.IsAny<EmisHeaderParameters>(),
                    It.IsAny<BookAppointmentSlotPostRequest>()))
                .Returns(Task.FromResult(new EmisClient.EmisApiObjectResponse<BookAppointmentSlotPostResponse>(HttpStatusCode
                    .InternalServerError) {ErrorResponse = errorResponse}))
                .Verifiable();
            
            // Act            
            var result = await _sut.Book(_userSession, _request);

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
            
            _mockEmisClient
                .Setup(x => x.AppointmentPost(It.IsAny<EmisHeaderParameters>(),
                    It.IsAny<BookAppointmentSlotPostRequest>()))
                .Returns(Task.FromResult(new EmisClient.EmisApiObjectResponse<BookAppointmentSlotPostResponse>(HttpStatusCode
                    .InternalServerError) {ErrorResponse = errorResponse}))
                .Verifiable();
            
            // Act            
            var result = await _sut.Book(_userSession, _request);

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentBookResult.SlotNotAvailable>();
        }
        
        [TestMethod]
        public async Task Book_WhenAppointmentsHasBeenAlreadyBooked_ReturnsSlotNotAvailable()
        {   
            _mockEmisClient
                .Setup(x => x.AppointmentPost(It.IsAny<EmisHeaderParameters>(),
                    It.IsAny<BookAppointmentSlotPostRequest>()))
                .Returns(Task.FromResult(new EmisClient.EmisApiObjectResponse<BookAppointmentSlotPostResponse>(HttpStatusCode
                    .Conflict)))
                .Verifiable();
            
            // Act            
            var result = await _sut.Book(_userSession, _request);

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentBookResult.SlotNotAvailable>();
        }
        
        [TestMethod]
        public async Task Book_WhenPatientDoesNotHaveNecessaryPermissions_ReturnsSlotNotAvailable()
        {   
            _mockEmisClient
                .Setup(x => x.AppointmentPost(It.IsAny<EmisHeaderParameters>(),
                    It.IsAny<BookAppointmentSlotPostRequest>()))
                .Returns(Task.FromResult(new EmisClient.EmisApiObjectResponse<BookAppointmentSlotPostResponse>(HttpStatusCode
                    .Forbidden)))
                .Verifiable();
            
            // Act            
            var result = await _sut.Book(_userSession, _request);

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentBookResult.InsufficientPermissions>();
        }
    }
}
