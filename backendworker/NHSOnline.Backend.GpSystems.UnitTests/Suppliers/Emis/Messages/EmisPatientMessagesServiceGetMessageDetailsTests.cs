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
    public class EmisPatientMessagesServiceGetMessageDetailsTests
    {
        private IFixture _fixture;

        private Mock<IEmisClient> _mockClient;
        private Mock<IEmisPatientMessageMapper> _mockMessageDetailsMapper;

        private EmisUserSession _userSession;
        private List<HttpStatusCode> _sampleSuccessStatusCodes;

        private EmisPatientMessagesService _systemUnderTest;

        private const string MessageDetailId = "1";

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _mockClient = _fixture.Freeze<Mock<IEmisClient>>();
            _mockMessageDetailsMapper = _fixture.Freeze<Mock<IEmisPatientMessageMapper>>();

            _userSession = _fixture.Create<EmisUserSession>();

            _systemUnderTest = _fixture.Create<EmisPatientMessagesService>();

            _sampleSuccessStatusCodes = new List<HttpStatusCode>
            {
                HttpStatusCode.OK
            };
        }

        [TestMethod]
        public async Task GetMessageDetails_WhenSuccessfulResponseFromEmis_ReturnsSuccess()
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
                .Setup(c => c.PatientMessageDetailsGet(MessageDetailId, GetMatchingEmisRequestParameters()))
                .Returns(Task.FromResult(new EmisClient.EmisApiObjectResponse<MessageGetResponse>(HttpStatusCode.OK,
                    RequestsForSuccessOutcome.PatientMessageDetailsGet, _sampleSuccessStatusCodes)
                {
                    Body = messageGetResponse
                }))
                .Verifiable();
            _mockMessageDetailsMapper
                .Setup(e => e.Map(It.Is<MessageGetResponse>(m => m.Equals(messageGetResponse))))
                .Returns(getPatientMessageResponse)
                .Verifiable();

            // Act
            var result = await _systemUnderTest.GetMessageDetails(MessageDetailId, _userSession);

            // Assert
            _mockClient.Verify();
            _mockMessageDetailsMapper.Verify();

            result.Should().BeAssignableTo<GetPatientMessageResult.Success>()
                .Subject.Response.Should().NotBeNull();
        }

        [TestMethod]
        public async Task GetMessageDetails_WhenBadRequestFromEmis_ReturnsBadRequest()
        {
            // Arrange
            var badRequestErrorResponse = _fixture.Create<BadRequestErrorResponse>();
            
            _mockClient
                .Setup(c => c.PatientMessageDetailsGet(MessageDetailId, GetMatchingEmisRequestParameters()))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<MessageGetResponse>(HttpStatusCode.BadRequest,
                        RequestsForSuccessOutcome.PatientMessageDetailsGet, _sampleSuccessStatusCodes)
                    {
                        ErrorResponseBadRequest = badRequestErrorResponse
                    }))
                .Verifiable();
            
            // Act
            var getMessageDetails = await _systemUnderTest.GetMessageDetails(MessageDetailId, _userSession);
            
            // Assert
            _mockClient.Verify();
            
            getMessageDetails.Should().BeAssignableTo<GetPatientMessageResult.BadRequest>();
        }

        [TestMethod]
        public async Task GetMessageDetails_WhenForbiddenResponseFromEmis_ReturnsForbidden()
        {
            // Arrange
            var exceptionErrorResponse = _fixture.Create<ExceptionErrorResponse>();
            exceptionErrorResponse.Exceptions.First().Message = EmisApiErrorMessages.EmisService_NotEnabledForUser;

            _mockClient
                .Setup(c => c.PatientMessageDetailsGet(MessageDetailId, GetMatchingEmisRequestParameters()))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<MessageGetResponse>(HttpStatusCode.Forbidden,
                        RequestsForSuccessOutcome.PatientMessageDetailsGet, _sampleSuccessStatusCodes)
                    {
                        ExceptionErrorResponse = exceptionErrorResponse
                    }))
                .Verifiable();

            // Act
            var getMessageDetails = await _systemUnderTest.GetMessageDetails(MessageDetailId, _userSession);

            // Assert
            _mockClient.Verify();

            getMessageDetails.Should().BeAssignableTo<GetPatientMessageResult.Forbidden>();
        }
        
        [TestMethod]
        public async Task GetMessageDetails_WhenHttpExceptionIsThrown_ReturnsBadGateway()
        {
            // Arrange
            _mockClient
                .Setup(c => c.PatientMessageDetailsGet(MessageDetailId, GetMatchingEmisRequestParameters()))
                .Throws<HttpRequestException>()
                .Verifiable();

            // Act
            var getMessageDetails = await _systemUnderTest.GetMessageDetails(MessageDetailId, _userSession);

            // Assert
            _mockClient.Verify();
            
            getMessageDetails.Should().BeAssignableTo<GetPatientMessageResult.BadGateway>();
        }
        
        [TestMethod]
        public async Task GetMessageDetails_WhenExceptionIsThrown_ReturnsInternalServerError()
        {
            // Arrange
            _mockClient
                .Setup(c => c.PatientMessageDetailsGet(MessageDetailId, GetMatchingEmisRequestParameters()))
                .Throws<Exception>()
                .Verifiable();

            // Act
            var getMessageDetails = await _systemUnderTest.GetMessageDetails(MessageDetailId, _userSession);

            // Assert
            _mockClient.Verify();
            
            getMessageDetails.Should().BeAssignableTo<GetPatientMessageResult.InternalServerError>();
        }

        [TestMethod]
        public async Task GetMessageDetails_WhenMapperReturnsNull_ReturnsBadGateway()
        {
            // Arrange
            var messageDetailsGetResponse = _fixture.Create<MessageGetResponse>();

            _mockClient
                .Setup(c => c.PatientMessageDetailsGet(MessageDetailId, GetMatchingEmisRequestParameters()))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<MessageGetResponse>(HttpStatusCode.OK,
                        RequestsForSuccessOutcome.PatientMessagesGet, _sampleSuccessStatusCodes)
                    {
                        Body = messageDetailsGetResponse
                    }))
                .Verifiable();

            _mockMessageDetailsMapper.
                Setup(e => e.Map(
                    It.Is<MessageGetResponse>(m => m.Equals(messageDetailsGetResponse))))
                .Returns((GetPatientMessageResponse)null)
                .Verifiable();

            // Act
            var messageDetailsResult = await _systemUnderTest.GetMessageDetails(MessageDetailId, _userSession);

            // Assert
            _mockClient.Verify();
            _mockMessageDetailsMapper.Verify();
            
            messageDetailsResult.Should().BeAssignableTo<GetPatientMessageResult.BadGateway>();
        }

        [TestMethod]
        public async Task GetMessageDetails_WhenUnknownExceptionIsThrown_ReturnsInternalServerError()
        {
            // Arrange
            _mockClient
                .Setup(c => c.PatientMessageDetailsGet(MessageDetailId, GetMatchingEmisRequestParameters()))
                .Throws<Exception>()
                .Verifiable();

            // Act
            var getMessageDetails = await _systemUnderTest.GetMessageDetails(MessageDetailId, _userSession);

            // Assert
            _mockClient.Verify();
            
            getMessageDetails.Should().BeAssignableTo<GetPatientMessageResult.InternalServerError>();
        }

        [TestMethod]
        public async Task GetMessageDetails_WhenUnknownErrorOccurs_ReturnsBadGateway()
        {
            // Arrange
            var exceptionErrorResponse = _fixture.Create<ExceptionErrorResponse>();

            _mockClient
                .Setup(c => c.PatientMessageDetailsGet(MessageDetailId, GetMatchingEmisRequestParameters()))
                .Returns(Task.FromResult(
                    new EmisClient.EmisApiObjectResponse<MessageGetResponse>(HttpStatusCode.InternalServerError,
                        RequestsForSuccessOutcome.PatientMessageRecipientsGet, _sampleSuccessStatusCodes)
                    {
                        ExceptionErrorResponse = exceptionErrorResponse
                    }))
                .Verifiable();

            // Act
            var getMessageDetails = await _systemUnderTest.GetMessageDetails(MessageDetailId, _userSession);

            // Assert
            _mockClient.Verify();

            getMessageDetails.Should().BeAssignableTo<GetPatientMessageResult.BadGateway>();
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