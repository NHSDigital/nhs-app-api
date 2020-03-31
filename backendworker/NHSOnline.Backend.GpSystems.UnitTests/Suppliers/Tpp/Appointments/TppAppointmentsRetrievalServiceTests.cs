using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Appointments;
using NHSOnline.Backend.Support.Temporal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client;
using NHSOnline.Backend.Support;
using UnitTestHelper;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp.Appointments
{
    [TestClass]
    public class TppAppointmentsRetrievalServiceTests
    {
        private IFixture _fixture;
        private TppAppointmentsService _systemUnderTest;
        private TppUserSession _tppUserSession;

        private Mock<ITppClientRequest<(TppRequestParameters, AppointmentViewType), ViewAppointmentsReply>> _mockViewAppointments;

        private Mock<IAppointmentsReplyMapper> _mockResponseMapper;
        private AppointmentsResponse _mappedResponse;
        private ViewAppointmentsReply _tppViewPastAppointmentsReply;
        private ViewAppointmentsReply _tppViewUpcomingAppointmentsReply;
        private TppApiObjectResponse<ViewAppointmentsReply> _tppErrorResponse;
        private Mock<ICurrentDateTimeProvider> _mockCurrentDateTimeProvider;
        private Guid _patientId;
        private GpLinkedAccountModel _gpLinkedAccountModel;

        [TestInitialize]
        public void TestInitialize()
        {
            _patientId = Guid.NewGuid();
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _mockCurrentDateTimeProvider = _fixture.Freeze<Mock<ICurrentDateTimeProvider>>();
            _mockCurrentDateTimeProvider.SetupGet(x => x.UtcNow)
                .Returns(DateTime.UtcNow);

            _tppUserSession = _fixture.Create<TppUserSession>();
            _tppUserSession.Id = _patientId;

            _gpLinkedAccountModel = new GpLinkedAccountModel(_tppUserSession, _patientId);

            _mockViewAppointments = _fixture.Freeze<Mock<ITppClientRequest
                <(TppRequestParameters, AppointmentViewType), ViewAppointmentsReply>>>();

            _tppViewPastAppointmentsReply = new ViewAppointmentsReply()
            {
                OnlineUserId = _tppUserSession.OnlineUserId,
                PatientId = _tppUserSession.PatientId,
                UuId = "uuid",
                Appointments = new List<Backend.GpSystems.Suppliers.Tpp.Models.Appointments.Appointment>()
                {
                    new Backend.GpSystems.Suppliers.Tpp.Models.Appointments.Appointment
                    {
                        StartDate = "2017-12-12T09:30:00",
                        EndDate = "2017-12-12T09:40:00",
                        Address = "Address",
                        ApptId = "apptId",
                        Details = "Details",
                        SiteName = "TheSite"
                    }
                }
            };
            var response1 = new TppApiObjectResponse<ViewAppointmentsReply>(HttpStatusCode.OK)
            {
                Body = _tppViewPastAppointmentsReply
            };
            MockTppClientAppointmentsGetMethod(response1, AppointmentViewType.Past);

            _tppViewUpcomingAppointmentsReply = new ViewAppointmentsReply()
            {
                OnlineUserId = _tppUserSession.OnlineUserId,
                PatientId = _tppUserSession.PatientId,
                UuId = "uuid",
                Appointments = new List<Backend.GpSystems.Suppliers.Tpp.Models.Appointments.Appointment>()
                {
                    new Backend.GpSystems.Suppliers.Tpp.Models.Appointments.Appointment
                    {
                        StartDate = "2018-12-12T09:30:00",
                        EndDate = "2018-12-12T09:40:00",
                        Address = "Address",
                        ApptId = "apptId",
                        Details = "Details",
                        SiteName = "TheSite"
                    }
                }
            };
            var response2 = new TppApiObjectResponse<ViewAppointmentsReply>(HttpStatusCode.OK)
            {
                Body = _tppViewUpcomingAppointmentsReply
            };
            MockTppClientAppointmentsGetMethod(response2, AppointmentViewType.Future);

            _tppErrorResponse =
                new TppApiObjectResponse<ViewAppointmentsReply>(HttpStatusCode.Ambiguous)
                {
                    ErrorResponse = _fixture.Create<Error>()
                };

            _mappedResponse = new AppointmentsResponse
            {
                PastAppointments = new[]
                {
                    new PastAppointment
                    {
                        StartTime = new DateTimeOffset(DateTime.Parse("2017-12-12T09:30:00",
                            CultureInfo.InvariantCulture)),
                        EndTime = new DateTimeOffset(
                            DateTime.Parse("2017-12-12T09:40:00", CultureInfo.InvariantCulture)),
                        Id = "apptId",
                        Location = "TheSite",
                        Type = "Details"
                    }
                },
                UpcomingAppointments = new[]
                {
                    new UpcomingAppointment
                    {
                        StartTime = new DateTimeOffset(DateTime.Parse("2018-12-12T09:30:00",
                            CultureInfo.InvariantCulture)),
                        EndTime = new DateTimeOffset(
                            DateTime.Parse("2018-12-12T09:40:00", CultureInfo.InvariantCulture)),
                        Id = "apptId",
                        Location = "TheSite",
                        Type = "Details"
                    }
                },
                PastAppointmentsEnabled = true
            };

            _mockResponseMapper = _fixture.Freeze<Mock<IAppointmentsReplyMapper>>();
            _mockResponseMapper.Setup(x => x.Map(_tppViewPastAppointmentsReply, _tppViewUpcomingAppointmentsReply))
                .Returns(_mappedResponse);

            IConfigurationBuilder configBuilder = new ConfigurationBuilder();
            configBuilder.AddInMemoryCollection(new[]
            {
                new KeyValuePair<string, string>("TIMEZONE",
                    TimeZoneResolver.GetTimeZoneNameForCurrentOperatingSystemPlatform())
            });
            var timeZoneInfoProvider =
                new TimeZoneInfoProvider(new Mock<ILogger<TimeZoneInfoProvider>>().Object, configBuilder.Build());

            var builder = new TppAppointmentsResultBuilder(
                _fixture.Create<ILogger<TppAppointmentsService>>(),
                _mockResponseMapper.Object
            );
            var dateTimeOffsetProvider =
                new DateTimeOffsetProvider(timeZoneInfoProvider, _mockCurrentDateTimeProvider.Object);

            _systemUnderTest = new TppAppointmentsService(
                new TppAppointmentsRetrievalService(Mock.Of<ILogger<TppAppointmentsRetrievalService>>(),
                    _mockViewAppointments.Object, builder),
                new TppAppointmentsBookingService(Mock.Of<ILogger<TppAppointmentsBookingService>>(),
                    _fixture.Freeze<Mock<ITppClientRequest<(TppRequestParameters, BookingDates, AppointmentBookRequest), BookAppointmentReply>>>()
                        .Object,
                    dateTimeOffsetProvider),
                new TppAppointmentsCancellationService(Mock.Of<ILogger<TppAppointmentsCancellationService>>(),
                    _fixture.Freeze<Mock<ITppClientRequest<(TppRequestParameters, AppointmentCancelRequest), CancelAppointmentReply>>>()
                        .Object));
        }

        [TestMethod]
        public async Task GetAppointments_HappyPath_ReturnsSuccessResponse()
        {
            // Act
            var result = await _systemUnderTest.GetAppointments(_gpLinkedAccountModel);

            // Assert
            var response = result.Should().BeAssignableTo<AppointmentsResult.Success>().Subject.Response;
            response.Should().BeEquivalentTo(_mappedResponse);
            response.PastAppointmentsEnabled.Should().BeTrue();
            _mockViewAppointments.VerifyAll();
            _mockResponseMapper.VerifyAll();
        }

        [TestMethod]
        public async Task GetAppointments_TppClientThrows_ReturnsBadGateway()
        {
            // Arrange
            _mockViewAppointments.Setup(x => x.Post(
                    It.Is<(TppRequestParameters, AppointmentViewType)>(tuple =>
                    string.Equals(tuple.Item1.PatientId, _tppUserSession.PatientId, StringComparison.Ordinal)
                        && string.Equals(tuple.Item1.OnlineUserId, _tppUserSession.OnlineUserId, StringComparison.Ordinal))
                    ))
                .Throws<HttpRequestException>()
                .Verifiable();

            // Act
            var result = await _systemUnderTest.GetAppointments(_gpLinkedAccountModel);

            // Assert
            result.Should().BeAssignableTo<AppointmentsResult.BadGateway>();
        }

        [TestMethod]
        public async Task GetAppointments_MapperThrows_ReturnsInternalServerError()
        {
            // Arrange
            _mockResponseMapper.Setup(x => x.Map(_tppViewPastAppointmentsReply, _tppViewUpcomingAppointmentsReply))
                .Throws<Exception>();

            // Act
            var result = await _systemUnderTest.GetAppointments(_gpLinkedAccountModel);

            // Assert
            result.Should().BeAssignableTo<AppointmentsResult.InternalServerError>();
        }

        [TestMethod]
        public async Task GetAppointments_TppClientReturnsForbiddenCode_ReturnsForbidden()
        {
            // Arrange
            var tppResponse = new TppApiObjectResponse<ViewAppointmentsReply>(HttpStatusCode.OK)
            {
                ErrorResponse = new Error { ErrorCode = TppApiErrorCodes.NoAccess }
            };
            MockTppClientAppointmentsGetMethod(tppResponse, AppointmentViewType.Past);
            MockTppClientAppointmentsGetMethod(tppResponse, AppointmentViewType.Future);

            // Act
            var result = await _systemUnderTest.GetAppointments(_gpLinkedAccountModel);

            // Assert
            result.Should().BeAssignableTo<AppointmentsResult.Forbidden>();
        }

        [TestMethod]
        public async Task GetAppointments_TppClientReturnsInternalServerErrorWithForbiddenMessage_ReturnsForbidden()
        {
            // Arrange
            var errorResponse = _fixture.Create<Error>();

            errorResponse.ErrorCode = TppApiErrorCodes.NoAccess;

            var tppResponse = new TppApiObjectResponse<ViewAppointmentsReply>(HttpStatusCode.InternalServerError)
            {
                ErrorResponse = errorResponse
            };
            MockTppClientAppointmentsGetMethod(tppResponse, AppointmentViewType.Past);
            MockTppClientAppointmentsGetMethod(tppResponse, AppointmentViewType.Future);

            // Act
            var result = await _systemUnderTest.GetAppointments(_gpLinkedAccountModel);

            // Assert
            result.Should().BeAssignableTo<AppointmentsResult.Forbidden>();

        }

        [TestMethod]
        public async Task GetAppointments_TppClientReturnsUnknownError_ReturnsBadGateway()
        {
            // Arrange
            MockTppClientAppointmentsGetMethod(_tppErrorResponse, AppointmentViewType.Past);
            MockTppClientAppointmentsGetMethod(_tppErrorResponse, AppointmentViewType.Future);

            // Act
            var result = await _systemUnderTest.GetAppointments(_gpLinkedAccountModel);

            // Assert
            result.Should().BeAssignableTo<AppointmentsResult.BadGateway>();
        }

        [TestMethod]
        public async Task GetAppointments_TppClientReturnsUnknownErrorOnPastAppointmentsRequest_ReturnsBadGateway()
        {
            // Arrange
            MockTppClientAppointmentsGetMethod(_tppErrorResponse, AppointmentViewType.Future);

            // Act
            var result = await _systemUnderTest.GetAppointments(_gpLinkedAccountModel);

            // Assert
            result.Should().BeAssignableTo<AppointmentsResult.BadGateway>();
        }

        [TestMethod]
        public async Task GetAppointments_TppClientReturnsUnknownErrorOnUpcomingAppointmentsRequest_ReturnsBadGateway()
        {
            // Arrange
            MockTppClientAppointmentsGetMethod(_tppErrorResponse, AppointmentViewType.Future);

            // Act
            var result = await _systemUnderTest.GetAppointments(_gpLinkedAccountModel);

            // Assert
            result.Should().BeAssignableTo<AppointmentsResult.BadGateway>();
        }

        private void MockTppClientAppointmentsGetMethod(
            TppApiObjectResponse<ViewAppointmentsReply> response, AppointmentViewType viewType)
        {
            _mockViewAppointments.Setup(x => x.Post(It.Is<(TppRequestParameters, AppointmentViewType)>(tuple =>
                    string.Equals(tuple.Item1.PatientId, _tppUserSession.PatientId, StringComparison.Ordinal)
                    && string.Equals(tuple.Item1.OnlineUserId, _tppUserSession.OnlineUserId, StringComparison.Ordinal)
                    && tuple.Item2.Equals(viewType))))
                .ReturnsAsync(response);
        }
    }
}
