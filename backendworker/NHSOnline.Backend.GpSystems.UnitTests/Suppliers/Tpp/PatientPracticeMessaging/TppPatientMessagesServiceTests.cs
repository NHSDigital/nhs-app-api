using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Messages;
using NHSOnline.Backend.GpSystems.Messages.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.PatientPracticeMessaging;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.PatientPracticeMessaging;
using UnitTestHelper;
using TppMessageDetails = NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.PatientPracticeMessaging.MessageDetails;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp.PatientPracticeMessaging
{
    [TestClass]
    public class TppPatientMessagesServiceTests
    {
        private Mock<TppUserSession> _gpUserSession;
        private Mock<ILogger<TppPatientMessagesService>> _mockLogger;
        private Mock<ITppClientRequest<TppUserSession, MessagesViewReply>> _mockViewMessageRequest;
        private Mock<ITppClientRequest<TppUserSession, MessageRecipientsReply>> _mockListRecipientsRequest;
        private Mock<ITppClientRequest<(TppRequestParameters, List<string>), MessagesMarkAsReadReply>> _mockMarkMessageAsReadRequest;
        private Mock<ITppClientRequest<(TppUserSession tppUserSession, string recipientIdentifier, string messageText),
            MessageCreateReply>> _mockMessagesCreateMessagePost;
        private Mock<IGetPatientPracticeMessagingRecipientsTaskChecker> _mockMessageRecipientsTaskChecker;
        private Mock<ITppPatientMessagesUnreadIdsMapper> _mockMessagesUnreadIdsMapper;
        private Mock<ITppPatientMessagesMapper> _mockMessagesViewMapper;

        private TppApiObjectResponse<MessagesViewReply> _messagesResponse;
        private List<string> _messagesUnreadIdsMapperMessageIds;
        private TppApiObjectResponse<MessagesMarkAsReadReply> _markAsReadResponse;

        private TppPatientMessagesService _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _gpUserSession = new Mock<TppUserSession>();
            _mockLogger = new Mock<ILogger<TppPatientMessagesService>>();
            _mockViewMessageRequest = new Mock<ITppClientRequest<TppUserSession, MessagesViewReply>>();
            _mockListRecipientsRequest = new Mock<ITppClientRequest<TppUserSession, MessageRecipientsReply>>();
            _mockMarkMessageAsReadRequest = new Mock<ITppClientRequest<(TppRequestParameters, List<string>), MessagesMarkAsReadReply>>();
            _mockMessageRecipientsTaskChecker = new Mock<IGetPatientPracticeMessagingRecipientsTaskChecker>();
            _mockMessagesUnreadIdsMapper = new Mock<ITppPatientMessagesUnreadIdsMapper>();
            _mockMessagesCreateMessagePost = new Mock<ITppClientRequest<(TppUserSession tppUserSession, string recipientIdentifier, string messageText),
                MessageCreateReply>>();
            _mockMessagesViewMapper = new Mock<ITppPatientMessagesMapper>();

            _messagesResponse = new TppApiObjectResponse<MessagesViewReply>(HttpStatusCode.OK)
            {
                Body = new MessagesViewReply
                {
                    Messages = new List<TppMessageDetails>
                    {
                        new TppMessageDetails { MessageId = "1" },
                        new TppMessageDetails { MessageId = "2" },
                        new TppMessageDetails { MessageId = "3" }
                    }
                }
            };

            _mockViewMessageRequest.Setup(r=> r.Post(_gpUserSession.Object))
                .Returns(() => Task.FromResult(_messagesResponse));

            _messagesUnreadIdsMapperMessageIds = new List<string> { "1", "2", "3" };
            _mockMessagesUnreadIdsMapper.Setup(m=> m.Map(It.IsAny<List<TppMessageDetails>>(), It.IsAny<string>()))
                .Returns(() => _messagesUnreadIdsMapperMessageIds);

            _markAsReadResponse = new TppApiObjectResponse<MessagesMarkAsReadReply>(HttpStatusCode.OK)
            {
                Body = new MessagesMarkAsReadReply()
            };

            _mockMarkMessageAsReadRequest.Setup(r => r.Post(It.IsAny<(TppRequestParameters, List<string>)>()))
                .Returns(() => Task.FromResult(_markAsReadResponse));

            _systemUnderTest = new TppPatientMessagesService(
                _mockLogger.Object,
                _mockListRecipientsRequest.Object,
                _mockMessageRecipientsTaskChecker.Object,
                _mockMessagesViewMapper.Object,
                _mockViewMessageRequest.Object,
                _mockMarkMessageAsReadRequest.Object,
                _mockMessagesUnreadIdsMapper.Object,
                _mockMessagesCreateMessagePost.Object);
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

            _mockListRecipientsRequest.Setup(x => x.Post(_gpUserSession.Object))
                .Returns(Task.FromResult(
                    new TppApiObjectResponse<MessageRecipientsReply>(HttpStatusCode.OK)
                    {
                        Body = messageRecipientsResponse,
                        ErrorResponse = null,
                    }))
                .Verifiable();

            _mockMessageRecipientsTaskChecker.Setup(x =>
                    x.Check(It.IsAny<TppApiObjectResponse<MessageRecipientsReply>>()))
                .Returns(new PatientPracticeMessageRecipients
                {
                    MessageRecipients = new List<MessageRecipient>
                    {
                        new MessageRecipient
                        {
                            Name = "Mr Dr",
                            RecipientIdentifier = "12345:Recipient"
                        },
                        new MessageRecipient
                        {
                            Name = "Mrs Dr",
                            RecipientIdentifier = "123456:UnitRecipient"
                        }
                    },
                    HasErrored = false
                })
                .Verifiable();

            // Act
            var result = await _systemUnderTest.GetMessageRecipients(_gpUserSession.Object);

            // Assert
            _mockListRecipientsRequest.Verify();
            _mockMessageRecipientsTaskChecker.Verify();

            result.Should().BeAssignableTo<GetPatientMessageRecipientsResult.Success>()
                .Subject.Response.MessageRecipients.Count.Should().Be(2);
        }

        [TestMethod]
        public async Task GetMessageRecipients_FiltersOutInvalidRecipientIdentifiers_WhenSuccessfulResponseFromTpp()
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

            _mockListRecipientsRequest.Setup(x => x.Post(_gpUserSession.Object))
                .Returns(Task.FromResult(
                    new TppApiObjectResponse<MessageRecipientsReply>(HttpStatusCode.OK)
                    {
                        Body = messageRecipientsResponse,
                        ErrorResponse = null,
                    }))
                .Verifiable();

            _mockMessageRecipientsTaskChecker.Setup(x =>
                    x.Check(It.IsAny<TppApiObjectResponse<MessageRecipientsReply>>()))
                .Returns(new PatientPracticeMessageRecipients
                {
                    MessageRecipients = new List<MessageRecipient>
                    {
                        new MessageRecipient
                        {
                            Name = "Mr Dr",
                            RecipientIdentifier = "12345:Recipient"
                        },
                        new MessageRecipient
                        {
                            Name = "Mrs Dr",
                            RecipientIdentifier = "InvalidIdentifier"
                        }
                    },
                    HasErrored = false
                })
                .Verifiable();

            // Act
            var result = await _systemUnderTest.GetMessageRecipients(_gpUserSession.Object);

            // Assert
            _mockListRecipientsRequest.Verify();
            _mockMessageRecipientsTaskChecker.Verify();

            var results = result.Should().BeAssignableTo<GetPatientMessageRecipientsResult.Success>();

            results.Subject.Response.MessageRecipients.Count.Should().Be(1);
            results.Subject.Response.MessageRecipients[0].Name.Should().Be("Mr Dr");
            results.Subject.Response.MessageRecipients[0].RecipientIdentifier.Should().Be("12345:Recipient");

            _mockLogger.VerifyLogger(
                LogLevel.Warning,
                $"Retrieved a MessageRecipient from ODSCode {_gpUserSession.Object.OdsCode} " +
                $"with an invalid RecipientIdentifier InvalidIdentifier - removing from list of valid message recipients",
                Times.Once());
        }

        [TestMethod]
        public async Task GetMessageRecipients_ReturnsSuccessResponseForZeroRecipients_WhenSuccessfulResponseFromTpp()
        {
            // Arrange
            var messageRecipientsResponse = new MessageRecipientsReply
            {
                Items = new List<Item> ()
            };

            _mockListRecipientsRequest.Setup(x => x.Post(_gpUserSession.Object))
                .Returns(Task.FromResult(
                    new TppApiObjectResponse<MessageRecipientsReply>(HttpStatusCode.OK)
                    {
                        Body = messageRecipientsResponse,
                        ErrorResponse = null,
                    }))
                .Verifiable();

            _mockMessageRecipientsTaskChecker.Setup(x =>
                    x.Check(It.IsAny<TppApiObjectResponse<MessageRecipientsReply>>()))
                .Returns(new PatientPracticeMessageRecipients
                {
                    MessageRecipients = new List<MessageRecipient>(),
                    HasErrored = false
                })
                .Verifiable();

            // Act
            var result = await _systemUnderTest.GetMessageRecipients(_gpUserSession.Object);

            // Assert
            _mockListRecipientsRequest.Verify();
            _mockMessageRecipientsTaskChecker.Verify();

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
                    s.UpdateMessageMessageReadStatus(_gpUserSession.Object,
                        new UpdateMessageReadStatusRequestBody { MessageId = value }))
                .Should()
                .ThrowAsync<ArgumentException>();
        }

        [TestMethod]
        public async Task UpdateMessageMessageReadStatus_WhenMessageIdValid_ThenPatientMessagesAreRetrieved()
        {
            await _systemUnderTest.UpdateMessageMessageReadStatus(_gpUserSession.Object,
                new UpdateMessageReadStatusRequestBody { MessageId = "1" });

            _mockViewMessageRequest.Verify(r => r.Post(_gpUserSession.Object));
        }

        [TestMethod]
        public async Task UpdateMessageMessageReadStatus_WhenMessageIdValid_ThenIdsMapperIsCalled()
        {
            await _systemUnderTest.UpdateMessageMessageReadStatus(_gpUserSession.Object,
                new UpdateMessageReadStatusRequestBody { MessageId = "1" });

            _mockMessagesUnreadIdsMapper.Verify(m =>
                m.Map(It.Is<List<TppMessageDetails>>(l => l == _messagesResponse.Body.Messages),
                    It.Is<string>(s => s == "1")));
        }

        [TestMethod]
        public async Task UpdateMessageMessageReadStatus_WhenMessageIdValid_ThenMarkAsReadRequestIsSent()
        {
            await _systemUnderTest.UpdateMessageMessageReadStatus(_gpUserSession.Object,
                new UpdateMessageReadStatusRequestBody { MessageId = "1" });

            _mockMarkMessageAsReadRequest.Verify(r =>
                r.Post(It.Is<(TppRequestParameters, List<string>)>(p =>
                        p.Item2.Intersect(_messagesUnreadIdsMapperMessageIds).Count() == 3)));
        }

        [TestMethod]
        public async Task UpdateMessageMessageReadStatus_WhenMessageIdValidAndRequestsSucceed_ThenSuccessResultIsReturned()
        {
            var result = await _systemUnderTest.UpdateMessageMessageReadStatus(_gpUserSession.Object,
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

            var result = await _systemUnderTest.UpdateMessageMessageReadStatus(_gpUserSession.Object,
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

            var result = await _systemUnderTest.UpdateMessageMessageReadStatus(_gpUserSession.Object,
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

            var result = await _systemUnderTest.UpdateMessageMessageReadStatus(_gpUserSession.Object,
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

            var result = await _systemUnderTest.UpdateMessageMessageReadStatus(_gpUserSession.Object,
                new UpdateMessageReadStatusRequestBody { MessageId = "1" });

            result.Should().BeOfType<PutPatientMessageReadStatusResult.Forbidden>();
        }
    }
}
