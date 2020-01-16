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
using NHSOnline.Backend.GpSystems.Messages;
using NHSOnline.Backend.GpSystems.Messages.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Messages;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.Messages;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Strategies.ResponseSuccessOutcome;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Emis.Messages
{
    [TestClass]
    public class EmisPatientMessagesServiceGetMessagesTests
    {
        private IFixture _fixture;

        private Mock<IEmisClient> _mockClient;
        private Mock<IEmisPatientMessagesMapper> _mockMessagesMapper;

        private EmisUserSession _userSession;
        private List<HttpStatusCode> _sampleSuccessStatusCodes;

        private EmisPatientMessagesService _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _mockClient = _fixture.Freeze<Mock<IEmisClient>>();
            _mockMessagesMapper = _fixture.Freeze<Mock<IEmisPatientMessagesMapper>>();

            _userSession = _fixture.Create<EmisUserSession>();

            _systemUnderTest = _fixture.Create<EmisPatientMessagesService>();

            _sampleSuccessStatusCodes = new List<HttpStatusCode>
            {
                HttpStatusCode.OK
            };
        }

        [TestMethod]
        public async Task GetMessages_WhenSuccessfulResponseFromEmis_ReturnsSuccess()
        {
            // Arrange
            var messagesGetResponse = _fixture.Create<MessagesGetResponse>();
            var getPatientMessagesResponse = _fixture.Create<GetPatientMessagesResponse>();

            _mockClient
                .Setup(c => c.PatientMessagesGet(GetMatchingEmisRequestParameters()))
                .Returns(Task.FromResult(new EmisClient.EmisApiObjectResponse<MessagesGetResponse>(HttpStatusCode.OK,
                    RequestsForSuccessOutcome.PatientMessagesGet, _sampleSuccessStatusCodes)
                {
                    Body = messagesGetResponse
                }))
                .Verifiable();
            
            _mockMessagesMapper
                .Setup(e => e.Map(It.Is<MessagesGetResponse>(m => m.Equals(messagesGetResponse))))
                .Returns(getPatientMessagesResponse)
                .Verifiable();

            // Act
            var result = await _systemUnderTest.GetMessages(_userSession);

            // Assert
            _mockClient.Verify();
            _mockMessagesMapper.Verify();

            result.Should().BeAssignableTo<GetPatientMessagesResult.Success>()
                .Subject.Response.Should().NotBeNull();
        }

        [TestMethod]
        public async Task GetMessages_WhenBadRequestFromEmis_ReturnsBadRequest()
        {
            // Arrange
            var badRequestErrorResponse = _fixture.Create<BadRequestErrorResponse>();

            _mockClient
                .Setup(c => c.PatientMessagesGet(GetMatchingEmisRequestParameters()))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<MessagesGetResponse>(HttpStatusCode.BadRequest,
                        RequestsForSuccessOutcome.PatientMessagesGet, _sampleSuccessStatusCodes)
                    {
                        ErrorResponseBadRequest = badRequestErrorResponse
                    }))
                .Verifiable();

            // Act
            var getMessagesResult = await _systemUnderTest.GetMessages(_userSession);
            
            // Assert
            _mockClient.Verify();

            getMessagesResult.Should().BeAssignableTo<GetPatientMessagesResult.BadRequest>();
        }

        [TestMethod]
        public async Task GetMessages_WhenForbiddenResponseFromEmis_ReturnsForbidden()
        {
            // Arrange
            var exceptionErrorResponse = _fixture.Create<ExceptionErrorResponse>();
            exceptionErrorResponse.Exceptions.First().Message = EmisApiErrorMessages.EmisService_NotEnabledForUser;

            _mockClient
                .Setup(c => c.PatientMessagesGet(GetMatchingEmisRequestParameters()))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<MessagesGetResponse>(HttpStatusCode.Forbidden,
                        RequestsForSuccessOutcome.PatientMessagesGet, _sampleSuccessStatusCodes)
                    {
                        ExceptionErrorResponse = exceptionErrorResponse
                    }))
                .Verifiable();

            // Act
            var getMessagesResult = await _systemUnderTest.GetMessages(_userSession);

            // Assert
            _mockClient.Verify();

            getMessagesResult.Should().BeAssignableTo<GetPatientMessagesResult.Forbidden>();
        }
        
        [TestMethod]
        public async Task GetMessages_WhenHttpExceptionIsThrown_ReturnsBadGateway()
        {
            // Arrange
            _mockClient
                .Setup(c => c.PatientMessagesGet(GetMatchingEmisRequestParameters()))
                .Throws<HttpRequestException>()
                .Verifiable();

            // Act
            var getMessagesResult = await _systemUnderTest.GetMessages(_userSession);

            // Assert
            _mockClient.Verify();
         
            getMessagesResult.Should().BeAssignableTo<GetPatientMessagesResult.BadGateway>();
        }
        
        [TestMethod]
        public async Task GetMessages_WhenExceptionIsThrown_ReturnsInternalServerError()
        {
            // Arrange
            _mockClient
                .Setup(c => c.PatientMessagesGet(GetMatchingEmisRequestParameters()))
                .Throws<Exception>()
                .Verifiable();

            // Act
            var getMessagesResult = await _systemUnderTest.GetMessages(_userSession);

            // Assert
            _mockClient.Verify();
            
            getMessagesResult.Should().BeAssignableTo<GetPatientMessagesResult.InternalServerError>();
        }

        [TestMethod]
        public async Task GetMessages_WhenMapperReturnsNull_ReturnsBadGateway()
        {
            // Arrange
            var messagesGetResponse = _fixture.Create<MessagesGetResponse>();
            messagesGetResponse.Messages = new List<MessageSummary>();

            _mockClient
                .Setup(c => c.PatientMessagesGet(GetMatchingEmisRequestParameters()))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<MessagesGetResponse>(HttpStatusCode.OK,
                        RequestsForSuccessOutcome.PatientMessagesGet, _sampleSuccessStatusCodes)
                    {
                        Body = messagesGetResponse
                    }))
                .Verifiable();

            _mockMessagesMapper
                .Setup(e => e.Map(
                    It.Is<MessagesGetResponse>(m => m.Equals(messagesGetResponse))))
                .Verifiable();

            // Act
            var messagesResult = await _systemUnderTest.GetMessages(_userSession);

            // Assert
            _mockClient.Verify();
            _mockMessagesMapper.Verify();
            
            messagesResult.Should().BeAssignableTo<GetPatientMessagesResult.BadGateway>();
        }

        [TestMethod]
        public async Task GetMessages_WhenUnknownExceptionIsThrown_ReturnsInternalServerError()
        {
            // Arrange
            _mockClient
                .Setup(c => c.PatientMessagesGet(GetMatchingEmisRequestParameters()))
                .Throws<Exception>()
                .Verifiable();

            // Act
            var getMessagesResult = await _systemUnderTest.GetMessages(_userSession);

            // Assert
            _mockClient.Verify();
            
            getMessagesResult.Should().BeAssignableTo<GetPatientMessagesResult.InternalServerError>();
        }

        [TestMethod]
        public async Task GetMessages_WhenUnknownErrorOccurs_ReturnsBadGateway()
        {
            // Arrange
            var exceptionErrorResponse = _fixture.Create<ExceptionErrorResponse>();

            _mockClient
                .Setup(c => c.PatientMessagesGet(GetMatchingEmisRequestParameters()))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<MessagesGetResponse>(HttpStatusCode.InternalServerError,
                        RequestsForSuccessOutcome.PatientMessagesGet, _sampleSuccessStatusCodes)
                    {
                        ExceptionErrorResponse = exceptionErrorResponse
                    }))
                .Verifiable();

            // Act
            var getMessagesResult = await _systemUnderTest.GetMessages(_userSession);

            // Assert
            _mockClient.Verify();

            getMessagesResult.Should().BeAssignableTo<GetPatientMessagesResult.BadGateway>();
        }

        private EmisRequestParameters GetMatchingEmisRequestParameters()
        {
            return It.Is<EmisRequestParameters>(e =>
                _userSession.SessionId.Equals(e.SessionId, StringComparison.Ordinal) &&
                _userSession.EndUserSessionId.Equals(e.EndUserSessionId, StringComparison.Ordinal) &&
                _userSession.UserPatientLinkToken.Equals(e.UserPatientLinkToken, StringComparison.Ordinal));
        }
    }
}