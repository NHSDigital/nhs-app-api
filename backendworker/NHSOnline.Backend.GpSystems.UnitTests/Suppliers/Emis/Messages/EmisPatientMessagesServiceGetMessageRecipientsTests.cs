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
    public class EmisPatientMessagesServiceGetMessageRecipientsTests
    {
        private IFixture _fixture;

        private Mock<IEmisClient> _mockClient;
        private Mock<IEmisPatientMessageRecipientsMapper> _mockMapper;

        private EmisUserSession _userSession;
        private List<HttpStatusCode> _sampleSuccessStatusCodes;

        private EmisPatientMessagesService _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _mockClient = _fixture.Freeze<Mock<IEmisClient>>();
            _mockMapper = _fixture.Freeze<Mock<IEmisPatientMessageRecipientsMapper>>();

            _userSession = _fixture.Create<EmisUserSession>();

            _systemUnderTest = _fixture.Create<EmisPatientMessagesService>();

            _sampleSuccessStatusCodes = new List<HttpStatusCode>
            {
                HttpStatusCode.OK
            };
        }

        [TestMethod]
        public async Task GetMessageRecipients_WhenSuccessfulResponseFromEmis_MapsResponseAndReturnsSuccess()
        {
            // Arrange
            var messageRecipientsGetResponse = _fixture.Create<MessageRecipientsResponse>();

            var messageRecipients = new List<MessageRecipient>();

            foreach (var recipient in messageRecipientsGetResponse.MessageRecipients)
            {
                messageRecipients.Add(new MessageRecipient
                {
                    RecipientIdentifier = recipient.RecipientGuid,
                    Name = recipient.Name
                });
            }

            var mappedMessageResponse = new PatientPracticeMessageRecipients
            {
                MessageRecipients = messageRecipients
            };

            _mockClient
                .Setup(c => c.PatientMessageRecipientsGet(GetMatchingEmisRequestParameters()))
                .Returns(Task.FromResult(new EmisApiObjectResponse<MessageRecipientsResponse>(
                    HttpStatusCode.OK,
                    RequestsForSuccessOutcome.PatientMessageDetailsGet, _sampleSuccessStatusCodes)
                {
                    Body = messageRecipientsGetResponse
                }))
                .Verifiable();
            _mockMapper
                .Setup(m => m.Map(It.Is<MessageRecipientsResponse>(r => r.Equals(messageRecipientsGetResponse))))
                .Returns(mappedMessageResponse)
                .Verifiable();

            // Act
            var result = await _systemUnderTest.GetMessageRecipients(_userSession);

            // Assert
            _mockClient.Verify();
            _mockMapper.Verify();

            result.Should().BeAssignableTo<GetPatientMessageRecipientsResult.Success>()
                .Subject.Response.Should().BeEquivalentTo(mappedMessageResponse);
        }

        [TestMethod]
        public async Task GetMessageRecipients_WhenBadRequestFromEmis_ReturnsBadRequest()
        {
            // Arrange
            var badRequestErrorResponse = _fixture.Create<BadRequestErrorResponse>();

            _mockClient
                .Setup(c => c.PatientMessageRecipientsGet(GetMatchingEmisRequestParameters()))
                .Returns(Task.FromResult(
                    new EmisApiObjectResponse<MessageRecipientsResponse>(HttpStatusCode.BadRequest,
                        RequestsForSuccessOutcome.PatientMessageRecipientsGet, _sampleSuccessStatusCodes)
                    {
                        ErrorResponseBadRequest = badRequestErrorResponse
                    }))
                .Verifiable();

            // Act
            var getMessageRecipientsResult = await _systemUnderTest.GetMessageRecipients(_userSession);

            // Assert
            _mockClient.Verify();

            getMessageRecipientsResult.Should().BeAssignableTo<GetPatientMessageRecipientsResult.BadRequest>();
        }

        [TestMethod]
        public async Task GetMessageRecipients_WhenForbiddenResponseFromEmis_ReturnsForbidden()
        {
            // Arrange
            var exceptionErrorResponse = _fixture.Create<ExceptionErrorResponse>();
            exceptionErrorResponse.Exceptions.First().Message = EmisApiErrorMessages.EmisService_NotEnabledForUser;

            _mockClient.Setup(c => c.PatientMessageRecipientsGet(GetMatchingEmisRequestParameters()))
                .Returns(Task.FromResult(
                    new EmisApiObjectResponse<MessageRecipientsResponse>(HttpStatusCode.Forbidden,
                        RequestsForSuccessOutcome.PatientMessageRecipientsGet, _sampleSuccessStatusCodes)
                    {
                        ExceptionErrorResponse = exceptionErrorResponse
                    })).Verifiable();

            // Act
            var getMessageRecipientsResult = await _systemUnderTest.GetMessageRecipients(_userSession);

            // Assert
            _mockClient.Verify();

            getMessageRecipientsResult.Should().BeAssignableTo<GetPatientMessageRecipientsResult.Forbidden>();
        }

        [TestMethod]
        public async Task GetMessageRecipients_WhenHttpExceptionIsThrown_ReturnsBadGateway()
        {
            // Arrange
            _mockClient
                .Setup(c => c.PatientMessageRecipientsGet(GetMatchingEmisRequestParameters()))
                .Throws<HttpRequestException>()
                .Verifiable();

            // Act
            var getMessageRecipientsResult = await _systemUnderTest.GetMessageRecipients(_userSession);

            // Assert
            _mockClient.Verify();

            getMessageRecipientsResult.Should().BeAssignableTo<GetPatientMessageRecipientsResult.BadGateway>();
        }

        [TestMethod]
        public async Task GetMessageRecipients_WhenExceptionIsThrown_ReturnsInternalServerError()
        {
            // Arrange
            _mockClient
                .Setup(c => c.PatientMessageRecipientsGet(GetMatchingEmisRequestParameters()))
                .Throws<Exception>()
                .Verifiable();

            // Act
            var getMessageRecipientsResult = await _systemUnderTest.GetMessageRecipients(_userSession);

            // Assert
            _mockClient.Verify();

            getMessageRecipientsResult.Should().BeAssignableTo<GetPatientMessageRecipientsResult.InternalServerError>();
        }

        [TestMethod]
        public async Task GetMessageRecipients_WhenUnknownExceptionIsThrown_ReturnsInternalServerError()
        {
            // Arrange
            _mockClient
                .Setup(c => c.PatientMessageRecipientsGet(GetMatchingEmisRequestParameters()))
                .Throws<Exception>()
                .Verifiable();

            // Act
            var getMessageRecipientsResult = await _systemUnderTest.GetMessageRecipients(_userSession);

            // Assert
            _mockClient.Verify();

            getMessageRecipientsResult.Should().BeAssignableTo<GetPatientMessageRecipientsResult.InternalServerError>();
        }

        [TestMethod]
        public async Task GetMessageRecipients_WhenUnknownErrorOccurs_ReturnsBadGateway()
        {
            // Arrange
            var exceptionErrorResponse = _fixture.Create<ExceptionErrorResponse>();

            _mockClient
                .Setup(c => c.PatientMessageRecipientsGet(GetMatchingEmisRequestParameters()))
                .Returns(Task.FromResult(
                    new EmisApiObjectResponse<MessageRecipientsResponse>(
                        HttpStatusCode.InternalServerError,
                        RequestsForSuccessOutcome.PatientMessageRecipientsGet, _sampleSuccessStatusCodes)
                    {
                        ExceptionErrorResponse = exceptionErrorResponse
                    }))
                .Verifiable();

            // Act
            var getMessageRecipientsResult = await _systemUnderTest.GetMessageRecipients(_userSession);

            // Assert
            _mockClient.Verify();

            getMessageRecipientsResult.Should().BeAssignableTo<GetPatientMessageRecipientsResult.BadGateway>();
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