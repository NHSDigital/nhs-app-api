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
using NHSOnline.Backend.Worker.Bridges.Emis.AppointmentSlots;
using NHSOnline.Backend.Worker.Bridges.Emis.Models;
using NHSOnline.Backend.Worker.Date;
using NHSOnline.Backend.Worker.Router.Appointments;
using Location = NHSOnline.Backend.Worker.Bridges.Emis.Models.Location;

namespace NHSOnline.Backend.Worker.UnitTests.Bridges.Emis
{
    [TestClass]
    public class EmisAppointmentSlotsServiceTests
    {
        private static IFixture _fixture;
        private Mock<IEmisClient> _mockEmisClient;
        private IDateTimeOffsetProvider _dateTimeOffsetProvider;
        private TimeZoneInfoProvider _timeZoneInfoProvider;
        private IAppointmentSlotsResponseMapper _appointmentSlotsResponseMapper;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _mockEmisClient = _fixture.Freeze<Mock<IEmisClient>>();
            _timeZoneInfoProvider = new TimeZoneInfoProvider();
            _dateTimeOffsetProvider = new DateTimeOffsetProvider(_timeZoneInfoProvider);
            _appointmentSlotsResponseMapper = new AppointmentSlotsResponseMapper(_dateTimeOffsetProvider);
        }

        [TestMethod]
        public async Task Get_EmisClientThrowsHttpRequestExceptionFromAppointmentSlots_ReturnsbadRequest()
        {
            // Arrange
            _mockEmisClient
                .Setup(x => x.AppointmentsSlotsMetadataGet(It.IsAny<EmisHeaderParameters>(),
                    It.IsAny<SlotsMetadataGetQueryParameters>()))
                .ReturnsAsync(
                    new EmisClient.EmisApiObjectResponse<AppointmentSlotsMetadataGetResponse>(HttpStatusCode.OK));
            // emis client throws HttpRequestException
            _mockEmisClient
                .Setup(x => x.AppointmentsSlotsGet(It.IsAny<EmisHeaderParameters>(), It.IsAny<SlotsGetQueryParameters>()))
                .Throws<HttpRequestException>()
                .Verifiable();
            
            var userSession = _fixture.Create<EmisUserSession>();
            var loggerFactory = _fixture.Create<ILoggerFactory>();

            var sut = new EmisAppointmentSlotsService(_mockEmisClient.Object, loggerFactory, _appointmentSlotsResponseMapper);

            // Act
            var result = await sut.Get(userSession, _dateTimeOffsetProvider.CreateDateTimeOffset(), _dateTimeOffsetProvider.CreateDateTimeOffset());

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentSlotsResult.SupplierSystemUnavailable>();
        }
        
        [TestMethod]
        public async Task Get_EmisClientThrowsHttpRequestExceptionFromAppointmentSlotsMetadata_ReturnsbadRequest()
        {
            // Arrange
            _mockEmisClient
                .Setup(x => x.AppointmentsSlotsGet(It.IsAny<EmisHeaderParameters>(),
                    It.IsAny<SlotsGetQueryParameters>()))
                .ReturnsAsync(
                    new EmisClient.EmisApiObjectResponse<AppointmentsSlotsGetResponse>(HttpStatusCode.OK));
            // emis client throws HttpRequestException
            _mockEmisClient
                .Setup(x => x.AppointmentsSlotsMetadataGet(It.IsAny<EmisHeaderParameters>(), It.IsAny<SlotsMetadataGetQueryParameters>()))
                .Throws<HttpRequestException>()
                .Verifiable();
            
            var userSession = _fixture.Create<EmisUserSession>();
            var loggerFactory = _fixture.Create<ILoggerFactory>();

            var sut = new EmisAppointmentSlotsService(_mockEmisClient.Object, loggerFactory, _appointmentSlotsResponseMapper);

            // Act
            var result = await sut.Get(userSession, _dateTimeOffsetProvider.CreateDateTimeOffset(), _dateTimeOffsetProvider.CreateDateTimeOffset());

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentSlotsResult.SupplierSystemUnavailable>();
        }
        
        [TestMethod]
        public async Task Get_EmisClientGetAppointmentSlotsUnsuccessful_ReturnsSupplierSystemUnavailable()
        {
            // Arrange
            _mockEmisClient
                .Setup(x => x.AppointmentsSlotsMetadataGet(It.IsAny<EmisHeaderParameters>(),
                    It.IsAny<SlotsMetadataGetQueryParameters>()))
                .ReturnsAsync(
                    new EmisClient.EmisApiObjectResponse<AppointmentSlotsMetadataGetResponse>(HttpStatusCode.OK));
            
            var unsuccessfulResponse = _fixture
                .Build<EmisClient.EmisApiObjectResponse<AppointmentsSlotsGetResponse>>()
                .With(x => x.StatusCode, HttpStatusCode.InternalServerError)
                .With(x => x.Body, null)
                .Create();

            _mockEmisClient
                .Setup(x => x.AppointmentsSlotsGet(It.IsAny<EmisHeaderParameters>(),
                    It.IsAny<SlotsGetQueryParameters>()))
                .Returns(Task.FromResult(unsuccessfulResponse))
                .Verifiable();

            var userSession = _fixture.Create<EmisUserSession>();
            var loggerFactory = _fixture.Create<ILoggerFactory>();
            var sut = new EmisAppointmentSlotsService(_mockEmisClient.Object, loggerFactory, _appointmentSlotsResponseMapper);
            
            // Act
            var result = await sut.Get(userSession, _dateTimeOffsetProvider.CreateDateTimeOffset(), _dateTimeOffsetProvider.CreateDateTimeOffset());

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentSlotsResult.SupplierSystemUnavailable>();
        }
        
        [TestMethod]
        public async Task Get_EmisClientGetAppointmentSlotsMetadataUnsuccessful_ReturnsSupplierSystemUnavailable()
        {
            // Arrange
            _mockEmisClient
                .Setup(x => x.AppointmentsSlotsGet(It.IsAny<EmisHeaderParameters>(),
                    It.IsAny<SlotsGetQueryParameters>()))
                .ReturnsAsync(
                    new EmisClient.EmisApiObjectResponse<AppointmentsSlotsGetResponse>(HttpStatusCode.OK));
            
            var unsuccessfulResponse = _fixture
                .Build<EmisClient.EmisApiObjectResponse<AppointmentSlotsMetadataGetResponse>>()
                .With(x => x.StatusCode, HttpStatusCode.InternalServerError)
                .With(x => x.Body, null)
                .Create();

            _mockEmisClient
                .Setup(x => x.AppointmentsSlotsMetadataGet(It.IsAny<EmisHeaderParameters>(),
                    It.IsAny<SlotsMetadataGetQueryParameters>()))
                .Returns(Task.FromResult(unsuccessfulResponse))
                .Verifiable();

            var userSession = _fixture.Create<EmisUserSession>();
            var loggerFactory = _fixture.Create<ILoggerFactory>();
            var sut = new EmisAppointmentSlotsService(_mockEmisClient.Object, loggerFactory, _appointmentSlotsResponseMapper);
            
            // Act
            var result = await sut.Get(userSession, _dateTimeOffsetProvider.CreateDateTimeOffset(), _dateTimeOffsetProvider.CreateDateTimeOffset());

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentSlotsResult.SupplierSystemUnavailable>();
        }
        
        [TestMethod]
        public async Task Get_EmisClientGetAppointmentSlotsNotAvaillable_ReturnsEmptyAppointmentsSlots()
        {
            // Arrange
            _mockEmisClient
                .Setup(x => x.AppointmentsSlotsMetadataGet(It.IsAny<EmisHeaderParameters>(),
                    It.IsAny<SlotsMetadataGetQueryParameters>()))
                .ReturnsAsync(
                    new EmisClient.EmisApiObjectResponse<AppointmentSlotsMetadataGetResponse>(HttpStatusCode.OK));
            
            var errorResponse = _fixture.Create<ErrorResponse>();
            errorResponse.Exceptions.First().Message = "Extra info: " + EmisApiErrorMessages.Appointments_NotEnabledOnEmisForUser;

            _mockEmisClient
                .Setup(x => x.AppointmentsSlotsGet(It.IsAny<EmisHeaderParameters>(),
                    It.IsAny<SlotsGetQueryParameters>()))
                .Returns(Task.FromResult(new EmisClient.EmisApiObjectResponse<AppointmentsSlotsGetResponse>(HttpStatusCode
                    .InternalServerError) {ErrorResponse = errorResponse}))
                .Verifiable();

            var userSession = _fixture.Create<EmisUserSession>();
            var loggerFactory = _fixture.Create<ILoggerFactory>();
            var sut = new EmisAppointmentSlotsService(_mockEmisClient.Object, loggerFactory, _appointmentSlotsResponseMapper);
            
            // Act
            
            var result = await sut.Get(userSession, _dateTimeOffsetProvider.CreateDateTimeOffset(), _dateTimeOffsetProvider.CreateDateTimeOffset());

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeEquivalentTo(new AppointmentSlotsResult.SuccessfullyRetrieved(new AppointmentSlotsResponse()));
        }
        
        [TestMethod]
        public async Task Get_EmisClientGetAppointmentSlotsMetadataNotAvaillable_ReturnsEmptyAppointmentsSlots()
        {
            // Arrange
            _mockEmisClient
                .Setup(x => x.AppointmentsSlotsGet(It.IsAny<EmisHeaderParameters>(),
                    It.IsAny<SlotsGetQueryParameters>()))
                .ReturnsAsync(
                    new EmisClient.EmisApiObjectResponse<AppointmentsSlotsGetResponse>(HttpStatusCode.OK));
            
            var errorResponse = _fixture.Create<ErrorResponse>();
            errorResponse.Exceptions.First().Message = "Extra info: " + EmisApiErrorMessages.Appointments_NotEnabledOnEmisForUser;

            _mockEmisClient
                .Setup(x => x.AppointmentsSlotsMetadataGet(It.IsAny<EmisHeaderParameters>(),
                    It.IsAny<SlotsMetadataGetQueryParameters>()))
                .Returns(Task.FromResult(new EmisClient.EmisApiObjectResponse<AppointmentSlotsMetadataGetResponse>(HttpStatusCode
                    .InternalServerError) {ErrorResponse = errorResponse}))
                .Verifiable();

            var userSession = _fixture.Create<EmisUserSession>();
            var loggerFactory = _fixture.Create<ILoggerFactory>();
            var sut = new EmisAppointmentSlotsService(_mockEmisClient.Object, loggerFactory, _appointmentSlotsResponseMapper);
            
            // Act
            
            var result = await sut.Get(userSession, _dateTimeOffsetProvider.CreateDateTimeOffset(), _dateTimeOffsetProvider.CreateDateTimeOffset());

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeEquivalentTo(new AppointmentSlotsResult.SuccessfullyRetrieved(new AppointmentSlotsResponse()));
        }
        
        
        [TestMethod]
        public async Task Create_ReturnsSupplierSystemUnavailable_WhenSomethingGoesWrongDuringMappingResponse()
        {
            // Arrange
            var appointmentSlotSession =
                CreateAppointmentsSlotSession(101, 1, "2018-05-09T10:59:19", "2018-05-09T10:59:19");
            
            var slotsResponse = new AppointmentsSlotsGetResponse
            {
                Sessions = new[]{ appointmentSlotSession}
            };
            
            var location = CreateLocation(23, "Lees");
            var sessionHolder = CreateSessionHolder(55, "Dr House");
            var session = CreateSession(location.LocationId, 1, "General Appointment Session");
            
            var slotsMetadataResponse = new AppointmentSlotsMetadataGetResponse
            {
                Locations = new[]{ location },
                SessionHolders = new[] { sessionHolder },
                Sessions = new[] { session }
            };

            _mockEmisClient.Setup(x => x.AppointmentsSlotsGet(It.IsAny<EmisHeaderParameters>(),
                It.IsAny<SlotsGetQueryParameters>())).Returns(
                Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<AppointmentsSlotsGetResponse>(HttpStatusCode.OK)
                    {
                        Body = slotsResponse,
                        ErrorResponse = null,
                        ErrorResponseBadRequest = null
                    }));
            
            _mockEmisClient.Setup(x => x.AppointmentsSlotsMetadataGet(It.IsAny<EmisHeaderParameters>(),
                It.IsAny<SlotsMetadataGetQueryParameters>())).Returns(
                Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<AppointmentSlotsMetadataGetResponse>(HttpStatusCode.OK)
                    {
                        Body = slotsMetadataResponse,
                        ErrorResponse = null,
                        ErrorResponseBadRequest = null
                    }));
            
            var userSession = _fixture.Create<EmisUserSession>();
            var loggerFactory = _fixture.Create<ILoggerFactory>();
            var sut = new EmisAppointmentSlotsService(_mockEmisClient.Object, loggerFactory, _appointmentSlotsResponseMapper);
            
            // Act
            var result = await sut.Get(userSession, _dateTimeOffsetProvider.CreateDateTimeOffset(), _dateTimeOffsetProvider.CreateDateTimeOffset());

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentSlotsResult.InternalServerError>();
        }

        [TestMethod]
        public async Task Create_HappyPath_ReturnsAppointmentSlots()
        {
            // Arrange
            _mockEmisClient.Setup(x => x.AppointmentsSlotsGet(It.IsAny<EmisHeaderParameters>(),
                It.IsAny<SlotsGetQueryParameters>())).Returns(
                Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<AppointmentsSlotsGetResponse>(HttpStatusCode.OK)
                    {
                        Body = new AppointmentsSlotsGetResponse(),
                        ErrorResponse = null,
                        ErrorResponseBadRequest = null
                    }));
            
            _mockEmisClient.Setup(x => x.AppointmentsSlotsMetadataGet(It.IsAny<EmisHeaderParameters>(),
                It.IsAny<SlotsMetadataGetQueryParameters>())).Returns(
                Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<AppointmentSlotsMetadataGetResponse>(HttpStatusCode.OK)
                    {
                        Body = new AppointmentSlotsMetadataGetResponse(),
                        ErrorResponse = null,
                        ErrorResponseBadRequest = null
                    }));
            
            var userSession = _fixture.Create<EmisUserSession>();
            var loggerFactory = _fixture.Create<ILoggerFactory>();
            var sut = new EmisAppointmentSlotsService(_mockEmisClient.Object, loggerFactory, _appointmentSlotsResponseMapper);
            
            // Act
            var result = await sut.Get(userSession, _dateTimeOffsetProvider.CreateDateTimeOffset(), _dateTimeOffsetProvider.CreateDateTimeOffset());

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentSlotsResult.SuccessfullyRetrieved>();
        }
        
        private AppointmentSlotSession CreateAppointmentsSlotSession(int slotId, int sessionId, string startTime, string endTime)
        {
            var appointmentSlot = new AppointmentSlot
            {
                SlotId = slotId,
                EndTime = endTime,
                StartTime = startTime,
            };
            
            var appointmentSlotSession = new AppointmentSlotSession
            {
                SessionId = sessionId,
                Slots = new[]{ appointmentSlot }
            };

            return appointmentSlotSession;
        }

        private Location CreateLocation(int id, string name)
        {
            return new Location
            {
                LocationId = id,
                LocationName = name
            };
        }

        private SessionHolder CreateSessionHolder(int id, string name)
        {
            return new SessionHolder
            {
                ClinicianId = id,
                DisplayName = name
            };
        }

        private Worker.Bridges.Emis.Models.Session CreateSession(int locationId, int sessionId, string name)
        {
            return new Worker.Bridges.Emis.Models.Session
            {
                LocationId = locationId,
                SessionId = sessionId,
                SessionName = name
            };
        }
    }
}
