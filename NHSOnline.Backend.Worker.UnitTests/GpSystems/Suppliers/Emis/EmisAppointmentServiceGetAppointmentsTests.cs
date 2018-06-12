using System;
using System.Collections.Generic;
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
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Emis
{
    [TestClass]
    public class EmisAppointmentServiceGetAppointmentsTests
    {
        private IFixture _fixture;
        private Mock<IEmisClient> _mockEmisClient;
        private EmisUserSession _userSession;
        private EmisAppointmentsService _systemUnderTest;
        private AppointmentsGetResponse _emisClientGetResponse;
        private Mock<IAppointmentsResponseMapper> _mockResponseMapper;
        private AppointmentsResponse _mappedResponse;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _userSession = _fixture.Create<EmisUserSession>();

            _mockEmisClient = _fixture.Freeze<Mock<IEmisClient>>();
            _emisClientGetResponse = _fixture.Create<AppointmentsGetResponse>();
            var response = new EmisClient.EmisApiObjectResponse<AppointmentsGetResponse>(HttpStatusCode.OK)
            {
                Body = _emisClientGetResponse
            };
            MockEmisClientAppointmentsGetMethod(response);
                
            _mappedResponse = _fixture.Create<AppointmentsResponse>();

            _mockResponseMapper = _fixture.Freeze<Mock<IAppointmentsResponseMapper>>();
            _mockResponseMapper.Setup(x => x.Map(_emisClientGetResponse))
                .Returns(_mappedResponse);
            
            _systemUnderTest = _fixture.Create<EmisAppointmentsService>();
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

            _mockEmisClient.VerifyAll();
            _mockResponseMapper.VerifyAll();
        }

        [TestMethod]
        public async Task GetAppointments_EmisClientThrows_ReturnsSupplierSystemUnavailable()
        {
            // Arrange
            _mockEmisClient.Setup(x => x.AppointmentsGet(
                    It.Is<EmisHeaderParameters>(p =>
                        p.EndUserSessionId == _userSession.EndUserSessionId && p.SessionId == _userSession.SessionId),
                    _userSession.UserPatientLinkToken,
                    It.IsAny<bool>(),
                    It.IsAny<DateTimeOffset?>()))
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
            _mockResponseMapper.Setup(x => x.Map(_emisClientGetResponse))
                .Throws<Exception>();

            // Act
            var result = await _systemUnderTest.GetAppointments(_userSession, false, null);

            // Assert
            result.Should().BeAssignableTo<AppointmentsResult.InternalServerError>();
        }

        [TestMethod]
        public async Task GetAppointments_EmisClientReturnsForbiddenCode_ReturnsSuccessfulEmptyResponse()
        {
            // Arrange
            var emisResponse = new EmisClient.EmisApiObjectResponse<AppointmentsGetResponse>(HttpStatusCode.Forbidden);
            MockEmisClientAppointmentsGetMethod(emisResponse);

            // Act
            var result = await _systemUnderTest.GetAppointments(_userSession, false, null);

            // Assert
            var apiResponse = result.Should().BeAssignableTo<AppointmentsResult.SuccessfullyRetrieved>().Subject.Response;
            apiResponse.Should().BeEquivalentTo(new AppointmentsResponse());
        }

        [TestMethod]
        public async Task GetAppointments_EmisClientReturnsInternalServerErrorWithForbiddenMessage_ReturnsSuccessfulEmptyResponse()
        {
            // Arrange
            var errorResponse = _fixture.Create<ErrorResponse>();
            errorResponse.Exceptions.First().Message =
                "Extra info: " + EmisApiErrorMessages.EmisService_NotEnabledForUser;
            var emisResponse =
                new EmisClient.EmisApiObjectResponse<AppointmentsGetResponse>(HttpStatusCode.InternalServerError)
                {
                    ErrorResponse = errorResponse
                };
            MockEmisClientAppointmentsGetMethod(emisResponse);

            // Act
            var result = await _systemUnderTest.GetAppointments(_userSession, false, null);

            // Assert
            var apiResponse = result.Should().BeAssignableTo<AppointmentsResult.SuccessfullyRetrieved>().Subject.Response;
            apiResponse.Should().BeEquivalentTo(new AppointmentsResponse());
        }

        private void MockEmisClientAppointmentsGetMethod(
            EmisClient.EmisApiObjectResponse<AppointmentsGetResponse> response)
        {
            _mockEmisClient.Setup(x => x.AppointmentsGet(
                    It.Is<EmisHeaderParameters>(p =>
                        p.EndUserSessionId == _userSession.EndUserSessionId && p.SessionId == _userSession.SessionId),
                    _userSession.UserPatientLinkToken,
                    It.IsAny<bool>(),
                    It.IsAny<DateTimeOffset?>()))
                .ReturnsAsync(response);
        }
    }
}
