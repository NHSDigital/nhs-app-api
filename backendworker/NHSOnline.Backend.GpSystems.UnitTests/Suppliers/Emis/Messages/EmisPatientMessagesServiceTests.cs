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

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Emis.Messages
{
    [TestClass]
    public class EmisPatientMessagesServiceTests
    {
        private IFixture _fixture;

        private Mock<IEmisClient> _mockClient;
        private Mock<IEmisPatientMessagesMapper> _mockMapper;

        private EmisUserSession _userSession;

        private EmisPatientMessagesService _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _mockClient = _fixture.Freeze<Mock<IEmisClient>>();
            _mockMapper = _fixture.Freeze<Mock<IEmisPatientMessagesMapper>>();

            _userSession = _fixture.Create<EmisUserSession>();

            _systemUnderTest = _fixture.Create<EmisPatientMessagesService>();
        }

        [TestMethod]
        public async Task GetPatientMessages_WhenSuccessfulResponseFromEmis_ReturnsSuccess()
        {
            // Arrange
            var messagesGetResponse = _fixture.Create<MessagesGetResponse>();
            var getPatientMessagesResponse = _fixture.Create<GetPatientMessagesResponse>();

            _mockClient
                .Setup(GetMatchingExpression())
                .Returns(Task.FromResult(new EmisClient.EmisApiObjectResponse<MessagesGetResponse>(HttpStatusCode.OK)
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
        public async Task GetPatientMessages_WhenForbiddenResponseFromEmis_ReturnsForbidden()
        {
            // Arrange
            var exceptionErrorResponse = _fixture.Create<ExceptionErrorResponse>();
            exceptionErrorResponse.Exceptions.First().Message = EmisApiErrorMessages.EmisService_NotEnabledForUser;

            _mockClient
                .Setup(GetMatchingExpression())
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<MessagesGetResponse>(HttpStatusCode.Forbidden)
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
                    new EmisClient.EmisApiObjectResponse<MessagesGetResponse>(HttpStatusCode.BadRequest)
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
                    new EmisClient.EmisApiObjectResponse<MessagesGetResponse>(HttpStatusCode.OK)
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
                    new EmisClient.EmisApiObjectResponse<MessagesGetResponse>(HttpStatusCode.InternalServerError)
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

        private Expression<Func<IEmisClient, Task<EmisClient.EmisApiObjectResponse<MessagesGetResponse>>>>
            GetMatchingExpression()
        {
            return c => c.PatientMessagesGet(It.Is<EmisRequestParameters>(e =>
                _userSession.SessionId.Equals(e.SessionId, StringComparison.Ordinal) &&
                _userSession.EndUserSessionId.Equals(e.EndUserSessionId, StringComparison.Ordinal) &&
                _userSession.UserPatientLinkToken.Equals(e.UserPatientLinkToken, StringComparison.Ordinal)));
        }
    }
}