using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
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

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Emis.Messages
{
    [TestClass]
    public class EmisPatientMessagesServiceTests
    {
        private IFixture _fixture;

        private Mock<IEmisClient> _mockClient;

        private EmisUserSession _userSession;
        private List<HttpStatusCode> _sampleSuccessStatusCodes;

        private EmisPatientMessagesService _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _mockClient = _fixture.Freeze<Mock<IEmisClient>>();
            _userSession = _fixture.Create<EmisUserSession>();

            _systemUnderTest = _fixture.Create<EmisPatientMessagesService>();

            _sampleSuccessStatusCodes = new List<HttpStatusCode>()
            {
                HttpStatusCode.OK
            };
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
                MessageId = "1",
                MessageReadState = "Read"
            };

            _mockClient
                .Setup(PutMessageUpdateExpression())
                .Returns(Task.FromResult(new EmisApiObjectResponse<MessageUpdateResponse>(HttpStatusCode.OK, RequestsForSuccessOutcome.PatientMessageUpdatePut, _sampleSuccessStatusCodes)
                {
                    Body = messageUpdateResponse
                }))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.UpdateMessageMessageReadStatus( _userSession, requestBody);

            // Assert
            _mockClient.Verify();

            result.Should().BeAssignableTo<PutPatientMessageReadStatusResult.Success>();
        }

        [TestMethod]
        public async Task PutPatientMessageUpdateReadStatus_WhenBadRequestFromEmis_ReturnsBadRequest()
        {
            // Arrange
            var badRequestErrorResponse = _fixture.Create<BadRequestErrorResponse>();

            var requestBody = new UpdateMessageReadStatusRequestBody()
            {
                MessageId = "1",
                MessageReadState = "Read"
            };

            _mockClient
                .Setup(PutMessageUpdateExpression())
                .Returns(Task.FromResult(
                    new EmisApiObjectResponse<MessageUpdateResponse>(HttpStatusCode.BadRequest, RequestsForSuccessOutcome.PatientMessageUpdatePut, _sampleSuccessStatusCodes)
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
                MessageId = "1",
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
                MessageId = "1",
                MessageReadState = "Read"
            };

            _mockClient
                .Setup(PutMessageUpdateExpression())
                .Returns(Task.FromResult(
                    new EmisApiObjectResponse<MessageUpdateResponse>(HttpStatusCode.Forbidden, RequestsForSuccessOutcome.PatientMessageUpdatePut, _sampleSuccessStatusCodes)
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

        [TestMethod]
        [DataRow("-1", DisplayName = "negative")]
        [DataRow("1.01", DisplayName = "double")]
        [DataRow(null, DisplayName = "null")]
        [DataRow("", DisplayName = "empty")]
        [DataRow("\t\r  \r\n", DisplayName = "blank")]
        public async Task PutPatientMessageUpdateReadStatus_WhenMessageIdIsNotAPositiveInteger_ThenArgumentExceptionIsThrown(string value)
        {
            var request = new UpdateMessageReadStatusRequestBody()
            {
                MessageId = value,
                MessageReadState = "Read"
            };

            await _systemUnderTest.Awaiting(s => s.UpdateMessageMessageReadStatus(_userSession, request))
                .Should()
                .ThrowAsync<ArgumentException>();
        }

        private Expression<Func<IEmisClient, Task<EmisApiObjectResponse<MessagesGetResponse>>>>
            GetMatchingExpression()
        {
            return c => c.PatientMessagesGet(It.Is<EmisRequestParameters>(e =>
                _userSession.SessionId.Equals(e.SessionId, StringComparison.Ordinal) &&
                _userSession.EndUserSessionId.Equals(e.EndUserSessionId, StringComparison.Ordinal) &&
                _userSession.UserPatientLinkToken.Equals(e.UserPatientLinkToken, StringComparison.Ordinal)));
        }

        private Expression<Func<IEmisClient, Task<EmisApiObjectResponse<MessageUpdateResponse>>>>
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