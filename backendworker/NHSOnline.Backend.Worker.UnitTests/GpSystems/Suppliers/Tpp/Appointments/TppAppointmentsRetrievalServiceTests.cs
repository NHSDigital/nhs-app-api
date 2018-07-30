using AutoFixture;
using AutoFixture.AutoMoq;
using Castle.Core.Logging;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.Areas.Appointments.Models;
using NHSOnline.Backend.Worker.GpSystems.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models.Appointments;
using NHSOnline.Backend.Worker.Support.Date;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Tpp.Appointments
{
    [TestClass]
    public class TppAppointmentsRetrievalServiceTests
    {
        private IFixture _fixture;
        private TppAppointmentsService _systemUnderTest;
        private TppUserSession _userSession;
        private Mock<ITppClient> _mockTppClient;
        private Mock<IAppointmentsReplyMapper> _mockResponseMapper;
        private AppointmentsResponse _mappedResponse;
        private ViewAppointmentsReply _tppViewAppointmentsReply;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _userSession = _fixture.Create<TppUserSession>();

            _mockTppClient = _fixture.Freeze<Mock<ITppClient>>();
            _tppViewAppointmentsReply = new ViewAppointmentsReply()
            {
                OnlineUserId = _userSession.OnlineUserId,
                PatientId = _userSession.PatientId,
                UuId = "uuid",
                Appointments = new List<Worker.GpSystems.Suppliers.Tpp.Models.Appointments.Appointment>()
                 {
                     new Worker.GpSystems.Suppliers.Tpp.Models.Appointments.Appointment {
                         StartDate = "2018-12-12T09:30:00",
                         EndDate = "2018-12-12T09:40:00",
                         Address = "Address",
                         ApptId = "apptId",
                         Details = "Details",
                         SiteName = "TheSite"
                     }
                 }
            };

            var response = new TppClient.TppApiObjectResponse<ViewAppointmentsReply>(HttpStatusCode.OK)
            {
                Body = _tppViewAppointmentsReply
            };
            MockTppClientAppointmentsGetMethod(response);

            _mappedResponse = new AppointmentsResponse()
            {
                Appointments = new[]
                {
                    new Worker.Areas.Appointments.Models.Appointment
                    {
                        StartTime = new DateTimeOffset(DateTime.Parse("2018-12-12T09:30:00")),
                        EndTime = new DateTimeOffset(DateTime.Parse("2018-12-12T09:40:00")),
                        Id = "apptId",
                        Location = "TheSite",
                        Type = "Details"
                    }
                }
            };

            _mockResponseMapper = _fixture.Freeze<Mock<IAppointmentsReplyMapper>>();
            _mockResponseMapper.Setup(x => x.Map(_tppViewAppointmentsReply))
                .Returns(_mappedResponse);

            IConfigurationBuilder configBuilder = new ConfigurationBuilder();
            configBuilder.AddInMemoryCollection(new[] { new KeyValuePair<string, string>("TIMEZONE", "GMT Standard Time") });
            var timeZoneInfoProvider = new TimeZoneInfoProvider(configBuilder.Build());

            var builder = new TppAppointmentsResultBuilder(
                _fixture.Create<ILogger<TppAppointmentsService>>(),
                _mockResponseMapper.Object
            );
            var dateTimeOffsetProvider = new DateTimeOffsetProvider(timeZoneInfoProvider);

            _systemUnderTest = new TppAppointmentsService(
                new TppAppointmentsRetrievalService(Mock.Of<ILogger<TppAppointmentsRetrievalService>>(),
                    _mockTppClient.Object, builder),
                new TppAppointmentsBookingService(Mock.Of<ILogger<TppAppointmentsBookingService>>(),
                    _mockTppClient.Object, dateTimeOffsetProvider),
                new TppAppointmentsCancellationService(Mock.Of<ILogger<TppAppointmentsCancellationService>>(),
                    _mockTppClient.Object));

        }

        [TestMethod]
        public async Task GetAppointments_HappyPath_ReturnsSuccessfullyRetrievedResponse()
        {
            // Arrange

            // Act
            var result = await _systemUnderTest.GetAppointments(_userSession, false, null);

            // Assert
            var response = result.Should().BeAssignableTo<AppointmentsResult.SuccessfullyRetrieved>().Subject.Response;
            response.Should().BeEquivalentTo(_mappedResponse);

            _mockTppClient.VerifyAll();
            //_mockResponseMapper.VerifyAll();
        }

        [TestMethod]
        public async Task GetAppointments_TppClientThrows_ReturnsSupplierSystemUnavailable()
        {
            // Arrange
            _mockTppClient.Setup(x => x.ViewAppointmentsPost(
                    It.Is<ViewAppointments>(p =>
                        p.PatientId == _userSession.PatientId 
                        && p.OnlineUserId == _userSession.OnlineUserId),
                    It.IsAny<string>()))
                .Throws<HttpRequestException>()
                .Verifiable();

            // Act
            var result = await _systemUnderTest.GetAppointments(_userSession, false, null);

            // Assert
            result.Should().BeAssignableTo<AppointmentsResult.SupplierSystemUnavailable>();
        }

        [TestMethod]
        public async Task GetAppointments_MapperThrows_ReturnsInternalServerError()
        {
            // Arrange
            _mockResponseMapper.Setup(x => x.Map(_tppViewAppointmentsReply))
                .Throws<Exception>();

            // Act
            var result = await _systemUnderTest.GetAppointments(_userSession, false, null);

            // Assert
            result.Should().BeAssignableTo<AppointmentsResult.InternalServerError>();
        }

        [TestMethod]
        public async Task GetAppointments_TppClientReturnsForbiddenCode_ReturnsCannotViewAppointments()
        {
            // Arrange
            var tppResponse = new TppClient.TppApiObjectResponse<ViewAppointmentsReply>(HttpStatusCode.OK) { ErrorResponse = new Error { ErrorCode = TppApiErrorCodes.NoAccess } };
            MockTppClientAppointmentsGetMethod(tppResponse);

            // Act
            var result = await _systemUnderTest.GetAppointments(_userSession, false, null);

            // Assert
            result.Should().BeAssignableTo<AppointmentsResult.CannotViewAppointments>();
        }

        [TestMethod]
        public async Task GetAppointments_TppClientReturnsInternalServerErrorWithForbiddenMessage_ReturnsCannotViewAppointments()
        {
            // Arrange
            var errorResponse = _fixture.Create<Error>();

            errorResponse.ErrorCode = TppApiErrorCodes.NoAccess;

            var tppResponse = new TppClient.TppApiObjectResponse<ViewAppointmentsReply>(HttpStatusCode.InternalServerError)
                {
                    ErrorResponse = errorResponse
                };
            MockTppClientAppointmentsGetMethod(tppResponse);

            // Act
            var result = await _systemUnderTest.GetAppointments(_userSession, false, null);

            // Assert
            result.Should().BeAssignableTo<AppointmentsResult.CannotViewAppointments>();

        }

        [TestMethod]
        public async Task GetAppointments_TppClientReturnsUnknownError_ReturnsSupplierSystemUnavailable()
        {
            // Arrange
            var errorResponse = _fixture.Create<Error>();

            var tppResponse =
                new TppClient.TppApiObjectResponse<ViewAppointmentsReply>(HttpStatusCode.Ambiguous)
                {
                    ErrorResponse = errorResponse
                };

            MockTppClientAppointmentsGetMethod(tppResponse);

            // Act
            var result = await _systemUnderTest.GetAppointments(_userSession, false, null);

            // Assert
            result.Should().BeAssignableTo<AppointmentsResult.SupplierSystemUnavailable>();
        }

        private void MockTppClientAppointmentsGetMethod(
            TppClient.TppApiObjectResponse<ViewAppointmentsReply> response)
        {
            _mockTppClient.Setup(x => x.ViewAppointmentsPost(
                     It.Is<ViewAppointments>(p =>
                        p.PatientId == _userSession.PatientId
                        && p.OnlineUserId == _userSession.OnlineUserId),
                    It.IsAny<string>()))
                .ReturnsAsync(response);
        }
    }
}
