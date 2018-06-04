using System;
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
using NHSOnline.Backend.Worker.Router.Appointments;
using NHSOnline.Backend.Worker.Support.Date;
using Location = NHSOnline.Backend.Worker.Bridges.Emis.Models.Location;

namespace NHSOnline.Backend.Worker.UnitTests.Bridges.Emis
{
    [TestClass]
    public class EmisAppointmentSlotsServiceTests
    {
        private const string UserPatientLinkToken = "USER_PATIENT_LINK_TOKEN";
        private const string EndUserSessionId = "END_USER_SESSION_ID";
        private const string SessionId = "SESSION_ID";
        
        private IFixture _fixture;
        private Mock<IEmisClient> _mockEmisClient;
        private EmisUserSession _userSession;
        private DateTimeOffset _fromdDteTimeOffset;
        private DateTimeOffset _toDateTimeOffset;
        private SlotsMetadataGetQueryParameters _slotsMetadataGetQueryParameters;
        private SlotsGetQueryParameters _slotsGetQueryParameters;
        private IAppointmentSlotsService _sut;

        [TestInitialize]
        public void TestInitialize()
        {
            var timeZoneInfoProvider = new TimeZoneInfoProvider();
            var dateTimeOffsetProvider = new DateTimeOffsetProvider(timeZoneInfoProvider);
            var appointmentSlotsResponseMapper = new AppointmentSlotsResponseMapper(dateTimeOffsetProvider);
            
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _mockEmisClient = _fixture.Freeze<Mock<IEmisClient>>();
            
            _userSession = new EmisUserSession()
            {
                UserPatientLinkToken = UserPatientLinkToken,
                EndUserSessionId = EndUserSessionId,
                SessionId = SessionId
            };
            
            _fromdDteTimeOffset = dateTimeOffsetProvider.CreateDateTimeOffset();
            _toDateTimeOffset = dateTimeOffsetProvider.CreateDateTimeOffset();

            _slotsMetadataGetQueryParameters = new SlotsMetadataGetQueryParameters()
            {
                SessionStartDate = _fromdDteTimeOffset,
                SessionEndDate = _toDateTimeOffset,
                UserPatientLinkToken = UserPatientLinkToken
            };
            
            _slotsGetQueryParameters = new SlotsGetQueryParameters()
            {
                FromDateTime = _fromdDteTimeOffset,
                ToDateTime = _toDateTimeOffset,
                UserPatientLinkToken = UserPatientLinkToken
            };
            
            _sut = new EmisAppointmentSlotsService(_mockEmisClient.Object, new LoggerFactory(), appointmentSlotsResponseMapper);
        }

        [TestMethod]
        public async Task Get_EmisClientThrowsHttpRequestExceptionFromAppointmentSlots_ReturnsbadRequest()
        {
            // Arrange
            var metadataResponse =
                new EmisClient.EmisApiObjectResponse<AppointmentSlotsMetadataGetResponse>(HttpStatusCode.OK);
            MockEmisClientAppointmentSlotsMetadataGetMethod(metadataResponse);
                
            _mockEmisClient
                .Setup(x => x.AppointmentsSlotsGet(It.IsAny<EmisHeaderParameters>(), It.IsAny<SlotsGetQueryParameters>()))
                .Throws<HttpRequestException>()
                .Verifiable();

            // Act
            var result = await GetAppointmentSlotsResult();

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentSlotsResult.SupplierSystemUnavailable>();
        }
        
        [TestMethod]
        public async Task Get_EmisClientThrowsHttpRequestExceptionFromAppointmentSlotsMetadata_SupplierSystemUnavailable()
        {
            // Arrange
            _mockEmisClient
                .Setup(x => x.AppointmentsSlotsMetadataGet(It.IsAny<EmisHeaderParameters>(), It.IsAny<SlotsMetadataGetQueryParameters>()))
                .Throws<HttpRequestException>()
                .Verifiable();
            
            // Act
            var result = await GetAppointmentSlotsResult();

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentSlotsResult.SupplierSystemUnavailable>();
        }
        
        [TestMethod]
        public async Task Get_EmisClientGetAppointmentSlotsUnsuccessful_ReturnsSupplierSystemUnavailable()
        {
            // Arrange
            var metadataResponse =
                new EmisClient.EmisApiObjectResponse<AppointmentSlotsMetadataGetResponse>(HttpStatusCode.OK);
            MockEmisClientAppointmentSlotsMetadataGetMethod(metadataResponse);
            
            var unsuccessfulSlotResponse = _fixture
                .Build<EmisClient.EmisApiObjectResponse<AppointmentsSlotsGetResponse>>()
                .With(x => x.StatusCode, HttpStatusCode.InternalServerError)
                .With(x => x.Body, null)
                .Create();
            
            MockEmisClientAppointmentSlotGetMethod(unsuccessfulSlotResponse);
            
            // Act
            var result = await GetAppointmentSlotsResult();

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentSlotsResult.SupplierSystemUnavailable>();
        }
        
        [TestMethod]
        public async Task Get_EmisClientGetAppointmentSlotsMetadataUnsuccessful_ReturnsSupplierSystemUnavailable()
        {
            // Arrange
            var unsuccessfulMetadataResponse = _fixture
                .Build<EmisClient.EmisApiObjectResponse<AppointmentSlotsMetadataGetResponse>>()
                .With(x => x.StatusCode, HttpStatusCode.InternalServerError)
                .With(x => x.Body, null)
                .Create();
                
            MockEmisClientAppointmentSlotsMetadataGetMethod(unsuccessfulMetadataResponse);
            
            var slotsResponse = new EmisClient.EmisApiObjectResponse<AppointmentsSlotsGetResponse>(HttpStatusCode.OK);
            MockEmisClientAppointmentSlotGetMethod(slotsResponse);
            
            // Act
            var result = await GetAppointmentSlotsResult();

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentSlotsResult.SupplierSystemUnavailable>();
        }
        
        [TestMethod]
        public async Task Get_EmisClientGetAppointmentSlotsNotAvaillable_ReturnsEmptyAppointmentsSlots()
        {
            // Arrange
            var metadataResponse =
                new EmisClient.EmisApiObjectResponse<AppointmentSlotsMetadataGetResponse>(HttpStatusCode.OK);
            MockEmisClientAppointmentSlotsMetadataGetMethod(metadataResponse);
            
            var errorResponse = _fixture.Create<ErrorResponse>();
            errorResponse.Exceptions.First().Message = "Extra info: " + EmisApiErrorMessages.EmisService_NotEnabledForUser;

            var slotsResponse = new EmisClient.EmisApiObjectResponse<AppointmentsSlotsGetResponse>(HttpStatusCode
                .InternalServerError) { ErrorResponse = errorResponse };

            MockEmisClientAppointmentSlotGetMethod(slotsResponse);
            
            // Act
            var result = await GetAppointmentSlotsResult();

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeEquivalentTo(new AppointmentSlotsResult.SuccessfullyRetrieved(new AppointmentSlotsResponse()));
        }
        
        [TestMethod]
        public async Task Get_EmisClientGetAppointmentSlotsMetadataNotAvaillable_ReturnsEmptyAppointmentsSlots()
        {
            // Arrange
            var errorResponse = _fixture.Create<ErrorResponse>();
            errorResponse.Exceptions.First().Message = "Extra info: " + EmisApiErrorMessages.EmisService_NotEnabledForUser;

            var errorMetadataResponse = new EmisClient.EmisApiObjectResponse<AppointmentSlotsMetadataGetResponse>(
                HttpStatusCode
                    .InternalServerError) { ErrorResponse = errorResponse };

            MockEmisClientAppointmentSlotsMetadataGetMethod(errorMetadataResponse);
            
            var slotsResponse = new EmisClient.EmisApiObjectResponse<AppointmentsSlotsGetResponse>(HttpStatusCode.OK);
            MockEmisClientAppointmentSlotGetMethod(slotsResponse);

            // Act
            var result = await GetAppointmentSlotsResult();

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
            var session = CreateSession(location.LocationId, 1, "Timed");
            
            var slotsMetadataResponse = new AppointmentSlotsMetadataGetResponse
            {
                Locations = new[]{ location },
                SessionHolders = new[] { sessionHolder },
                Sessions = new[] { session }
            };

            var metadataResponse =
                new EmisClient.EmisApiObjectResponse<AppointmentSlotsMetadataGetResponse>(HttpStatusCode.OK)
                {
                    Body = slotsMetadataResponse,
                    ErrorResponse = null,
                    ErrorResponseBadRequest = null
                };
            
            MockEmisClientAppointmentSlotsMetadataGetMethod(metadataResponse);

            var slotResponse = new EmisClient.EmisApiObjectResponse<AppointmentsSlotsGetResponse>(HttpStatusCode.OK)
            {
                Body = slotsResponse,
                ErrorResponse = null,
                ErrorResponseBadRequest = null
            };

            MockEmisClientAppointmentSlotGetMethod(slotResponse);

            // Act
            var result = await GetAppointmentSlotsResult();

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentSlotsResult.InternalServerError>();
        }

        [TestMethod]
        public async Task Create_HappyPath_ReturnsAppointmentSlots()
        {
            // Arrange
            var metadataResponse =
                new EmisClient.EmisApiObjectResponse<AppointmentSlotsMetadataGetResponse>(HttpStatusCode.OK)
                {
                    Body = new AppointmentSlotsMetadataGetResponse(),
                    ErrorResponse = null,
                    ErrorResponseBadRequest = null
                };
            
            MockEmisClientAppointmentSlotsMetadataGetMethod(metadataResponse);

            var slotResponse = new EmisClient.EmisApiObjectResponse<AppointmentsSlotsGetResponse>(HttpStatusCode.OK)
            {
                Body = new AppointmentsSlotsGetResponse(),
                ErrorResponse = null,
                ErrorResponseBadRequest = null
            };
            
            MockEmisClientAppointmentSlotGetMethod(slotResponse);

            // Act
            var result = await GetAppointmentSlotsResult();

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentSlotsResult.SuccessfullyRetrieved>();
        }
        
        private void MockEmisClientAppointmentSlotGetMethod(
            EmisClient.EmisApiObjectResponse<AppointmentsSlotsGetResponse> response)
        {
            _mockEmisClient.Setup(x => x.AppointmentsSlotsGet(
                    It.Is<EmisHeaderParameters>(p =>
                        p.EndUserSessionId == EndUserSessionId && p.SessionId == SessionId),
                    It.Is<SlotsGetQueryParameters>(p =>
                        p.FromDateTime == _slotsGetQueryParameters.FromDateTime
                        && p.ToDateTime == _slotsGetQueryParameters.ToDateTime
                        && p.UserPatientLinkToken == UserPatientLinkToken
                    )
                )
            )
            .ReturnsAsync(response)
            .Verifiable();     
        }
        
        private void MockEmisClientAppointmentSlotsMetadataGetMethod(
            EmisClient.EmisApiObjectResponse<AppointmentSlotsMetadataGetResponse> response)
        {
            _mockEmisClient.Setup(x => x.AppointmentsSlotsMetadataGet(
                        It.Is<EmisHeaderParameters>(p =>
                            p.EndUserSessionId == EndUserSessionId && p.SessionId == SessionId),
                        It.Is<SlotsMetadataGetQueryParameters>(p =>
                            p.SessionStartDate == _slotsMetadataGetQueryParameters.SessionStartDate
                            && p.SessionEndDate == _slotsMetadataGetQueryParameters.SessionEndDate
                            && p.UserPatientLinkToken == UserPatientLinkToken
                        )
                    )
                )
                .ReturnsAsync(response)
                .Verifiable();     
        }

        private async Task<AppointmentSlotsResult> GetAppointmentSlotsResult()
        {
            return await _sut.Get(_userSession, _fromdDteTimeOffset, _toDateTimeOffset);
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

        private Session CreateSession(int locationId, int sessionId, string sessionType)
        {
            return new Session
            {
                LocationId = locationId,
                SessionId = sessionId,
                SessionType = sessionType
            };
        }
    }
}
