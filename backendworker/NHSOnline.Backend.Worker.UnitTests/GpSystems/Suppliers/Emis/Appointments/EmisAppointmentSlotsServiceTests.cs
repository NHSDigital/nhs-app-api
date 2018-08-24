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
        private AppointmentSlotsDateRange _dateRange;
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
            configBuilder.AddInMemoryCollection(new[] { new KeyValuePair<string, string>("TIMEZONE", TimeZoneResolver.GetTimeZoneNameForCurrentOS()) });
            var timeZoneInfoProvider = new TimeZoneInfoProvider(configBuilder.Build());
            var dateTimeOffsetProvider = new DateTimeOffsetProvider(timeZoneInfoProvider);

            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _mockEmisClient = _fixture.Freeze<Mock<IEmisClient>>();
            _mockResponseMapper = _fixture.Freeze<Mock<IAppointmentSlotsResponseMapper>>();

            _userSession = new EmisUserSession()
            {
                UserPatientLinkToken = UserPatientLinkToken,
                EndUserSessionId = EndUserSessionId,
                SessionId = SessionId,
                OdsCode = "TestOds"
            };

            _fromdDteTimeOffset = dateTimeOffsetProvider.CreateDateTimeOffset();
            _toDateTimeOffset = dateTimeOffsetProvider.CreateDateTimeOffset();

            _dateRange = new AppointmentSlotsDateRange(
                dateTimeOffsetProvider,
                _fromdDteTimeOffset,
                _toDateTimeOffset
            );

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
        public async Task GetAppointmentSlotsResult_EmisClientThrowsHttpRequestExceptionFromAppointmentSlots_ReturnsSupplierSystemUnavailable()
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
        public async Task GetAppointmentSlotsResult_EmisClientThrowsHttpRequestExceptionFromPracticewSettings_ReturnsSupplierSystemUnavailable()
        {
            // Arrange
            MockEmisClientAppointmentSlotsMetadataGetMethod(new EmisClient.EmisApiObjectResponse<AppointmentSlotsMetadataGetResponse>(HttpStatusCode.OK));
            MockEmisClientAppointmentSlotGetMethod(new EmisClient.EmisApiObjectResponse<AppointmentSlotsGetResponse>(HttpStatusCode.OK));

            _mockEmisClient
                .Setup(x => x.PracticeSettingsGet(It.IsAny<EmisHeaderParameters>(), It.IsAny<string>()))
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

            MockEmisClientPracticeSettingsGetMethod(new EmisClient.EmisApiObjectResponse<PracticeSettingsGetResponse>(HttpStatusCode.OK));

            var unsuccessfulSlotResponse = _fixture
                .Build<EmisClient.EmisApiObjectResponse<AppointmentSlotsGetResponse>>()
                .With(x => x.StatusCode, HttpStatusCode.InternalServerError)
                .With(x => x.Body, null)
                .Create();
            
            MockEmisClientAppointmentSlotGetMethod(unsuccessfulSlotResponse);

            MockEmisClientPracticeSettingsGetMethod(new EmisClient.EmisApiObjectResponse<PracticeSettingsGetResponse>(HttpStatusCode.OK));

            // Act
            var result = await GetAppointmentSlotsResult();

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentSlotsResult.SupplierSystemUnavailable>();
        }

        [TestMethod]
        public async Task GetAppointmentSlotsResult_EmisClientGetPracticeSettingsError_ReturnsSuccessfullyAnyway()
        {
            // Arrange
            var unsuccessfulPracticeSettingsResponse = _fixture
                .Build<EmisClient.EmisApiObjectResponse<PracticeSettingsGetResponse>>()
                .With(x => x.StatusCode, HttpStatusCode.InternalServerError)
                .With(x => x.Body, null)
                .Create();

            MockEmisClientAppointmentSlotsMetadataGetMethod(new EmisClient.EmisApiObjectResponse<AppointmentSlotsMetadataGetResponse>(HttpStatusCode.OK));
            MockEmisClientAppointmentSlotGetMethod(new EmisClient.EmisApiObjectResponse<AppointmentSlotsGetResponse>(HttpStatusCode.OK));
            MockEmisClientPracticeSettingsGetMethod(unsuccessfulPracticeSettingsResponse);

            // Act
            var result = await GetAppointmentSlotsResult();

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentSlotsResult.SuccessfullyRetrieved>();
        }

        [TestMethod]
        public async Task GetAppointmentSlotsResult_EmisClientGetPracticeSettingsUnsuccessful_ReturnsSuccessfullyAnyway()
        {
            // Arrange
            var errorResponse = _fixture.Create<ExceptionErrorResponse>();
            errorResponse.Exceptions.First().Message = "Extra info: " + EmisApiErrorMessages.EmisService_NotEnabledForUser;
            var unsuccessfulPracticeSettingsResponse = new EmisClient.EmisApiObjectResponse<PracticeSettingsGetResponse>(HttpStatusCode
                    .InternalServerError)
                {
                    ExceptionErrorResponse = errorResponse
                };

            MockEmisClientAppointmentSlotsMetadataGetMethod(new EmisClient.EmisApiObjectResponse<AppointmentSlotsMetadataGetResponse>(HttpStatusCode.OK));
            MockEmisClientAppointmentSlotGetMethod(new EmisClient.EmisApiObjectResponse<AppointmentSlotsGetResponse>(HttpStatusCode.OK));
            MockEmisClientPracticeSettingsGetMethod(unsuccessfulPracticeSettingsResponse);

            // Act
            var result = await GetAppointmentSlotsResult();

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentSlotsResult.SuccessfullyRetrieved>();
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

            MockEmisClientPracticeSettingsGetMethod(new EmisClient.EmisApiObjectResponse<PracticeSettingsGetResponse>(HttpStatusCode.OK));

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

            var errorResponse = _fixture.Create<StandardErrorResponse>();

            var slotsResponse = new EmisClient.EmisApiObjectResponse<AppointmentSlotsGetResponse>(HttpStatusCode
                .Forbidden) { StandardErrorResponse = errorResponse };

            MockEmisClientPracticeSettingsGetMethod(
                new EmisClient.EmisApiObjectResponse<PracticeSettingsGetResponse>(HttpStatusCode.OK));

            MockEmisClientAppointmentSlotGetMethod(slotsResponse);

            // Act
            var result = await GetAppointmentSlotsResult();

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentSlotsResult.CannotBookAppointments>();
        }
        
        [TestMethod]
        public async Task GetAppointmentSlotsResult_EmisClientGetAppointmentSlotsNotAvailableException_ReturnsCannotBookAppointments()
        {
            // Arrange
            var metadataResponse =
                new EmisClient.EmisApiObjectResponse<AppointmentSlotsMetadataGetResponse>(HttpStatusCode.OK);
            MockEmisClientAppointmentSlotsMetadataGetMethod(metadataResponse);
            
            var errorResponse = _fixture.Create<ExceptionErrorResponse>();
            errorResponse.Exceptions.First().Message = "Extra info: " + EmisApiErrorMessages.EmisService_NotEnabledForUser;

            var slotsResponse = new EmisClient.EmisApiObjectResponse<AppointmentSlotsGetResponse>(HttpStatusCode
                .InternalServerError) { ExceptionErrorResponse = errorResponse };

            MockEmisClientAppointmentSlotGetMethod(slotsResponse);

            MockEmisClientPracticeSettingsGetMethod(new EmisClient.EmisApiObjectResponse<PracticeSettingsGetResponse>(HttpStatusCode.OK));

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
            var errorResponse = _fixture.Create<StandardErrorResponse>();

            var errorMetadataResponse = new EmisClient.EmisApiObjectResponse<AppointmentSlotsMetadataGetResponse>(
                HttpStatusCode
                    .Forbidden) { StandardErrorResponse = errorResponse };

            MockEmisClientAppointmentSlotsMetadataGetMethod(errorMetadataResponse);

            var slotsResponse = new EmisClient.EmisApiObjectResponse<AppointmentSlotsGetResponse>(HttpStatusCode.OK);
            MockEmisClientAppointmentSlotGetMethod(slotsResponse);

            MockEmisClientPracticeSettingsGetMethod(
                new EmisClient.EmisApiObjectResponse<PracticeSettingsGetResponse>(HttpStatusCode.OK));

            // Act
            var result = await GetAppointmentSlotsResult();

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentSlotsResult.CannotBookAppointments>();
        }



        [TestMethod]
        public async Task GetAppointmentSlotsResult_EmisClientGetAppointmentSlotsMetadataNotAvailableException_ReturnsCannotBookAppointment()
        {
            // Arrange
            var errorResponse = _fixture.Create<ExceptionErrorResponse>();
            errorResponse.Exceptions.First().Message = "Extra info: " + EmisApiErrorMessages.EmisService_NotEnabledForUser;

            var errorMetadataResponse = new EmisClient.EmisApiObjectResponse<AppointmentSlotsMetadataGetResponse>(
                HttpStatusCode
                    .InternalServerError) { ExceptionErrorResponse = errorResponse };

            MockEmisClientAppointmentSlotsMetadataGetMethod(errorMetadataResponse);

            MockEmisClientPracticeSettingsGetMethod(new EmisClient.EmisApiObjectResponse<PracticeSettingsGetResponse>(HttpStatusCode.OK));

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
                    ExceptionErrorResponse = null,
                    ErrorResponseBadRequest = null
                };

            MockEmisClientAppointmentSlotsMetadataGetMethod(metadataResponse);

            MockEmisClientPracticeSettingsGetMethod(new EmisClient.EmisApiObjectResponse<PracticeSettingsGetResponse>(HttpStatusCode.OK));

            var slotResponse = new EmisClient.EmisApiObjectResponse<AppointmentSlotsGetResponse>(HttpStatusCode.OK)
            {
                Body = new AppointmentSlotsGetResponse(),
                ExceptionErrorResponse = null,
                ErrorResponseBadRequest = null
            };

            MockEmisClientAppointmentSlotGetMethod(slotResponse);

            _mockResponseMapper.Setup(x => x.Map(slotResponse.Body, metadataResponse.Body, It.IsAny<PracticeSettingsGetResponse>()))
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
                    ExceptionErrorResponse = null,
                    ErrorResponseBadRequest = null
                };
            
            MockEmisClientAppointmentSlotsMetadataGetMethod(metadataResponse);
            MockEmisClientPracticeSettingsGetMethod(new EmisClient.EmisApiObjectResponse<PracticeSettingsGetResponse>(HttpStatusCode.OK));

            var slotResponse = new EmisClient.EmisApiObjectResponse<AppointmentSlotsGetResponse>(HttpStatusCode.OK)
            {
                Body = new AppointmentSlotsGetResponse(),
                ExceptionErrorResponse = null,
                ErrorResponseBadRequest = null
            };
            
            MockEmisClientAppointmentSlotGetMethod(slotResponse);

            var expectedResponse = _fixture.Create<AppointmentSlotsResponse>();

            _mockResponseMapper.Setup(x => x.Map(slotResponse.Body, metadataResponse.Body, It.IsAny<PracticeSettingsGetResponse>()))
                .Returns(expectedResponse);

            // Act
            var result = await GetAppointmentSlotsResult();

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentSlotsResult.SuccessfullyRetrieved>();
        }

        private void MockEmisClientPracticeSettingsGetMethod(
            EmisClient.EmisApiObjectResponse<PracticeSettingsGetResponse> response)
        {
            _mockEmisClient.Setup(x => x.PracticeSettingsGet(It.Is<EmisHeaderParameters>(p =>
                    p.EndUserSessionId.Equals(EndUserSessionId, StringComparison.Ordinal)
                    && p.SessionId.Equals(SessionId, StringComparison.Ordinal)), "TestOds"))
                .ReturnsAsync(response)
                .Verifiable();
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
            return await _systemUnderTest.GetSlots(_userSession, _dateRange);
        }
    }
}
