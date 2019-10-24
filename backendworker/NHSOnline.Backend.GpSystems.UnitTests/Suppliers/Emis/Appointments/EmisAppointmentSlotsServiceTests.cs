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
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Temporal;
using UnitTestHelper;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Emis.Appointments
{
    [TestClass]
    public class EmisAppointmentSlotsServiceTests
    {
        private IFixture _fixture;
        private Mock<IEmisClient> _mockEmisClient;
        private EmisUserSession _emisUserSession;
        private AppointmentSlotsDateRange _dateRange;
        private DateTimeOffset _fromDateTimeOffset;
        private DateTimeOffset _toDateTimeOffset;
        private SlotsMetadataGetQueryParameters _slotsMetadataGetQueryParameters;
        private SlotsGetQueryParameters _slotsGetQueryParameters;
        private Mock<IAppointmentSlotsResponseMapper> _mockResponseMapper;
        private GpLinkedAccountModel _gpLinkedAccountModel;

        private EmisAppointmentSlotsService _systemUnderTest;
        private Mock<ICurrentDateTimeProvider> _mockCurrentDateTimeProvider;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _mockCurrentDateTimeProvider = _fixture.Freeze<Mock<ICurrentDateTimeProvider>>();
            _mockCurrentDateTimeProvider.SetupGet(x => x.UtcNow)
                .Returns(DateTime.UtcNow);

            IConfigurationBuilder configBuilder = new ConfigurationBuilder();
            configBuilder.AddInMemoryCollection(new[] { new KeyValuePair<string, string>("TIMEZONE", TimeZoneResolver.GetTimeZoneNameForCurrentOperatingSystemPlatform()) });
            var timeZoneInfoProvider = new TimeZoneInfoProvider(new Mock<ILogger<TimeZoneInfoProvider>>().Object, configBuilder.Build());
            var dateTimeOffsetProvider = new DateTimeOffsetProvider(timeZoneInfoProvider, _mockCurrentDateTimeProvider.Object);

            _mockEmisClient = _fixture.Freeze<Mock<IEmisClient>>();
            _mockResponseMapper = _fixture.Freeze<Mock<IAppointmentSlotsResponseMapper>>();

            _emisUserSession = _fixture.Create<EmisUserSession>();

            _gpLinkedAccountModel = new GpLinkedAccountModel(_emisUserSession, Guid.NewGuid());

            _fromDateTimeOffset = dateTimeOffsetProvider.CreateDateTimeOffset();
            _toDateTimeOffset = dateTimeOffsetProvider.CreateDateTimeOffset();

            _dateRange = new AppointmentSlotsDateRange(_fromDateTimeOffset, _toDateTimeOffset);

            _slotsMetadataGetQueryParameters =
                new SlotsMetadataGetQueryParameters(_fromDateTimeOffset, _toDateTimeOffset);
            _slotsGetQueryParameters =
                new SlotsGetQueryParameters(_fromDateTimeOffset, _toDateTimeOffset);

            _systemUnderTest = new EmisAppointmentSlotsService(
                _mockEmisClient.Object,
                new LoggerFactory().CreateLogger<EmisAppointmentSlotsService>(),
                _mockResponseMapper.Object);
        }

        [TestMethod]
        public async Task GetSlots_EmisClientAppointmentSlotsGetThrowsHttpRequestException_ReturnsBadGateway()
        {
            // Arrange
            MockEmisClientAppointmentSlotsMetadataGetMethod(new EmisClient.EmisApiObjectResponse<AppointmentSlotsMetadataGetResponse>(HttpStatusCode.OK));

            _mockEmisClient
                .Setup(x => x.AppointmentSlotsGet(It.IsAny<EmisRequestParameters>(), It.IsAny<SlotsGetQueryParameters>()))
                .ThrowsAsync(new HttpRequestException())
                .Verifiable();

            // Act
            var result = await _systemUnderTest.GetSlots(_gpLinkedAccountModel, _dateRange);

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentSlotsResult.BadGateway>();
        }

        [TestMethod]
        public async Task GetSlots_EmisClientAppointmentSlotsGetUnsuccessful_ReturnsBadGateway()
        {
            // Arrange
            var metadataResponse =
                new EmisClient.EmisApiObjectResponse<AppointmentSlotsMetadataGetResponse>(HttpStatusCode.OK);
            MockEmisClientAppointmentSlotsMetadataGetMethod(metadataResponse);

            MockEmisClientPracticeSettingsGetMethod(new EmisClient.EmisApiObjectResponse<PracticeSettingsGetResponse>(HttpStatusCode.OK));

            var unsuccessfulSlotResponse = _fixture
                .Build<EmisClient.EmisApiObjectResponse<AppointmentSlotsGetResponse>>()
                .With(x => x.StatusCode, HttpStatusCode.InternalServerError)
                .With(x => x.Body, () => null)
                .Create();

            MockEmisClientAppointmentSlotGetMethod(unsuccessfulSlotResponse);

            MockEmisDemographicsGetMethod(
                new EmisClient.EmisApiObjectResponse<DemographicsGetResponse>(HttpStatusCode.OK));

            // Act
            var result = await _systemUnderTest.GetSlots(_gpLinkedAccountModel, _dateRange);

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentSlotsResult.BadGateway>();
        }

        [TestMethod]
        public async Task GetSlots_EmisClientAppointmentSlotsMetadataGetThrowsHttpRequestException_ReturnsBadGateway()
        {
            // Arrange
            MockEmisClientAppointmentSlotGetMethod(new EmisClient.EmisApiObjectResponse<AppointmentSlotsGetResponse>(HttpStatusCode.OK));

            _mockEmisClient
                .Setup(x => x.AppointmentSlotsMetadataGet(It.IsAny<EmisRequestParameters>(), It.IsAny<SlotsMetadataGetQueryParameters>()))
                .ThrowsAsync(new HttpRequestException())
                .Verifiable();

            // Act
            var result = await _systemUnderTest.GetSlots(_gpLinkedAccountModel, _dateRange);

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentSlotsResult.BadGateway>();
        }

        [TestMethod]
        public async Task GetSlots_EmisClientAppointmentSlotsMetadataGetUnsuccessful_ReturnsBadGateway()
        {
            // Arrange
            var unsuccessfulMetadataResponse = _fixture
                .Build<EmisClient.EmisApiObjectResponse<AppointmentSlotsMetadataGetResponse>>()
                .With(x => x.StatusCode, HttpStatusCode.InternalServerError)
                .With(x => x.Body, () => null)
                .Create();

            MockEmisClientAppointmentSlotsMetadataGetMethod(unsuccessfulMetadataResponse);

            MockEmisClientPracticeSettingsGetMethod(new EmisClient.EmisApiObjectResponse<PracticeSettingsGetResponse>(HttpStatusCode.OK));

            var slotsResponse = new EmisClient.EmisApiObjectResponse<AppointmentSlotsGetResponse>(HttpStatusCode.OK);
            MockEmisClientAppointmentSlotGetMethod(slotsResponse);

            MockEmisDemographicsGetMethod(
                new EmisClient.EmisApiObjectResponse<DemographicsGetResponse>(HttpStatusCode.OK));

            // Act
            var result = await _systemUnderTest.GetSlots(_gpLinkedAccountModel, _dateRange);

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentSlotsResult.BadGateway>();
        }

        [TestMethod]
        public async Task GetSlots_EmisClientPracticeSettingsGetThrowsHttpRequestException_ReturnsSuccessfullyAnyway()
        {
            // Arrange
            _mockEmisClient
                .Setup(x => x.PracticeSettingsGet(It.IsAny<EmisRequestParameters>(), It.IsAny<string>()))
                .ThrowsAsync(new HttpRequestException())
                .Verifiable();

            MockEmisClientAppointmentSlotsMetadataGetMethod(new EmisClient.EmisApiObjectResponse<AppointmentSlotsMetadataGetResponse>(HttpStatusCode.OK));
            MockEmisClientAppointmentSlotGetMethod(new EmisClient.EmisApiObjectResponse<AppointmentSlotsGetResponse>(HttpStatusCode.OK));
            MockEmisDemographicsGetMethod(
                new EmisClient.EmisApiObjectResponse<DemographicsGetResponse>(HttpStatusCode.OK));

            // Act
            var result = await _systemUnderTest.GetSlots(_gpLinkedAccountModel, _dateRange);

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentSlotsResult.Success>();
        }

        [TestMethod]
        public async Task GetSlots_EmisClientPracticeSettingsGetError_ReturnsSuccessfullyAnyway()
        {
            // Arrange
            var unsuccessfulPracticeSettingsResponse = _fixture
                .Build<EmisClient.EmisApiObjectResponse<PracticeSettingsGetResponse>>()
                .With(x => x.StatusCode, HttpStatusCode.InternalServerError)
                .With(x => x.Body, () => null)
                .Create();

            MockEmisClientAppointmentSlotsMetadataGetMethod(new EmisClient.EmisApiObjectResponse<AppointmentSlotsMetadataGetResponse>(HttpStatusCode.OK));
            MockEmisClientAppointmentSlotGetMethod(new EmisClient.EmisApiObjectResponse<AppointmentSlotsGetResponse>(HttpStatusCode.OK));
            MockEmisClientPracticeSettingsGetMethod(unsuccessfulPracticeSettingsResponse);
            MockEmisDemographicsGetMethod(
                new EmisClient.EmisApiObjectResponse<DemographicsGetResponse>(HttpStatusCode.OK));

            // Act
            var result = await _systemUnderTest.GetSlots(_gpLinkedAccountModel, _dateRange);

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentSlotsResult.Success>();
        }

        [TestMethod]
        public async Task GetSlots_EmisClientPracticeSettingsGetUnsuccessful_ReturnsSuccessfullyAnyway()
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
            MockEmisDemographicsGetMethod(
                new EmisClient.EmisApiObjectResponse<DemographicsGetResponse>(HttpStatusCode.OK));

            // Act
            var result = await _systemUnderTest.GetSlots(_gpLinkedAccountModel, _dateRange);

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentSlotsResult.Success>();
        }

        [TestMethod]
        public async Task GetSlots_EmisClientAppointmentSlotsGetNotAvailable_ReturnsForbidden()
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

            MockEmisDemographicsGetMethod(
                new EmisClient.EmisApiObjectResponse<DemographicsGetResponse>(HttpStatusCode.OK));

            // Act
            var result = await _systemUnderTest.GetSlots(_gpLinkedAccountModel, _dateRange);

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentSlotsResult.Forbidden>();
        }

        [TestMethod]
        public async Task GetSlots_EmisClientAppointmentSlotsGetNotAvailableException_ReturnsForbidden()
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

            MockEmisDemographicsGetMethod(
                new EmisClient.EmisApiObjectResponse<DemographicsGetResponse>(HttpStatusCode.OK));

            // Act
            var result = await _systemUnderTest.GetSlots(_gpLinkedAccountModel, _dateRange);

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentSlotsResult.Forbidden>();
        }

        [TestMethod]
        public async Task GetSlots_EmisClientAppointmentSlotsMetadataGetNotAvailable_ReturnsForbidden()
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

            MockEmisDemographicsGetMethod(
                new EmisClient.EmisApiObjectResponse<DemographicsGetResponse>(HttpStatusCode.OK));

            // Act
            var result = await _systemUnderTest.GetSlots(_gpLinkedAccountModel, _dateRange);

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentSlotsResult.Forbidden>();
        }

        [TestMethod]
        public async Task GetSlots_EmisClientAppointmentSlotsMetadataGetNotAvailableException_ReturnsForbidden()
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

            MockEmisDemographicsGetMethod(
                new EmisClient.EmisApiObjectResponse<DemographicsGetResponse>(HttpStatusCode.OK));

            // Act
            var result = await _systemUnderTest.GetSlots(_gpLinkedAccountModel, _dateRange);

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentSlotsResult.Forbidden>();
        }

        [TestMethod]
        public async Task GetSlots_SomethingGoesWrongDuringMappingResponse_ReturnsInternalServerError()
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

            MockEmisDemographicsGetMethod(
                new EmisClient.EmisApiObjectResponse<DemographicsGetResponse>(HttpStatusCode.OK));

            _mockResponseMapper.Setup(x => x.Map(slotResponse.Body, metadataResponse.Body, It.IsAny<PracticeSettingsGetResponse>(), It.IsAny<DemographicsGetResponse>(), _emisUserSession))
                .Throws<Exception>();

            // Act
            var result = await _systemUnderTest.GetSlots(_gpLinkedAccountModel, _dateRange);

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentSlotsResult.InternalServerError>();
        }

        [TestMethod]
        public async Task GetSlots_HappyPath_ReturnsAppointmentSlots()
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

            MockEmisDemographicsGetMethod(
                new EmisClient.EmisApiObjectResponse<DemographicsGetResponse>(HttpStatusCode.OK));

            var expectedResponse = _fixture.Create<AppointmentSlotsResponse>();

            _mockResponseMapper.Setup(x => x.Map(slotResponse.Body, metadataResponse.Body, It.IsAny<PracticeSettingsGetResponse>(), It.IsAny<DemographicsGetResponse>(), _emisUserSession))
                .Returns(expectedResponse);

            // Act
            var result = await _systemUnderTest.GetSlots(_gpLinkedAccountModel, _dateRange);

            // Assert
            _mockEmisClient.Verify();
            result.Should().BeAssignableTo<AppointmentSlotsResult.Success>();
        }

        private void MockEmisClientPracticeSettingsGetMethod(
            EmisClient.EmisApiObjectResponse<PracticeSettingsGetResponse> response)
        {
            _mockEmisClient.Setup(x => x.PracticeSettingsGet(It.Is<EmisRequestParameters>(p =>
                    p.EndUserSessionId.Equals(_emisUserSession.EndUserSessionId, StringComparison.Ordinal)
                    && p.SessionId.Equals(_emisUserSession.SessionId, StringComparison.Ordinal)), _emisUserSession.OdsCode))
                .ReturnsAsync(response)
                .Verifiable();
        }

        private void MockEmisClientAppointmentSlotGetMethod(
            EmisClient.EmisApiObjectResponse<AppointmentSlotsGetResponse> response)
        {
            _mockEmisClient.Setup(x => x.AppointmentSlotsGet(
                    It.Is<EmisRequestParameters>(p =>
                        p.EndUserSessionId.Equals(_emisUserSession.EndUserSessionId, StringComparison.Ordinal)
                        && p.SessionId.Equals(_emisUserSession.SessionId, StringComparison.Ordinal)
                        && p.UserPatientLinkToken.Equals(_emisUserSession.UserPatientLinkToken, StringComparison.Ordinal)),
                    It.Is<SlotsGetQueryParameters>(p =>
                        p.FromDateTime == _slotsGetQueryParameters.FromDateTime
                        && p.ToDateTime == _slotsGetQueryParameters.ToDateTime
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
                        It.Is<EmisRequestParameters>(p =>
                            p.EndUserSessionId.Equals(_emisUserSession.EndUserSessionId, StringComparison.Ordinal)
                            && p.SessionId.Equals(_emisUserSession.SessionId, StringComparison.Ordinal)
                            && p.UserPatientLinkToken.Equals(_emisUserSession.UserPatientLinkToken, StringComparison.Ordinal)),
                        It.Is<SlotsMetadataGetQueryParameters>(p =>
                            p.SessionStartDate == _slotsMetadataGetQueryParameters.SessionStartDate
                            && p.SessionEndDate == _slotsMetadataGetQueryParameters.SessionEndDate
                        )
                    )
                )
                .ReturnsAsync(response)
                .Verifiable();
        }

        private void MockEmisDemographicsGetMethod(EmisClient.EmisApiObjectResponse<DemographicsGetResponse> response)
        {
            _mockEmisClient.Setup(x => x.DemographicsGet(
                    It.Is<EmisRequestParameters>(
                    e => e.UserPatientLinkToken.Equals(_emisUserSession.UserPatientLinkToken, StringComparison.Ordinal) &&
                         e.SessionId.Equals(_emisUserSession.SessionId, StringComparison.Ordinal) &&
                         e.EndUserSessionId.Equals(_emisUserSession.EndUserSessionId, StringComparison.Ordinal))))
                .ReturnsAsync(response).Verifiable();
        }
    }
}
