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
using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Emis.Appointments
{
    [TestClass]
    public class EmisAppointmentsRetrievalServiceTests
    {
        private IFixture _fixture;
        private Mock<IEmisClient> _mockEmisClient;
        private EmisUserSession _emisUserSession;
        private EmisAppointmentsService _systemUnderTest;
        private AppointmentsGetResponse _emisClientGetResponse;
        private Mock<IAppointmentsResponseMapper> _mockResponseMapper;
        private AppointmentsResponse _mappedResponse;
        private Guid _patientId;
        private GpLinkedAccountModel _gpLinkedAccountModel;

        [TestInitialize]
        public void TestInitialize()
        {
            _patientId = Guid.NewGuid();
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _emisUserSession = _fixture.Build<EmisUserSession>()
                .With(x => x.Id, _patientId)
                .Create();

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
            
            _gpLinkedAccountModel = new GpLinkedAccountModel(_emisUserSession, _patientId);
            
            _systemUnderTest = _fixture.Create<EmisAppointmentsService>();
        }

        [TestMethod]
        public async Task GetAppointments_HappyPath_ReturnsSuccessResponse()
        {
            // Act
            var result = await _systemUnderTest.GetAppointments(_gpLinkedAccountModel);

            // Assert
            var response = result.Should().BeAssignableTo<AppointmentsResult.Success>().Subject.Response;
            response.Should().BeEquivalentTo(_mappedResponse);

            _mockEmisClient.VerifyAll();
            _mockResponseMapper.VerifyAll();
        }

        [TestMethod]
        public async Task GetAppointments_EmisClientThrows_ReturnsBadGateway()
        {
            // Arrange
            _mockEmisClient.Setup(x => x.AppointmentsGet(
                    It.Is<EmisRequestParameters>(
                        p => p.EndUserSessionId.Equals(_emisUserSession.EndUserSessionId, StringComparison.Ordinal)
                             && p.SessionId.Equals(_emisUserSession.SessionId, StringComparison.Ordinal)
                             && p.UserPatientLinkToken.Equals(_emisUserSession.UserPatientLinkToken, StringComparison.Ordinal))))
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
            _mockResponseMapper.Setup(x => x.Map(_emisClientGetResponse))
                .Throws<Exception>();

            // Act
            var result = await _systemUnderTest.GetAppointments(_gpLinkedAccountModel);

            // Assert
            result.Should().BeAssignableTo<AppointmentsResult.InternalServerError>();
        }

        [TestMethod]
        public async Task GetAppointments_EmisClientReturnsForbiddenCode_ReturnsForbidden()
        {
            // Arrange
            var emisResponse = new EmisClient.EmisApiObjectResponse<AppointmentsGetResponse>(HttpStatusCode.Forbidden);
            MockEmisClientAppointmentsGetMethod(emisResponse);

            // Act
            var result = await _systemUnderTest.GetAppointments(_gpLinkedAccountModel);

            // Assert
            result.Should().BeAssignableTo<AppointmentsResult.Forbidden>();
        }

        [TestMethod]
        public async Task GetAppointments_EmisClientReturnsForbidden_ReturnsForbidden()
        {
            // Arrange
            var errorResponse = _fixture.Create<StandardErrorResponse>();
            var emisResponse =
                new EmisClient.EmisApiObjectResponse<AppointmentsGetResponse>(HttpStatusCode.Forbidden)
                {
                    StandardErrorResponse = errorResponse
                };
            MockEmisClientAppointmentsGetMethod(emisResponse);

            // Act
            var result = await _systemUnderTest.GetAppointments(_gpLinkedAccountModel);

            // Assert
            result.Should().BeAssignableTo<AppointmentsResult.Forbidden>();
        }

        [TestMethod]
        public async Task GetAppointments_EmisClientReturnsInternalServerErrorWithForbiddenMessage_ReturnsForbidden()
        {
            // Arrange
            var errorResponse = _fixture.Create<ExceptionErrorResponse>();
            errorResponse.Exceptions.First().Message =
                "Extra info: " + EmisApiErrorMessages.EmisService_NotEnabledForUser;
            var emisResponse =
                new EmisClient.EmisApiObjectResponse<AppointmentsGetResponse>(HttpStatusCode.InternalServerError)
                {
                    ExceptionErrorResponse = errorResponse
                };
            MockEmisClientAppointmentsGetMethod(emisResponse);

            // Act
            var result = await _systemUnderTest.GetAppointments(_gpLinkedAccountModel);

            // Assert
            result.Should().BeAssignableTo<AppointmentsResult.Forbidden>();
        }

        [TestMethod]
        public async Task GetAppointments_EmisClientReturnsUnknownError_ReturnsBadGateway()
        {
            // Arrange
            var errorResponse = _fixture.Create<ExceptionErrorResponse>();
            errorResponse.Exceptions.First().Message ="Extra info: UnknownError";
            var emisResponse =
                new EmisClient.EmisApiObjectResponse<AppointmentsGetResponse>(HttpStatusCode.Ambiguous)
                {
                    ExceptionErrorResponse = errorResponse
                };
            MockEmisClientAppointmentsGetMethod(emisResponse);

            // Act
            var result = await _systemUnderTest.GetAppointments(_gpLinkedAccountModel);

            // Assert
            result.Should().BeAssignableTo<AppointmentsResult.BadGateway>();
        }

        private void MockEmisClientAppointmentsGetMethod(
            EmisClient.EmisApiObjectResponse<AppointmentsGetResponse> response)
        {
            _mockEmisClient.Setup(x => x.AppointmentsGet(
                    It.Is<EmisRequestParameters>(p =>
                        p.EndUserSessionId.Equals(_emisUserSession.EndUserSessionId, StringComparison.Ordinal)
                        && p.SessionId.Equals(_emisUserSession.SessionId, StringComparison.Ordinal)
                        && p.UserPatientLinkToken.Equals(_emisUserSession.UserPatientLinkToken, StringComparison.Ordinal))))
                .ReturnsAsync(response);
        }
    }
}
