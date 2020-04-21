using System;
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
using NHSOnline.Backend.GpSystems.Suppliers.Tpp;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.PatientPracticeMessaging;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.PatientPracticeMessaging;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp.PatientPracticeMessaging
{
    [TestClass]
    public class TppPatientPracticeCreateMessageTests
    {
        private IFixture _fixture;

        private Mock<ITppClientRequest<(TppUserSession tppUserSession, string recipientIdentifier,
            string messageText), MessageCreateReply>> _messageCreatePost;

        private PatientPracticeMessagingService _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _messageCreatePost =
                _fixture
                    .Freeze<Mock<ITppClientRequest<(TppUserSession tppUserSession, string recipientIdentifier, string
                        messageText), MessageCreateReply>>>();
            _systemUnderTest = _fixture.Create<PatientPracticeMessagingService>();
        }

        [TestMethod]
        public async Task CreateMessage_WhenSuccessfulResponseFromTpp_ReturnsSuccess()
        {
            // Arrange
            var tppUserSession = new TppUserSession
            {
                PatientId = "1234",
                OnlineUserId = "12345",
                OdsCode = "1234"
            };

            var message = new CreatePatientMessage
            {
                MessageBody = "Test message",
                RecipientIdentifier = "1:Recipient",
            };

            var expectedMessageCreateReply = new MessageCreateReply
            {
                PatientId = tppUserSession.PatientId,
                OnlineUserId = tppUserSession.OnlineUserId,
                Uuid = tppUserSession.OnlineUserId
            };

            var parameters = (tppUserSession, message.RecipientIdentifier, message.MessageBody);
            _messageCreatePost
                .Setup(c => c.Post(parameters))
                .Returns(Task.FromResult(
                    new TppApiObjectResponse<MessageCreateReply>(HttpStatusCode.OK)
                    {
                        Body = expectedMessageCreateReply,
                        ErrorResponse = null,
                    }))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.SendMessage(tppUserSession, message);

            //Assert
            result.Should().BeAssignableTo<PostPatientMessageResult.Success>();
        }

        [TestMethod]
        public async Task MessageCreate_WhenHttpExceptionIsThrown_ReturnsBadGateway()
        {
            var tppUserSession = new TppUserSession
            {
                PatientId = "1234",
                OnlineUserId = "12345",
                OdsCode = "1234"
            };

            var message = new CreatePatientMessage
            {
                MessageBody = "Test message",
                RecipientIdentifier = "1:Recipient",
            };
            var parameters = (tppUserSession, message.RecipientIdentifier, message.MessageBody);

            // Arrange
            _messageCreatePost
                .Setup(c => c.Post(parameters))
                .Throws<HttpRequestException>()
                .Verifiable();

            // Act
            var createMessageResult = await _systemUnderTest.SendMessage(tppUserSession, message);

            // Assert
           _messageCreatePost.Verify();

            createMessageResult.Should().BeAssignableTo<PostPatientMessageResult.BadGateway>();
        }

        [TestMethod]
        public async Task MessageCreate_WhenNullReferenceException_ReturnsBadGateway()
        {
            var tppUserSession = new TppUserSession
            {
                PatientId = "1234",
                OnlineUserId = "12345",
                OdsCode = "1234"
            };

            var message = new CreatePatientMessage
            {
                MessageBody = "Test message",
                RecipientIdentifier = "1:Recipient",
            };
            var parameters = (tppUserSession, message.RecipientIdentifier, message.MessageBody);

            // Arrange
            _messageCreatePost
                .Setup(c => c.Post(parameters))
                .Throws<NullReferenceException>()
                .Verifiable();

            // Act
            var createMessageResult = await _systemUnderTest.SendMessage(tppUserSession, message);

            // Assert
            _messageCreatePost.Verify();

            createMessageResult.Should().BeAssignableTo<PostPatientMessageResult.BadGateway>();
        }

        [TestMethod]
        public async Task MessageCreate_WhenNullIsReturned_ReturnsBadGateway()
        {
            var tppUserSession = new TppUserSession
            {
                PatientId = "1234",
                OnlineUserId = "12345",
                OdsCode = "1234"
            };

            var message = new CreatePatientMessage
            {
                MessageBody = "Test message",
                RecipientIdentifier = "1:Recipient",
            };
            var parameters = (tppUserSession, message.RecipientIdentifier, message.MessageBody);

            // Arrange
            _messageCreatePost
                .Setup(c => c.Post(parameters))
                .Returns(Task.FromResult(
                    new TppApiObjectResponse<MessageCreateReply>(HttpStatusCode.OK)
                    {
                        Body = null,
                        ErrorResponse = null,
                    }))
                .Verifiable();

            // Act
            var createMessageResult = await _systemUnderTest.SendMessage(tppUserSession, message);

            // Assert
            _messageCreatePost.Verify();

            createMessageResult.Should().BeAssignableTo<PostPatientMessageResult.BadGateway>();
        }
    }
}