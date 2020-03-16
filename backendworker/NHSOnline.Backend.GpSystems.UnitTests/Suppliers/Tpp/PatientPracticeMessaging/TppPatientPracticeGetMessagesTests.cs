using System;
using System.Collections.Generic;
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
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.PatientPracticeMessaging;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.PatientPracticeMessaging;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp.PatientPracticeMessaging
{
    [TestClass]
    public class TppPatientPracticeGetMessagesTests
    {
        private IFixture _fixture;

        private Mock<ITppClientRequest<TppUserSession, MessagesViewReply>> _messagesViewPost;
        private Mock<ITppPatientMessagesMapper> _mockMessagesMapper;

        private PatientPracticeMessagingService _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _messagesViewPost = _fixture.Freeze<Mock<ITppClientRequest<TppUserSession, MessagesViewReply>>>();
            _mockMessagesMapper = _fixture.Freeze<Mock<ITppPatientMessagesMapper>>();

            _systemUnderTest = _fixture.Create<PatientPracticeMessagingService>();
        }

        [TestMethod]
        public async Task GetMessages_WhenSuccessfulResponseFromTpp_ReturnsSuccess()
        {
            // Arrange
            var getPatientMessagesResponse = _fixture.Create<GetPatientMessagesResponse>();

            var tppUserSession = new TppUserSession
            {
                PatientId = "1234",
                OnlineUserId = "12345",
                OdsCode = "1234"
            };

            var message = new Message
            {
                MessageId = "1",
                Sender = "Test Sender",
                Recipient = "Test Recipient",
                MessageText = "This is the message",
                IncomingString = "y"
            };

            var expectedMessageViewReply = new MessagesViewReply
            {
                Messages = new List<Message> { message }
            };

            _messagesViewPost
                .Setup(c => c.Post(tppUserSession))
                .Returns(Task.FromResult(
                    new TppApiObjectResponse<MessagesViewReply>(HttpStatusCode.OK)
                    {
                        Body = expectedMessageViewReply,
                        ErrorResponse = null,
                    }))
                .Verifiable();

            _mockMessagesMapper
                .Setup(e => e.Map(
                    It.Is<MessagesViewReply>(m => m.Equals(expectedMessageViewReply))))
                .Returns(getPatientMessagesResponse)
                .Verifiable();

            // Act
            var result = await _systemUnderTest.GetMessages(tppUserSession);

            //Assert
            result.Should().BeAssignableTo<GetPatientMessagesResult.Success>()
                .Subject.Response.Should().NotBeNull();
        }

        [TestMethod]
        public async Task GetMessages_WhenNoAccessFromTpp_ReturnsForbidden()
        {
            // Arrange
            var tppUserSession = new TppUserSession
            {
                PatientId = "1234",
                OnlineUserId = "12345",
                OdsCode = "1234"
            };

            var noAccessError = new Error
            {
                ErrorCode = TppApiErrorCodes.NoAccess,
                UserFriendlyMessage = "No patient access to im1 messaging"
            };

            _messagesViewPost
                .Setup(c => c.Post(tppUserSession))
                .Returns(Task.FromResult(
                    new TppApiObjectResponse<MessagesViewReply>(HttpStatusCode.OK)
                    {
                        Body = null,
                        ErrorResponse = noAccessError,
                    }))
                .Verifiable();

            // Act
            var getMessagesResult = await _systemUnderTest.GetMessages(tppUserSession);

            // Assert
            _messagesViewPost.Verify();

            getMessagesResult.Should().BeAssignableTo<GetPatientMessagesResult.Forbidden>();
        }

        [TestMethod]
        public async Task GetMessages_WhenHttpExceptionIsThrown_ReturnsBadGateway()
        {
            var tppUserSession = new TppUserSession
            {
                PatientId = "1234",
                OnlineUserId = "12345",
                OdsCode = "1234"
            };
            // Arrange
            _messagesViewPost
                .Setup(c => c.Post(tppUserSession))
                .Throws<HttpRequestException>()
                .Verifiable();

            // Act
            var getMessagesResult = await _systemUnderTest.GetMessages(tppUserSession);

            // Assert
            _messagesViewPost.Verify();

            getMessagesResult.Should().BeAssignableTo<GetPatientMessagesResult.BadGateway>();
        }

        [TestMethod]
        public async Task GetMessages_WhenExceptionIsThrown_ReturnsInternalServerError()
        {

            var tppUserSession = new TppUserSession
            {
                PatientId = "1234",
                OnlineUserId = "12345",
                OdsCode = "1234"
            };
            // Arrange
            _messagesViewPost
                .Setup(c => c.Post(tppUserSession))
                .Throws<Exception>()
                .Verifiable();

            // Act
            var getMessagesResult = await _systemUnderTest.GetMessages(tppUserSession);

            // Assert
            _messagesViewPost.Verify();

            getMessagesResult.Should().BeAssignableTo<GetPatientMessagesResult.InternalServerError>();
        }

        [TestMethod]
        public async Task GetMessages_WhenMapperReturnsNull_ReturnsBadGateway()
        {
            // Arrange
            var messagesViewReply = new MessagesViewReply
            {
                Messages = null
            };

            var tppUserSession = new TppUserSession
            {
                PatientId = "1234",
                OnlineUserId = "12345",
                OdsCode = "1234"
            };
            _messagesViewPost
                .Setup(c => c.Post(tppUserSession))
                .Returns(Task.FromResult(
                    new TppApiObjectResponse<MessagesViewReply>(HttpStatusCode.OK)
                    {
                        Body = messagesViewReply,
                        ErrorResponse = null,
                    }))
                .Verifiable();

            _mockMessagesMapper
                .Setup(e => e.Map(
                    It.Is<MessagesViewReply>(m => m.Equals(messagesViewReply))))
                .Returns((GetPatientMessagesResponse) null)
                .Verifiable();

            // Act
            var messagesResult = await _systemUnderTest.GetMessages(tppUserSession);

            // Assert
            _messagesViewPost.Verify();
            _mockMessagesMapper.Verify();

            messagesResult.Should().BeAssignableTo<GetPatientMessagesResult.BadGateway>();
        }
    }
}