using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
    public class EmisPatientMessagesServiceTests
    {
        private IFixture _fixture;

        private Mock<IEmisClient> _mockClient;
        private Mock<IEmisPatientMessagesMapper> _mockMapper;
        private Mock<IEmisPatientMessageMapper> _mockMessageMapper;
        private Mock<IEmisPatientMessageUpdateMapper> _mockMessagePutMapper;

        private EmisUserSession _userSession;
        private List<HttpStatusCode> _sampleSuccessStatusCodes;

        private EmisPatientMessagesService _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _mockClient = _fixture.Freeze<Mock<IEmisClient>>();
            _mockMapper = _fixture.Freeze<Mock<IEmisPatientMessagesMapper>>();
            _mockMessageMapper = _fixture.Freeze<Mock<IEmisPatientMessageMapper>>();
            _mockMessagePutMapper = _fixture.Freeze<Mock<IEmisPatientMessageUpdateMapper>>();

            _userSession = _fixture.Create<EmisUserSession>();

            _systemUnderTest = _fixture.Create<EmisPatientMessagesService>();

            _sampleSuccessStatusCodes = new List<HttpStatusCode>()
            {
                HttpStatusCode.OK
            };
        }

        [TestMethod]
        public async Task GetPatientMessages_WhenSuccessfulResponseFromEmis_ReturnsSuccess()
        {
            // Arrange
            var messagesGetResponse = _fixture.Create<MessagesGetResponse>();
            var getPatientMessagesResponse = _fixture.Create<GetPatientMessagesResponse>();

            _mockClient
                .Setup(GetMatchingExpression())
                .Returns(Task.FromResult(new EmisClient.EmisApiObjectResponse<MessagesGetResponse>(HttpStatusCode.OK, RequestsForSuccessOutcome.PatientMessagesGet, _sampleSuccessStatusCodes)
                {
                    Body = messagesGetResponse
                }))
                .Verifiable();
            _mockMapper
                .Setup(e => e.Map(It.Is<MessagesGetResponse>(m => m.Equals(messagesGetResponse))))
                .Returns(getPatientMessagesResponse)
                .Verifiable();

            // Act
            var result = await _systemUnderTest.GetMessages(_userSession);

            // Assert
            _mockClient.Verify();
            _mockMapper.Verify();

            result.Should().BeAssignableTo<GetPatientMessagesResult.Success>()
                .Subject.Response.Should().NotBeNull();
        }

        [TestMethod]
        public async Task GetPatientMessage_WhenSuccessfulResponseFromEmis_ReturnsSuccess()
        {
            // Arrange
            var messageGetResponse = _fixture.Create<MessageGetResponse>();
            var getPatientMessageResponse = _fixture.Create<GetPatientMessageResponse>();

            getPatientMessageResponse.MessageDetails.Content = messageGetResponse.Message.Content;
            getPatientMessageResponse.MessageDetails.Recipient = messageGetResponse.Message.Recipients[0].Name;
            getPatientMessageResponse.MessageDetails.Subject = messageGetResponse.Message.Subject;
            getPatientMessageResponse.MessageDetails.MessageId = messageGetResponse.Message.MessageId;
            getPatientMessageResponse.MessageDetails.MessageReplies = messageGetResponse.Message.MessageReplies;


            _mockClient
                .Setup(GetMessageExpression())
                .Returns(Task.FromResult(new EmisClient.EmisApiObjectResponse<MessageGetResponse>(HttpStatusCode.OK, RequestsForSuccessOutcome.PatientMessageDetailsGet, _sampleSuccessStatusCodes)
                {
                    Body = messageGetResponse
                }))
                .Verifiable();
            _mockMessageMapper
                .Setup(e => e.Map(It.Is<MessageGetResponse>(m => m.Equals(messageGetResponse))))
                .Returns(getPatientMessageResponse)
                .Verifiable();

            // Act
            var result = await _systemUnderTest.GetMessageDetails("1", _userSession);

            // Assert
            _mockClient.Verify();
            _mockMessageMapper.Verify();

            result.Should().BeAssignableTo<GetPatientMessageResult.Success>()
                .Subject.Response.Should().NotBeNull();
        }

        [TestMethod]
        public async Task GetPatientMessage_WhenBadRequestFromEmis_ReturnsBadRequest()
        {
            // Arrange
            var badRequestErrorResponse = _fixture.Create<BadRequestErrorResponse>();

            _mockClient
                .Setup(GetMessageExpression())
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<MessageGetResponse>(HttpStatusCode.BadRequest, RequestsForSuccessOutcome.PatientMessageDetailsGet, _sampleSuccessStatusCodes)
                    {
                        ErrorResponseBadRequest = badRequestErrorResponse
                    }))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.GetMessageDetails("1", _userSession);

            // Assert
            _mockClient.Verify();

            result.Should().BeAssignableTo<GetPatientMessageResult.BadRequest>();
        }

        [TestMethod]
        public async Task GetPatientMessage_WhenInternalServerErrorFromEmis_ReturnsInternalServerError()
        {
            // Arrange
            _mockClient
                .Setup(GetMatchingExpression())
                .Throws<Exception>();

            // Act
            var result = await _systemUnderTest.GetMessageDetails("1", _userSession);

            // Assert
            result.Should().BeAssignableTo<GetPatientMessageResult.InternalServerError>();
        }

        [TestMethod]
        public async Task GetPatientMessage_WhenForbiddenResponseFromEmis_ReturnsForbidden()
        {
            // Arrange
            var exceptionErrorResponse = _fixture.Create<ExceptionErrorResponse>();
            exceptionErrorResponse.Exceptions.First().Message = EmisApiErrorMessages.EmisService_NotEnabledForUser;

            _mockClient
                .Setup(GetMessageExpression())
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<MessageGetResponse>(HttpStatusCode.Forbidden, RequestsForSuccessOutcome.PatientMessageDetailsGet, _sampleSuccessStatusCodes)
                    {
                        ExceptionErrorResponse = exceptionErrorResponse
                    }))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.GetMessageDetails("1", _userSession);

            // Assert
            _mockClient.Verify();

            result.Should().BeAssignableTo<GetPatientMessageResult.Forbidden>();
        }

        [TestMethod]
        public async Task GetPatientMessages_WhenForbiddenResponseFromEmis_ReturnsForbidden()
        {
            // Arrange
            var exceptionErrorResponse = _fixture.Create<ExceptionErrorResponse>();
            exceptionErrorResponse.Exceptions.First().Message = EmisApiErrorMessages.EmisService_NotEnabledForUser;

            _mockClient
                .Setup(GetMatchingExpression())
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<MessagesGetResponse>(HttpStatusCode.Forbidden, RequestsForSuccessOutcome.PatientMessagesGet, _sampleSuccessStatusCodes)
                    {
                        ExceptionErrorResponse = exceptionErrorResponse
                    }))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.GetMessages(_userSession);

            // Assert
            _mockClient.Verify();

            result.Should().BeAssignableTo<GetPatientMessagesResult.Forbidden>();
        }

        [TestMethod]
        public async Task GetPatientMessages_WhenExceptionIsThrown_ReturnsBadGateway()
        {
            // Arrange
            _mockClient
                .Setup(GetMatchingExpression())
                .Throws<HttpRequestException>()
                .Verifiable();

            // Act
            var result = await _systemUnderTest.GetMessages(_userSession);

            // Assert
            _mockClient.Verify();

            result.Should().BeAssignableTo<GetPatientMessagesResult.BadGateway>();
        }

        [TestMethod]
        public async Task GetPatientMessages_WhenBadRequestFromEmis_ReturnsBadRequest()
        {
            // Arrange
            var badRequestErrorResponse = _fixture.Create<BadRequestErrorResponse>();

            _mockClient
                .Setup(GetMatchingExpression())
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<MessagesGetResponse>(HttpStatusCode.BadRequest, RequestsForSuccessOutcome.PatientMessagesGet, _sampleSuccessStatusCodes)
                    {
                        ErrorResponseBadRequest = badRequestErrorResponse
                    }))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.GetMessages(_userSession);

            // Assert
            _mockClient.Verify();

            result.Should().BeAssignableTo<GetPatientMessagesResult.BadRequest>();
        }

        [TestMethod]
        public async Task GetPatientMessages_WhenMapperReturnsNull_ReturnsBadGateway()
        {
            // Arrange
            var messagesGetResponse = _fixture.Create<MessagesGetResponse>();
            messagesGetResponse.Messages = new List<MessageSummary>();

            _mockClient
                .Setup(GetMatchingExpression())
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<MessagesGetResponse>(HttpStatusCode.OK, RequestsForSuccessOutcome.PatientMessagesGet, _sampleSuccessStatusCodes)
                    {
                        Body = messagesGetResponse
                    }));
            _mockMapper
                .Setup(e => e.Map(It.Is<MessagesGetResponse>(m => m.Equals(messagesGetResponse))));

            // Act
            var result = await _systemUnderTest.GetMessages(_userSession);

            // Assert
            result.Should().BeAssignableTo<GetPatientMessagesResult.BadGateway>();
        }

        [TestMethod]
        public async Task GetPatientMessages_WhenUnknownExceptionIsThrown_ReturnsInternalServerError()
        {
            // Arrange
            _mockClient
                .Setup(GetMatchingExpression())
                .Throws<Exception>();

            // Act
            var result = await _systemUnderTest.GetMessages(_userSession);

            // Assert
            result.Should().BeAssignableTo<GetPatientMessagesResult.InternalServerError>();
        }

        [TestMethod]
        public async Task GetPatientMessages_WhenUnknownErrorOccurs_ReturnsBadGateway()
        {
            // Arrange
            var exceptionErrorResponse = _fixture.Create<ExceptionErrorResponse>();

            _mockClient
                .Setup(GetMatchingExpression())
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<MessagesGetResponse>(HttpStatusCode.InternalServerError, RequestsForSuccessOutcome.PatientMessagesGet, _sampleSuccessStatusCodes)
                    {
                        ExceptionErrorResponse = exceptionErrorResponse
                    }))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.GetMessages(_userSession);

            // Assert
            _mockClient.Verify();

            result.Should().BeAssignableTo<GetPatientMessagesResult.BadGateway>();
        }

        [TestMethod]
        public async Task PutPatientMessageUpdateReadStatus_WhenSuccessfulResponseFromEmis_ReturnsSuccess()
        {
            // Arrange
            var messageUpdateResponse = _fixture.Create<MessageUpdateResponse>();
            var putPatientMessageUpdateStatusResponse = _fixture.Create<PutPatientMessageUpdateStatusResponse>();

            putPatientMessageUpdateStatusResponse.MessageReadStateUpdateStatus =
                messageUpdateResponse.MessageReadStateUpdateStatus;

            var requestBody = new UpdateMessageReadStatusRequestBody()
            {
                MessageId = 1,
                MessageReadState = "Read"
            };

            _mockClient
                .Setup(PutMessageUpdateExpression())
                .Returns(Task.FromResult(new EmisClient.EmisApiObjectResponse<MessageUpdateResponse>(HttpStatusCode.OK, RequestsForSuccessOutcome.PatientMessageUpdatePut, _sampleSuccessStatusCodes)
                {
                    Body = messageUpdateResponse
                }))
                .Verifiable();
            _mockMessagePutMapper
                .Setup(e => e.Map(It.Is<MessageUpdateResponse>(m => m.Equals(messageUpdateResponse))))
                .Returns(putPatientMessageUpdateStatusResponse)
                .Verifiable();

            // Act
            var result = await _systemUnderTest.UpdateMessageMessageReadStatus( _userSession, requestBody);

            // Assert
            _mockClient.Verify();
            _mockMessagePutMapper.Verify();

            result.Should().BeAssignableTo<PutPatientMessageReadStatusResult.Success>()
                .Subject.Response.Should().NotBeNull();
        }

        [TestMethod]
        public async Task PutPatientMessageUpdateReadStatus_WhenBadRequestFromEmis_ReturnsBadRequest()
        {
            // Arrange
            var badRequestErrorResponse = _fixture.Create<BadRequestErrorResponse>();

            var requestBody = new UpdateMessageReadStatusRequestBody()
            {
                MessageId = 1,
                MessageReadState = "Read"
            };

            _mockClient
                .Setup(PutMessageUpdateExpression())
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<MessageUpdateResponse>(HttpStatusCode.BadRequest, RequestsForSuccessOutcome.PatientMessageUpdatePut, _sampleSuccessStatusCodes)
                    {
                        ErrorResponseBadRequest = badRequestErrorResponse
                    }))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.UpdateMessageMessageReadStatus(_userSession, requestBody);

            // Assert
            _mockClient.Verify();

            result.Should().BeAssignableTo<PutPatientMessageReadStatusResult.BadRequest>();
        }

        [TestMethod]
        public async Task PutPatientMessageUpdateReadStatus_WhenInternalServerErrorFromEmis_ReturnsInternalServerError()
        {
            // Arrange
            _mockClient
                .Setup(GetMatchingExpression())
                .Throws<Exception>();

            var requestBody = new UpdateMessageReadStatusRequestBody()
            {
                MessageId = 1,
                MessageReadState = "Read"
            };

            // Act
            var result = await _systemUnderTest.UpdateMessageMessageReadStatus(_userSession, requestBody);

            // Assert
            result.Should().BeAssignableTo<PutPatientMessageReadStatusResult.InternalServerError>();
        }

        [TestMethod]
        public async Task PutPatientMessageUpdateReadStatus_WhenForbiddenResponseFromEmis_ReturnsForbidden()
        {
            // Arrange
            var exceptionErrorResponse = _fixture.Create<ExceptionErrorResponse>();
            exceptionErrorResponse.Exceptions.First().Message = EmisApiErrorMessages.EmisService_NotEnabledForUser;

            var requestBody = new UpdateMessageReadStatusRequestBody()
            {
                MessageId = 1,
                MessageReadState = "Read"
            };

            _mockClient
                .Setup(PutMessageUpdateExpression())
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<MessageUpdateResponse>(HttpStatusCode.Forbidden, RequestsForSuccessOutcome.PatientMessageUpdatePut, _sampleSuccessStatusCodes)
                    {
                        ExceptionErrorResponse = exceptionErrorResponse
                    }))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.UpdateMessageMessageReadStatus(_userSession, requestBody);

            // Assert
            _mockClient.Verify();

            result.Should().BeAssignableTo<PutPatientMessageReadStatusResult.Forbidden>();
        }

        private Expression<Func<IEmisClient, Task<EmisClient.EmisApiObjectResponse<MessagesGetResponse>>>>
            GetMatchingExpression()
        {
            return c => c.PatientMessagesGet(It.Is<EmisRequestParameters>(e =>
                _userSession.SessionId.Equals(e.SessionId, StringComparison.Ordinal) &&
                _userSession.EndUserSessionId.Equals(e.EndUserSessionId, StringComparison.Ordinal) &&
                _userSession.UserPatientLinkToken.Equals(e.UserPatientLinkToken, StringComparison.Ordinal)));
        }

        private Expression<Func<IEmisClient, Task<EmisClient.EmisApiObjectResponse<MessageGetResponse>>>>
            GetMessageExpression()
        {
            return c => c.PatientMessageDetailsGet("1", It.Is<EmisRequestParameters>(e =>
                _userSession.SessionId.Equals(e.SessionId, StringComparison.Ordinal) &&
                _userSession.EndUserSessionId.Equals(e.EndUserSessionId, StringComparison.Ordinal) &&
                _userSession.UserPatientLinkToken.Equals(e.UserPatientLinkToken, StringComparison.Ordinal)));
        }

        private Expression<Func<IEmisClient, Task<EmisClient.EmisApiObjectResponse<MessageUpdateResponse>>>>
            PutMessageUpdateExpression()
        {
            var requestBody = new UpdateMessageReadStatusRequest
            {
                UserPatientLinkToken = _userSession.UserPatientLinkToken,
                MessageId = 1,
                MessageReadState = "Read"
            };
            return c => c.PatientMessageUpdatePut( It.Is<EmisRequestParameters>(e =>
                _userSession.SessionId.Equals(e.SessionId, StringComparison.Ordinal) &&
                _userSession.EndUserSessionId.Equals(e.EndUserSessionId, StringComparison.Ordinal) &&
                _userSession.UserPatientLinkToken.Equals(e.UserPatientLinkToken, StringComparison.Ordinal)),
                It.Is<UpdateMessageReadStatusRequest>(p =>
                    p.MessageId == requestBody.MessageId && p.MessageReadState.Equals(requestBody.MessageReadState, StringComparison.Ordinal)
                    && p.UserPatientLinkToken.Equals(requestBody.UserPatientLinkToken, StringComparison.Ordinal)));
        }
    }
}