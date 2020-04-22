using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
using NHSOnline.Backend.Support;

using TppMessageDetails = NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.PatientPracticeMessaging.MessageDetails;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp.PatientPracticeMessaging
{
    [TestClass]
    public class TppPatientMessagesServiceTests
    {
        private IFixture _fixture;

        private GpUserSession _gpUserSession;

        private Mock<ITppClientRequest<TppUserSession, MessagesViewReply>> _viewMessageRequest;
        private Mock<ITppClientRequest<TppUserSession, MessageRecipientsReply>> _messageRecipients;
        private Mock<ITppClientRequest<(TppRequestParameters, List<string>), MessagesMarkAsReadReply>> _markMessageAsReadRequest;

        private Mock<IGetPatientPracticeMessagingRecipientsTaskChecker> _messageRecipientsTaskChecker;
        private Mock<ITppPatientMessagesUnreadIdsMapper> _messagesUnreadIdsMapper;

        private List<TppMessageDetails> _messages;
        private TppApiObjectResponse<MessagesViewReply> _messagesResponse;

        private List<string> _messagesUnreadIdsMapperMessageIds;

        private MessagesMarkAsReadReply _markAsReadReply;
        private TppApiObjectResponse<MessagesMarkAsReadReply> _markAsReadResponse;

        private TppPatientMessagesService _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _gpUserSession = _fixture.Create<TppUserSession>();
            _viewMessageRequest = _fixture.Freeze<Mock<ITppClientRequest<TppUserSession, MessagesViewReply>>>();
            _messageRecipients = _fixture.Freeze<Mock<ITppClientRequest<TppUserSession, MessageRecipientsReply>>>();
            _markMessageAsReadRequest = _fixture.Freeze<Mock<ITppClientRequest<(TppRequestParameters, List<string>), MessagesMarkAsReadReply>>>();
            _messageRecipientsTaskChecker = _fixture.Freeze<Mock<IGetPatientPracticeMessagingRecipientsTaskChecker>>();
            _messagesUnreadIdsMapper = _fixture.Freeze<Mock<ITppPatientMessagesUnreadIdsMapper>>();

            _messages = new List<TppMessageDetails>
            {
                new TppMessageDetails { MessageId = "1" },
                new TppMessageDetails { MessageId = "2" },
                new TppMessageDetails { MessageId = "3" }
            };
            _messagesResponse = new TppApiObjectResponse<MessagesViewReply>(HttpStatusCode.OK)
            {
                Body = new MessagesViewReply
                {
                    Messages = _messages
                }
            };

            _viewMessageRequest.Setup(r => r.Post(It.IsAny<TppUserSession>()))
                .Returns(() => Task.FromResult(_messagesResponse));

            _messagesUnreadIdsMapperMessageIds = new List<string> { "1", "2", "3" };
            _messagesUnreadIdsMapper.Setup(m => m.Map(It.IsAny<List<TppMessageDetails>>(), It.IsAny<string>()))
                .Returns(() => _messagesUnreadIdsMapperMessageIds);

            _markAsReadReply = new MessagesMarkAsReadReply();
            _markAsReadResponse = new TppApiObjectResponse<MessagesMarkAsReadReply>(HttpStatusCode.OK)
            {
                Body = _markAsReadReply
            };

            _markMessageAsReadRequest.Setup(r => r.Post(It.IsAny<(TppRequestParameters, List<string>)>()))
                .Returns(() => Task.FromResult(_markAsReadResponse));

            _systemUnderTest = _fixture.Create<TppPatientMessagesService>();
        }

        [TestMethod]
        public async Task GetMessageRecipients_ReturnsSuccessResponseForHappyPath_WhenSuccessfulResponseFromTpp()
        {
            // Arrange
            var messageRecipientsResponse = new MessageRecipientsReply
            {
                Items = new List<Item>
                {
                    new Item
                    {
                        ItemText = "Mr Dr",
                        Id = "12345"
                    },
                    new Item
                    {
                        ItemText = "Mrs Dr",
                        Id = "123456"
                    }
                }
            };

            _messageRecipients.Setup(x => x.Post(It.IsAny<TppUserSession>()))
                .Returns(Task.FromResult(
                    new TppApiObjectResponse<MessageRecipientsReply>(HttpStatusCode.OK)
                    {
                        Body = messageRecipientsResponse,
                        ErrorResponse = null,
                    }));

            _messageRecipientsTaskChecker.Setup(x =>
                    x.Check(It.IsAny<TppApiObjectResponse<MessageRecipientsReply>>()))
                .Returns(new PatientPracticeMessageRecipients
                {
                    MessageRecipients = new List<MessageRecipient>
                    {
                        new MessageRecipient
                        {
                            Name = "Mr Dr",
                            RecipientIdentifier = "12345"
                        },
                        new MessageRecipient
                        {
                            Name = "Mrs Dr",
                            RecipientIdentifier = "123456"
                        }
                    },
                    HasErrored = false
                })
                .Verifiable();

            // Act
            var result = await _systemUnderTest.GetMessageRecipients(_gpUserSession);

            // Assert
            _messageRecipients.Verify(x => x.Post(It.IsAny<TppUserSession>()));

            result.Should().BeAssignableTo<GetPatientMessageRecipientsResult.Success>()
                .Subject.Response.MessageRecipients.Count.Should().Be(2);
        }

        [TestMethod]
        public async Task GetMessageRecipients_ReturnsSuccessResponseForZeroRecipients_WhenSuccessfulResponseFromTpp()
        {
            // Arrange
            var messageRecipientsResponse = new MessageRecipientsReply
            {
                Items = new List<Item> ()
            };

            _messageRecipients.Setup(x => x.Post(It.IsAny<TppUserSession>()))
                .Returns(Task.FromResult(
                    new TppApiObjectResponse<MessageRecipientsReply>(HttpStatusCode.OK)
                    {
                        Body = messageRecipientsResponse,
                        ErrorResponse = null,
                    }));

            _messageRecipientsTaskChecker.Setup(x =>
                    x.Check(It.IsAny<TppApiObjectResponse<MessageRecipientsReply>>()))
                .Returns(new PatientPracticeMessageRecipients
                {
                    MessageRecipients = new List<MessageRecipient>(),
                    HasErrored = false
                })
                .Verifiable();

            // Act
            var result = await _systemUnderTest.GetMessageRecipients(_gpUserSession);

            // Assert
            _messageRecipients.Verify(x => x.Post(It.IsAny<TppUserSession>()));

            result.Should().BeAssignableTo<GetPatientMessageRecipientsResult.Success>()
                .Subject.Response.MessageRecipients.Count.Should().Be(0);
        }

        [TestMethod]
        [DataRow(null, DisplayName = "null")]
        [DataRow("", DisplayName = "empty")]
        [DataRow("\t\r   \r\n", DisplayName = "blank")]
        public async Task UpdateMessageMessageReadStatus_WhenMessageIdIsNullOrEmpty_ThenArgumentExceptionIsThrown(string value)
        {
            await _systemUnderTest.Awaiting(s =>
                    s.UpdateMessageMessageReadStatus(_gpUserSession,
                        new UpdateMessageReadStatusRequestBody { MessageId = value }))
                .Should()
                .ThrowAsync<ArgumentException>();
        }

        [TestMethod]
        public async Task UpdateMessageMessageReadStatus_WhenMessageIdValid_ThenPatientMessagesAreRetrieved()
        {
            await _systemUnderTest.UpdateMessageMessageReadStatus(_gpUserSession,
                new UpdateMessageReadStatusRequestBody { MessageId = "1" });

            _viewMessageRequest.Verify(r => r.Post(It.IsAny<TppUserSession>()));
        }

        [TestMethod]
        public async Task UpdateMessageMessageReadStatus_WhenMessageIdValid_ThenIdsMapperIsCalled()
        {
            await _systemUnderTest.UpdateMessageMessageReadStatus(_gpUserSession,
                new UpdateMessageReadStatusRequestBody { MessageId = "1" });

            _messagesUnreadIdsMapper.Verify(m =>
                m.Map(It.Is<List<TppMessageDetails>>(l => l == _messages),
                    It.Is<string>(s => s == "1")));
        }

        [TestMethod]
        public async Task UpdateMessageMessageReadStatus_WhenMessageIdValid_ThenMarkAsReadRequestIsSent()
        {
            await _systemUnderTest.UpdateMessageMessageReadStatus(_gpUserSession,
                new UpdateMessageReadStatusRequestBody { MessageId = "1" });

            _markMessageAsReadRequest.Verify(r =>
                r.Post(It.Is<(TppRequestParameters, List<string>)>(p =>
                        p.Item2.Intersect(_messagesUnreadIdsMapperMessageIds).Count() == 3)));
        }

        [TestMethod]
        public async Task UpdateMessageMessageReadStatus_WhenMessageIdValidAndRequestsSucceed_ThenSuccessResultIsReturned()
        {
            var result = await _systemUnderTest.UpdateMessageMessageReadStatus(_gpUserSession,
                new UpdateMessageReadStatusRequestBody { MessageId = "1" });

            result.Should().BeOfType<PutPatientMessageReadStatusResult.Success>();
        }

        [TestMethod]
        public async Task UpdateMessageMessageReadStatus_WhenMessageIdValidAndGetMessagesRequestFails_ThenBadGatewayResultIsReturned()
        {
            _messagesResponse.ErrorResponse = new Error
            {
                ErrorCode = "Help Me I am Stuck in a TPP Factory"
            };

            var result = await _systemUnderTest.UpdateMessageMessageReadStatus(_gpUserSession,
                new UpdateMessageReadStatusRequestBody { MessageId = "1" });

            result.Should().BeOfType<PutPatientMessageReadStatusResult.BadGateway>();
        }

        [TestMethod]
        public async Task UpdateMessageMessageReadStatus_WhenMessageIdValidAndMessageAccessIsDenied_ThenForbiddenResultIsReturned()
        {
            _messagesResponse.ErrorResponse = new Error
            {
                ErrorCode = TppApiErrorCodes.NoAccess
            };

            var result = await _systemUnderTest.UpdateMessageMessageReadStatus(_gpUserSession,
                new UpdateMessageReadStatusRequestBody { MessageId = "1" });

            result.Should().BeOfType<PutPatientMessageReadStatusResult.Forbidden>();
        }

        [TestMethod]
        public async Task UpdateMessageMessageReadStatus_WhenMessageIdValidAndMarkAsReadRequestFails_ThenBadGatewayResultIsReturned()
        {
            _markAsReadResponse.ErrorResponse = new Error
            {
                ErrorCode = "Help Me I am Stuck in a TPP Factory"
            };

            var result = await _systemUnderTest.UpdateMessageMessageReadStatus(_gpUserSession,
                new UpdateMessageReadStatusRequestBody { MessageId = "1" });

            result.Should().BeOfType<PutPatientMessageReadStatusResult.BadGateway>();
        }

        [TestMethod]
        public async Task UpdateMessageMessageReadStatus_WhenMessageIdValidAndMarkAsReadRequestFailsWIthForbiddenError_ThenForbiddenResultIsReturned()
        {
            _markAsReadResponse.ErrorResponse = new Error
            {
                ErrorCode = TppApiErrorCodes.NoAccess
            };

            var result = await _systemUnderTest.UpdateMessageMessageReadStatus(_gpUserSession,
                new UpdateMessageReadStatusRequestBody { MessageId = "1" });

            result.Should().BeOfType<PutPatientMessageReadStatusResult.Forbidden>();
        }

    }
}
