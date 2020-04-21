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
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Strategies.ResponseSuccessOutcome;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Emis.Messages
{
    [TestClass]
    public class EmisPatientMessagesServiceSendMessageTests
    {
        private IFixture _fixture;

        private Mock<IEmisClient> _mockClient;
        private Mock<IEmisPatientMessageSendMapper> _mockMessagesMapper;

        private EmisUserSession _userSession;
        private List<HttpStatusCode> _sampleSuccessStatusCodes;

        private EmisPatientMessagesService _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _mockClient = _fixture.Freeze<Mock<IEmisClient>>();
            _mockMessagesMapper = _fixture.Freeze<Mock<IEmisPatientMessageSendMapper>>();

            _userSession = _fixture.Create<EmisUserSession>();

            _systemUnderTest = _fixture.Create<EmisPatientMessagesService>();

            _sampleSuccessStatusCodes = new List<HttpStatusCode>
            {
                HttpStatusCode.OK
            };
        }

        [TestMethod]
        public async Task SendMessage_WhenSuccessfulResponseFromEmis_ReturnsSuccess()
        {
            // Arrange
            var messagePostResponse = _fixture.Create<MessagePostResponse>();
            var postMessageResponse = _fixture.Create<PostPatientMessageResponse>();
            var message = _fixture.Create<CreatePatientMessage>();
            message.RecipientIdentifier = "Recipient 1";
            message.Subject = "Subject";
            message.MessageBody = "Message";

            _mockClient
                .Setup(c => c.PatientMessagePost(GetMatchingEmisRequestParameters(), message))
                .Returns(Task.FromResult(new EmisClient.EmisApiObjectResponse<MessagePostResponse>(HttpStatusCode.OK,
                    RequestsForSuccessOutcome.SendPatientMessagePost, _sampleSuccessStatusCodes)
                {
                    Body = messagePostResponse
                }))
                .Verifiable();

            _mockMessagesMapper
                .Setup(e => e.Map(It.Is<MessagePostResponse>(m => m.Equals(messagePostResponse))))
                .Returns(Option.Some(postMessageResponse))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.SendMessage(_userSession, message);

            // Assert
            _mockClient.Verify();
            _mockMessagesMapper.Verify();

            result.Should().BeAssignableTo<PostPatientMessageResult.Success>();
        }

        [TestMethod]
        public async Task SendMessage_WhenBadRequestFromEmis_ReturnsBadRequest()
        {
            // Arrange
            var message = new CreatePatientMessage
            {
                Subject = "subject",
                MessageBody = "message",
                RecipientIdentifier = "recipient 1"
            };

            var badRequestErrorResponse = _fixture.Create<BadRequestErrorResponse>();

            _mockClient
                .Setup(c => c.PatientMessagePost(GetMatchingEmisRequestParameters(), message))
                .Returns(Task.FromResult(new EmisClient.EmisApiObjectResponse<MessagePostResponse>(HttpStatusCode.BadRequest,
                    RequestsForSuccessOutcome.SendPatientMessagePost, _sampleSuccessStatusCodes)
                {
                    ErrorResponseBadRequest = badRequestErrorResponse
                }))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.SendMessage(_userSession, message);

            // Assert
            _mockClient.Verify();

            result.Should().BeAssignableTo<PostPatientMessageResult.BadRequest>();
        }

        [TestMethod]
        public async Task SendMessage_WhenForbiddenFromEmis_ReturnsForbidden()
        {
            // Arrange
            var message = new CreatePatientMessage
            {
                Subject = "subject",
                MessageBody = "message",
                RecipientIdentifier = "recipient 1"
            };
            var exceptionErrorResponse = _fixture.Create<ExceptionErrorResponse>();
            exceptionErrorResponse.Exceptions.First().Message = EmisApiErrorMessages.EmisService_NotEnabledForUser;

            _mockClient
                .Setup(c => c.PatientMessagePost(GetMatchingEmisRequestParameters(), message))
                .Returns(Task.FromResult(new EmisClient.EmisApiObjectResponse<MessagePostResponse>(HttpStatusCode.Forbidden,
                    RequestsForSuccessOutcome.SendPatientMessagePost, _sampleSuccessStatusCodes)
                {
                    ExceptionErrorResponse = exceptionErrorResponse
                }))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.SendMessage(_userSession, message);

            // Assert
            _mockClient.Verify();

            result.Should().BeAssignableTo<PostPatientMessageResult.Forbidden>();
        }

        [TestMethod]
        public async Task SendMessage_WhenHttpExceptionIsThrown_ReturnsBadGateway()
        {
            // Arrange
            var message = new CreatePatientMessage
            {
                Subject = "subject",
                MessageBody = "message",
                RecipientIdentifier = "recipient 1"
            };

            _mockClient
                .Setup(c => c.PatientMessagePost(GetMatchingEmisRequestParameters(), message))
                .Throws<HttpRequestException>()
                .Verifiable();

            // Act
            var result = await _systemUnderTest.SendMessage(_userSession, message);

            // Assert
            _mockClient.Verify();

            result.Should().BeAssignableTo<PostPatientMessageResult.BadGateway>();
        }

        [TestMethod]
        public async Task SendMessage_WhenExceptionIsThrown_ReturnsInternalServerError()
        {
            // Arrange
            var message = new CreatePatientMessage
            {
                Subject = "subject",
                MessageBody = "message",
                RecipientIdentifier = "recipient 1"
            };

            _mockClient
                .Setup(c => c.PatientMessagePost(GetMatchingEmisRequestParameters(), message))
                .Throws<Exception>()
                .Verifiable();

            // Act
            var result = await _systemUnderTest.SendMessage(_userSession, message);

            // Assert
            _mockClient.Verify();

            result.Should().BeAssignableTo<PostPatientMessageResult.InternalServerError>();
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