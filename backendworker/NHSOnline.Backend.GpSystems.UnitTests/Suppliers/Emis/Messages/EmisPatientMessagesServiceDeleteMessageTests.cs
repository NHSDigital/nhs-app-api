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

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Emis.Messages
{
    [TestClass]
    public class EmisPatientMessagesServiceDeleteMessageTests
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

            _sampleSuccessStatusCodes = new List<HttpStatusCode>
            {
                HttpStatusCode.OK
            };
        }

        [TestMethod]
        public async Task DeleteMessage_WhenSuccessfulResponseFromEmis_ReturnsSuccess()
        {
            // Arrange
            var messageDeleteResponse = _fixture.Create<MessageDeleteResponse>();
            messageDeleteResponse.IsDeleted = true;
            var deleteMessageResponse = _fixture.Create<DeletePatientMessageResponse>();
            deleteMessageResponse.IsDeleted = true;
            var messageId = "1";

            _mockClient
                .Setup(c => c.PatientPracticeMessageDelete(GetMatchingEmisRequestParameters(), messageId))
                .Returns(Task.FromResult(new EmisApiObjectResponse<MessageDeleteResponse>(HttpStatusCode.OK,
                    RequestsForSuccessOutcome.PatientPracticeMessageDelete, _sampleSuccessStatusCodes)
                {
                    Body = messageDeleteResponse
                }))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.DeleteMessage(_userSession, messageId);

            // Assert
            _mockClient.Verify();

            result.Should().BeAssignableTo<DeletePatientMessageResult.Success>();
        }

        [TestMethod]
        public async Task DeleteMessage_WhenBadRequestFromEmis_ReturnsBadRequest()
        {
            // Arrange
            var badRequestErrorResponse = _fixture.Create<BadRequestErrorResponse>();

            _mockClient
                .Setup(c => c.PatientPracticeMessageDelete(GetMatchingEmisRequestParameters(), "1"))
                .Returns(Task.FromResult(new EmisApiObjectResponse<MessageDeleteResponse>(HttpStatusCode.BadRequest,
                    RequestsForSuccessOutcome.PatientPracticeMessageDelete, _sampleSuccessStatusCodes)
                {
                    ErrorResponseBadRequest = badRequestErrorResponse
                }))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.DeleteMessage(_userSession, "1");

            // Assert
            _mockClient.Verify();

            result.Should().BeAssignableTo<DeletePatientMessageResult.BadRequest>();
        }

        [TestMethod]
        public async Task DeleteMessage_WhenForbiddenFromEmis_ReturnsForbidden()
        {
            // Arrange
            var exceptionErrorResponse = _fixture.Create<ExceptionErrorResponse>();
            exceptionErrorResponse.Exceptions.First().Message = EmisApiErrorMessages.EmisService_NotEnabledForUser;

            _mockClient
                .Setup(c => c.PatientPracticeMessageDelete(GetMatchingEmisRequestParameters(), "1"))
                .Returns(Task.FromResult(new EmisApiObjectResponse<MessageDeleteResponse>(HttpStatusCode.Forbidden,
                    RequestsForSuccessOutcome.PatientPracticeMessageDelete, _sampleSuccessStatusCodes)
                {
                    ExceptionErrorResponse = exceptionErrorResponse
                }))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.DeleteMessage(_userSession, "1");

            // Assert
            _mockClient.Verify();

            result.Should().BeAssignableTo<DeletePatientMessageResult.Forbidden>();
        }

        [TestMethod]
        public async Task DeleteMessage_WhenHttpExceptionIsThrown_ReturnsBadGateway()
        {
            // Arrange
            _mockClient
                .Setup(c => c.PatientPracticeMessageDelete(GetMatchingEmisRequestParameters(), "1"))
                .Throws<HttpRequestException>()
                .Verifiable();

            // Act
            var result = await _systemUnderTest.DeleteMessage(_userSession, "1");

            // Assert
            _mockClient.Verify();

            result.Should().BeAssignableTo<DeletePatientMessageResult.BadGateway>();
        }

        [TestMethod]
        public async Task DeleteMessage_WhenExceptionIsThrown_ReturnsInternalServerError()
        {
            // Arrange
            _mockClient
                .Setup(c => c.PatientPracticeMessageDelete(GetMatchingEmisRequestParameters(), "1"))
                .Throws<Exception>()
                .Verifiable();

            // Act
            var result = await _systemUnderTest.DeleteMessage(_userSession, "1");

            // Assert
            _mockClient.Verify();

            result.Should().BeAssignableTo<DeletePatientMessageResult.InternalServerError>();
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