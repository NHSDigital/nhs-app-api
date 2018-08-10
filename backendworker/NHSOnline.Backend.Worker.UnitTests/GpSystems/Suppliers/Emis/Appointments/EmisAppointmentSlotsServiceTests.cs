using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.Areas.Appointments.Models;
using NHSOnline.Backend.Worker.GpSystems.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models;
using NHSOnline.Backend.Worker.Support.Temporal;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Emis.Appointments
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
        private IAppointmentSlotsService _systemUnderTest;
        private Mock<IAppointmentSlotsResponseMapper> _mockResponseMapper;

        [TestInitialize]
        public void TestInitialize()
        {
            IConfigurationBuilder configBuilder = new ConfigurationBuilder();
            configBuilder.AddInMemoryCollection(new[] { new KeyValuePair<string, string>("TIMEZONE", "GMT Standard Time") });
            var timeZoneInfoProvider = new TimeZoneInfoProvider(configBuilder.Build());
            var dateTimeOffsetProvider = new DateTimeOffsetProvider(timeZoneInfoProvider);

            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _mockEmisClient = _fixture.Freeze<Mock<IEmisClient>>();
            _mockResponseMapper = _fixture.Freeze<Mock<IAppointmentSlotsResponseMapper>>();

            _userSession = new EmisUserSession()
            {
                UserPatientLinkToken = UserPatientLinkToken,
                EndUserSessionId = EndUserSessionId,
                SessionId = SessionId
            };

            _fromdDteTimeOffset = dateTimeOffsetProvider.CreateDateTimeOffset();
            _toDateTimeOffset = dateTimeOffsetProvider.CreateDateTimeOffset();

            _slotsMetadataGetQueryParameters =
                new SlotsMetadataGetQueryParameters(_fromdDteTimeOffset, _toDateTimeOffset, UserPatientLinkToken);
            _slotsGetQueryParameters =
                new SlotsGetQueryParameters(_fromdDteTimeOffset, _toDateTimeOffset, UserPatientLinkToken);


            _systemUnderTest = new EmisAppointmentSlotsService(
                _mockEmisClient.Object,
                new LoggerFactory().CreateLogger<EmisAppointmentSlotsService>(),
                _mockResponseMapper.Object);
        }

        [TestMethod]
        public async Task GetAppointmentSlotsResult_EmisClientThrowsHttpRequestExceptionFromAppointmentSlots_ReturnsbadRequest()
        {
            // Arrange
            var metadataResponse =
                new EmisClient.EmisApiObjectResponse<AppointmentSlotsMetadataGetResponse>(HttpStatusCode.OK);
            MockEmisClientAppointmentSlotsMetadataGetMethod(metadataResponse);
                
            _mockEmisClient
                .Setup(x => x.AppointmentSlotsGet(It.IsAny<EmisHeaderParameters>(), It.IsAny<SlotsGetQueryParameters>()))
                .Throws<HttpRequestException>()
                .Verifiable();

            // Act
            var result = await GetAppointmentSlotsResult();

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentSlotsResult.SupplierSystemUnavailable>();
        }
        
        [TestMethod]
        public async Task GetAppointmentSlotsResult_EmisClientThrowsHttpRequestExceptionFromAppointmentSlotsMetadata_SupplierSystemUnavailable()
        {
            // Arrange
            _mockEmisClient
                .Setup(x => x.AppointmentSlotsMetadataGet(It.IsAny<EmisHeaderParameters>(), It.IsAny<SlotsMetadataGetQueryParameters>()))
                .Throws<HttpRequestException>()
                .Verifiable();
            
            // Act
            var result = await GetAppointmentSlotsResult();

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentSlotsResult.SupplierSystemUnavailable>();
        }
        
        [TestMethod]
        public async Task GetAppointmentSlotsResult_EmisClientGetAppointmentSlotsUnsuccessful_ReturnsSupplierSystemUnavailable()
        {
            // Arrange
            var metadataResponse =
                new EmisClient.EmisApiObjectResponse<AppointmentSlotsMetadataGetResponse>(HttpStatusCode.OK);
            MockEmisClientAppointmentSlotsMetadataGetMethod(metadataResponse);
            
            var unsuccessfulSlotResponse = _fixture
                .Build<EmisClient.EmisApiObjectResponse<AppointmentSlotsGetResponse>>()
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
        public async Task GetAppointmentSlotsResult_EmisClientGetAppointmentSlotsMetadataUnsuccessful_ReturnsSupplierSystemUnavailable()
        {
            // Arrange
            var unsuccessfulMetadataResponse = _fixture
                .Build<EmisClient.EmisApiObjectResponse<AppointmentSlotsMetadataGetResponse>>()
                .With(x => x.StatusCode, HttpStatusCode.InternalServerError)
                .With(x => x.Body, null)
                .Create();
                
            MockEmisClientAppointmentSlotsMetadataGetMethod(unsuccessfulMetadataResponse);
            
            var slotsResponse = new EmisClient.EmisApiObjectResponse<AppointmentSlotsGetResponse>(HttpStatusCode.OK);
            MockEmisClientAppointmentSlotGetMethod(slotsResponse);
            
            // Act
            var result = await GetAppointmentSlotsResult();

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentSlotsResult.SupplierSystemUnavailable>();
        }
        
        [TestMethod]
        public async Task GetAppointmentSlotsResult_EmisClientGetAppointmentSlotsNotAvailable_ReturnsCannotBookAppointments()
        {
            // Arrange
            var metadataResponse =
                new EmisClient.EmisApiObjectResponse<AppointmentSlotsMetadataGetResponse>(HttpStatusCode.OK);
            MockEmisClientAppointmentSlotsMetadataGetMethod(metadataResponse);
            
            var errorResponse = _fixture.Create<ErrorResponse>();
            errorResponse.Exceptions.First().Message = "Extra info: " + EmisApiErrorMessages.EmisService_NotEnabledForUser;

            var slotsResponse = new EmisClient.EmisApiObjectResponse<AppointmentSlotsGetResponse>(HttpStatusCode
                .InternalServerError) { ErrorResponse = errorResponse };

            MockEmisClientAppointmentSlotGetMethod(slotsResponse);
            
            // Act
            var result = await GetAppointmentSlotsResult();

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentSlotsResult.CannotBookAppointments>();
        }
        
        [TestMethod]
        public async Task GetAppointmentSlotsResult_EmisClientGetAppointmentSlotsMetadataNotAvailable_ReturnsCannotBookAppointment()
        {
            // Arrange
            var errorResponse = _fixture.Create<ErrorResponse>();
            errorResponse.Exceptions.First().Message = "Extra info: " + EmisApiErrorMessages.EmisService_NotEnabledForUser;

            var errorMetadataResponse = new EmisClient.EmisApiObjectResponse<AppointmentSlotsMetadataGetResponse>(
                HttpStatusCode
                    .InternalServerError) { ErrorResponse = errorResponse };

            MockEmisClientAppointmentSlotsMetadataGetMethod(errorMetadataResponse);
            
            var slotsResponse = new EmisClient.EmisApiObjectResponse<AppointmentSlotsGetResponse>(HttpStatusCode.OK);
            MockEmisClientAppointmentSlotGetMethod(slotsResponse);

            // Act
            var result = await GetAppointmentSlotsResult();

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentSlotsResult.CannotBookAppointments>();
        }
        
        
        [TestMethod]
        public async Task Create_ReturnsSupplierSystemUnavailable_WhenSomethingGoesWrongDuringMappingResponse()
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

            var slotResponse = new EmisClient.EmisApiObjectResponse<AppointmentSlotsGetResponse>(HttpStatusCode.OK)
            {
                Body = new AppointmentSlotsGetResponse(),
                ErrorResponse = null,
                ErrorResponseBadRequest = null
            };

            MockEmisClientAppointmentSlotGetMethod(slotResponse);

            _mockResponseMapper.Setup(x => x.Map(slotResponse.Body, metadataResponse.Body))
                .Throws<Exception>();

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

            var slotResponse = new EmisClient.EmisApiObjectResponse<AppointmentSlotsGetResponse>(HttpStatusCode.OK)
            {
                Body = new AppointmentSlotsGetResponse(),
                ErrorResponse = null,
                ErrorResponseBadRequest = null
            };
            
            MockEmisClientAppointmentSlotGetMethod(slotResponse);

            var expectedResponse = _fixture.Create<AppointmentSlotsResponse>();

            _mockResponseMapper.Setup(x => x.Map(slotResponse.Body, metadataResponse.Body))
                .Returns(expectedResponse);

            // Act
            var result = await GetAppointmentSlotsResult();

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentSlotsResult.SuccessfullyRetrieved>();
        }
        
        private void MockEmisClientAppointmentSlotGetMethod(
            EmisClient.EmisApiObjectResponse<AppointmentSlotsGetResponse> response)
        {
            _mockEmisClient.Setup(x => x.AppointmentSlotsGet(
                    It.Is<EmisHeaderParameters>(p =>
                        p.EndUserSessionId.Equals(EndUserSessionId, StringComparison.Ordinal)
                        && p.SessionId.Equals(SessionId, StringComparison.Ordinal)),
                    It.Is<SlotsGetQueryParameters>(p =>
                        p.FromDateTime == _slotsGetQueryParameters.FromDateTime
                        && p.ToDateTime == _slotsGetQueryParameters.ToDateTime
                        && p.UserPatientLinkToken.Equals(UserPatientLinkToken, StringComparison.Ordinal)
                    )
                )
            )
            .ReturnsAsync(response)
            .Verifiable();     
        }
        
        private void MockEmisClientAppointmentSlotsMetadataGetMethod(
            EmisClient.EmisApiObjectResponse<AppointmentSlotsMetadataGetResponse> response)
        {
            _mockEmisClient.Setup(x => x.AppointmentSlotsMetadataGet(
                        It.Is<EmisHeaderParameters>(p =>
                            p.EndUserSessionId.Equals(EndUserSessionId, StringComparison.Ordinal)
                            && p.SessionId.Equals(SessionId, StringComparison.Ordinal)),
                        It.Is<SlotsMetadataGetQueryParameters>(p =>
                            p.SessionStartDate == _slotsMetadataGetQueryParameters.SessionStartDate
                            && p.SessionEndDate == _slotsMetadataGetQueryParameters.SessionEndDate
                            && p.UserPatientLinkToken.Equals(UserPatientLinkToken, StringComparison.Ordinal)
                        )
                    )
                )
                .ReturnsAsync(response)
                .Verifiable();     
        }

        private async Task<AppointmentSlotsResult> GetAppointmentSlotsResult()
        {
            return await _systemUnderTest.GetSlots(_userSession, _fromdDteTimeOffset, _toDateTimeOffset);
        }
    }
}
