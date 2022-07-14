using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Messages.Areas.Messages;
using NHSOnline.Backend.Messages.Areas.Messages.Models;
using NHSOnline.Backend.Messages.Repository;
using NHSOnline.Backend.Repository;

namespace NHSOnline.Backend.Messages.UnitTests.Repository
{
    [TestClass]
    public class MessageRepositoryTests
    {
        private Mock<IRepository<UserMessage>> _mockRepository;
        private Mock<ICanonicalSenderNameService> _mockCanonicalSenderNameService;

        private MessageRepository _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockRepository = new Mock<IRepository<UserMessage>>(MockBehavior.Strict);
            _mockCanonicalSenderNameService = new Mock<ICanonicalSenderNameService>(MockBehavior.Strict);

            _systemUnderTest = new MessageRepository(
                new Mock<ILogger<MessageRepository>>().Object,
                _mockRepository.Object,
                _mockCanonicalSenderNameService.Object);
        }

        [TestMethod]
        public void Create_WhenUserMessageIsNull_ThrowsException()
        {
            // Act
            Func<Task> act = async () => await _systemUnderTest.Create(null);

            // Assert
            act.Should().ThrowAsync<ArgumentNullException>()
                .WithParameterName("userMessage");

            VerifyAll();
        }

        [TestMethod]
        public async Task Create_WithUserMessage_AddsToCollection()
        {
            // Arrange
            var userMessage = new UserMessage();

            _mockRepository.Setup(x => x.Create(userMessage, It.IsAny<string>()))
                .ReturnsAsync(new RepositoryCreateResult<UserMessage>.Created(userMessage));

            // Act
            var result = await _systemUnderTest.Create(userMessage);

            // Assert
            result.Should().BeOfType<RepositoryCreateResult<UserMessage>.Created>();

            VerifyAll();
        }

        [TestMethod]
        [DataRow(null, "test", "nhsLoginId")]
        [DataRow("test", null, "sender")]
        public void FindMessagesFromSenderByName_WhenArgumentsAreNotValid_ThrowsException(string nhsLoginId, string sender, string paramName)
        {
            // Act
            Func<Task> act = async () => await _systemUnderTest.FindMessagesFromSenderByName(nhsLoginId, sender);

            // Assert
            act.Should().ThrowAsync<ArgumentNullException>()
                .WithParameterName(paramName);

            VerifyAll();
        }

        [TestMethod]
        public async Task FindMessagesFromSenderByName_WhenCanonicalSenderNameServiceThrowsException_ThrowsException()
        {
            // Arrange
            var messages = new List<UserMessage>
            {
                new UserMessage(), new UserMessage()
            };

            _mockRepository.Setup(x =>
                    x.Find( It.IsAny<Expression<Func<UserMessage, bool>>>(), It.IsAny<string>(), null))
                .ReturnsAsync(new RepositoryFindResult<UserMessage>.Found(messages));

            _mockCanonicalSenderNameService.Setup(x =>
                    x.UpdateWithCanonicalSenderName(It.IsAny<ICollection<UserMessage>>()))
                .ThrowsAsync(new ArgumentException("This is a test"));

            // Act
            await FluentActions.Awaiting(() => _systemUnderTest.FindMessagesFromSenderByName("value", "value"))
                .Should().ThrowAsync<ArgumentException>();

            // Assert
            VerifyAll();
        }

        [TestMethod]
        public async Task FindMessagesFromSenderByName_ReturnsMessages()
        {
            // Arrange
            const string senderId = "Sender Id";

            var messages = new List<UserMessage>
            {
                new UserMessage
                {
                    Sender = "Sender1",
                    SenderContext = new SenderContext{ SenderId = senderId }
                },
                new UserMessage
                {
                    Sender = "Sender2",
                    SenderContext = new SenderContext{ SenderId = senderId }
                }
            };

            _mockRepository.Setup(x =>
                    x.Find( It.IsAny<Expression<Func<UserMessage, bool>>>(), It.IsAny<string>(), null))
                .ReturnsAsync(new RepositoryFindResult<UserMessage>.Found(messages));

            _mockCanonicalSenderNameService.Setup(x =>
                    x.UpdateWithCanonicalSenderName(messages))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _systemUnderTest.FindMessagesFromSenderByName("value", "value");

            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<RepositoryFindResult<UserMessage>.Found>()
                .Subject.Records.Should().BeEquivalentTo(messages);

            VerifyAll();
        }

        [TestMethod]
        public async Task FindMessagesFromSenderByName_WhenCannotFindMatchingRecords_ShouldReturnEmptyList()
        {
            // Arrange
            _mockRepository.Setup(x =>
                    x.Find(It.IsAny<Expression<Func<UserMessage, bool>>>(), It.IsAny<string>(), null))
                .ReturnsAsync(new RepositoryFindResult<UserMessage>.NotFound());

            // Act
            var result = await _systemUnderTest.FindMessagesFromSenderByName("value", "value");

            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<RepositoryFindResult<UserMessage>.NotFound>();

            VerifyAll();
        }

        [TestMethod]
        [DataRow(null, "test", "nhsLoginId")]
        [DataRow("test", null, "senderId")]
        public void FindMessagesFromSenderById_WhenArgumentsAreNotValid_ThrowsException(string nhsLoginId, string senderId, string paramName)
        {
            // Act
            Func<Task> act = async () => await _systemUnderTest.FindMessagesFromSenderById(nhsLoginId, senderId);

            // Assert
            act.Should().ThrowAsync<ArgumentNullException>()
                .WithParameterName(paramName);

            VerifyAll();
        }

        [TestMethod]
        public async Task FindMessagesFromSenderById_ReturnsMessages()
        {
            // Arrange
            const string senderId = "Sender Id";

            var messages = new List<UserMessage>
            {
                new UserMessage
                {
                    Sender = "Sender1",
                    SenderContext = new SenderContext{ SenderId = senderId }
                },
                new UserMessage
                {
                    Sender = "Sender2",
                    SenderContext = new SenderContext{ SenderId = "NotSenderId" }
                }
            };

            _mockRepository.Setup(x =>
                    x.Find(It.IsAny<Expression<Func<UserMessage, bool>>>(), It.IsAny<string>(), null))
                .ReturnsAsync(new RepositoryFindResult<UserMessage>.Found(messages));

            _mockCanonicalSenderNameService.Setup(x =>
                    x.UpdateWithCanonicalSenderName(messages))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _systemUnderTest.FindMessagesFromSenderById("value", senderId);

            // Assert
            result.Should().NotBeNull();
            var records = result.Should().BeAssignableTo<RepositoryFindResult<UserMessage>.Found>()
                .Subject.Records;
            records.Should().HaveCount(1);
            records.Should().BeEquivalentTo(messages);

            VerifyAll();
        }

        [TestMethod]
        public async Task FindMessagesFromSenderById_WhenCannotFindMatchingRecords_ShouldReturnEmptyList()
        {
            // Arrange
            _mockRepository.Setup(x =>
                    x.Find(It.IsAny<Expression<Func<UserMessage, bool>>>(), It.IsAny<string>(), null))
                .ReturnsAsync(new RepositoryFindResult<UserMessage>.NotFound());

            // Act
            var result = await _systemUnderTest.FindMessagesFromSenderById("value", "value");

            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<RepositoryFindResult<UserMessage>.NotFound>();

            VerifyAll();
        }

        [TestMethod]
        public void FindMessage_WhenMessageIdIsNull_ThrowsException()
        {
            // Act
            Func<Task> act = async () => await _systemUnderTest.FindMessage("value", null);

            // Assert
            act.Should().ThrowAsync<ArgumentNullException>()
                .WithParameterName("messageId");

            VerifyAll();
        }

        [TestMethod]
        public void FindMessage_WhenNhsLoginIdIsNull_ThrowsException()
        {
            // Act
            Func<Task> act = async () => await _systemUnderTest.FindMessage(null, "value");

            // Assert
            act.Should().ThrowAsync<ArgumentNullException>()
                .WithParameterName("nhsLoginId");

            VerifyAll();
        }

        [TestMethod]
        public async Task FindMessage_ReturnMessage()
        {
            // Arrange
            const string senderId = "Sender Id";
            const string nhsLoginId = "value";

            var userMessages = new List<UserMessage>
            {
                new UserMessage
                {
                    Sender = "Not CanonicalSenderName",
                    SenderContext = new SenderContext
                    {
                        SenderId = senderId
                    }
                }
            };

            _mockRepository.Setup(x =>
                    x.Find(It.IsAny<Expression<Func<UserMessage, bool>>>(), It.IsAny<string>(), null))
                .ReturnsAsync(new RepositoryFindResult<UserMessage>.Found(userMessages));

            _mockCanonicalSenderNameService.Setup(s =>
                    s.UpdateWithCanonicalSenderName(userMessages))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _systemUnderTest.FindMessage(nhsLoginId, userMessages.First().Id.ToString());

            // Assert
            result.Should().BeOfType<RepositoryFindResult<UserMessage>.Found>()
                .Subject.Records.Should().BeEquivalentTo(userMessages);

            VerifyAll();
        }

        [TestMethod]
        public async Task FindMessage_WhenRecordDoesNotExist_ShouldNotReturnRecord()
        {
            // Arrange
            _mockRepository.Setup(x =>
                    x.Find(It.IsAny<Expression<Func<UserMessage, bool>>>(), It.IsAny<string>(), null))
                .ReturnsAsync(new RepositoryFindResult<UserMessage>.NotFound());

            // Act
            var result = await _systemUnderTest.FindMessage("nhsLoginId", "fd9fb3db27402da79fe66515");

            // Assert;
            result.Should().BeAssignableTo<RepositoryFindResult<UserMessage>.NotFound>();

            VerifyAll();
        }

        [TestMethod]
        public async Task FindMessage_WhenCanonicalSenderNameServiceThrowsException_ThrowsException()
        {
            // Arrange
            const string nhsLoginId = "value";

            var userMessage = new UserMessage();

            _mockRepository.Setup(x =>
                    x.Find(It.IsAny<Expression<Func<UserMessage, bool>>>(), It.IsAny<string>(), null))
                .ReturnsAsync(new RepositoryFindResult<UserMessage>.Found(new []{userMessage}));

            _mockCanonicalSenderNameService.Setup(s =>
                    s.UpdateWithCanonicalSenderName(It.IsAny<ICollection<UserMessage>>()))
                .ThrowsAsync(new ArgumentException("This is a test"));

            // Act
            await FluentActions.Awaiting(() => _systemUnderTest.FindMessage(nhsLoginId, userMessage.Id.ToString()))
                .Should().ThrowAsync<ArgumentException>();

            // Assert
            VerifyAll();
        }

        [TestMethod]
        public void UpdateOne_WhenMessageIdIsNull_ThrowsException()
        {
            // Act
            Func<Task> act = async () => await _systemUnderTest.UpdateOne(
                "NhsLoginId",
                null,
                It.IsAny<UpdateRecordBuilder<UserMessage>>());

            // Assert
            act.Should().ThrowAsync<ArgumentNullException>()
                .WithParameterName("messageId");

            VerifyAll();
        }

        [TestMethod]
        public void UpdateOne_WhenNhsLoginIdIsNull_ThrowsException()
        {
            // Act
            Func<Task> act = async () => await _systemUnderTest.UpdateOne(
                null,
                "MessageId",
                It.IsAny<UpdateRecordBuilder<UserMessage>>());

            // Assert
            act.Should().ThrowAsync<ArgumentNullException>()
                .WithParameterName("nhsLoginId");

            VerifyAll();
        }

        [TestMethod]
        public async Task UpdateOne_WithMessage_ShouldUpdateRecord()
        {
            // Arrange
            _mockRepository.Setup(x =>
                    x.Update(It.IsAny<Expression<Func<UserMessage, bool>>>(),
                        It.IsAny<UpdateRecordBuilder<UserMessage>>(),
                        It.IsAny<string>()))
                .ReturnsAsync(new RepositoryUpdateResult<UserMessage>.Updated());

            // Act
            var result = await _systemUnderTest.UpdateOne("NhsLoginId", "fd9fb3db27402da79fe66515", new UpdateRecordBuilder<UserMessage>());

            // Assert
            result.Should().BeOfType<RepositoryUpdateResult<UserMessage>.Updated>();

            VerifyAll();
        }

        [TestMethod]
        public void FindAllForUser_WhenNhsLoginIdIsNull_ThrowsException()
        {
            // Act
            Func<Task> act = async () => await _systemUnderTest.FindAllForUser(null);

            // Assert
            act.Should().ThrowAsync<ArgumentNullException>()
                .WithParameterName("nhsLoginId");

            VerifyAll();
        }

        [TestMethod]
        public async Task FindAllForUser_WhenCanonicalSenderNameServiceThrowsException_ThrowsException()
        {
            // Arrange
            var messages = new List<UserMessage> { new UserMessage(), new UserMessage() };

            _mockRepository.Setup(x =>
                    x.Find(It.IsAny<Expression<Func<UserMessage, bool>>>(), It.IsAny<string>(), null))
                .ReturnsAsync(new RepositoryFindResult<UserMessage>.Found(messages));

            _mockCanonicalSenderNameService.Setup(x =>
                    x.UpdateWithCanonicalSenderName(messages))
                .ThrowsAsync(new ArgumentException("This is a test"));

            // Act
            await FluentActions.Awaiting(() => _systemUnderTest.FindAllForUser("nhsLoginId"))
                .Should().ThrowAsync<ArgumentException>();

            // Assert
            VerifyAll();
        }

        [TestMethod]
        public async Task FindAllForUser_ReturnsMessages()
        {
            // Arrange
            var messages = new List<UserMessage> { new UserMessage(), new UserMessage() };

            _mockRepository.Setup(x =>
                    x.Find(It.IsAny<Expression<Func<UserMessage, bool>>>(), It.IsAny<string>(), null))
                .ReturnsAsync(new RepositoryFindResult<UserMessage>.Found(messages));

            _mockCanonicalSenderNameService.Setup(x => x.UpdateWithCanonicalSenderName(messages))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _systemUnderTest.FindAllForUser("nhsLoginId");

            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<RepositoryFindResult<UserMessage>.Found>()
                .Subject.Records.Should().BeEquivalentTo(messages);

            VerifyAll();
        }

        [TestMethod]
        public async Task FindAllForUser_WhenCannotFindMatchingRecords_ShouldReturnEmptyList()
        {
            // Arrange
            _mockRepository.Setup(x =>
                    x.Find(It.IsAny<Expression<Func<UserMessage, bool>>>(), It.IsAny<string>(), null))
                .ReturnsAsync(new RepositoryFindResult<UserMessage>.NotFound());

            // Act
            var result = await _systemUnderTest.FindAllForUser("nhsLoginId");

            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<RepositoryFindResult<UserMessage>.NotFound>();

            VerifyAll();
        }

                [TestMethod]
        public void FindAllForUserV1_WhenNhsLoginIdIsNull_ThrowsException()
        {
            // Act
            Func<Task> act = async () => await _systemUnderTest.FindAllForUserV1(null);

            // Assert
            act.Should().ThrowAsync<ArgumentNullException>()
                .WithParameterName("nhsLoginId");

            VerifyAll();
        }

        [TestMethod]
        public async Task FindAllForUserV1_ReturnsMessages()
        {
            // Arrange
            var messages = new List<UserMessage> { new UserMessage(), new UserMessage() };

            _mockRepository.Setup(x =>
                    x.Find(It.IsAny<Expression<Func<UserMessage, bool>>>(), It.IsAny<string>(), null))
                .ReturnsAsync(new RepositoryFindResult<UserMessage>.Found(messages));

            // Act
            var result = await _systemUnderTest.FindAllForUserV1("nhsLoginId");

            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<RepositoryFindResult<UserMessage>.Found>()
                .Subject.Records.Should().BeEquivalentTo(messages);

            VerifyAll();
        }

        [TestMethod]
        public async Task FindAllForUserV1_WhenCannotFindMatchingRecords_ShouldReturnEmptyList()
        {
            // Arrange
            _mockRepository.Setup(x =>
                    x.Find(It.IsAny<Expression<Func<UserMessage, bool>>>(), It.IsAny<string>(), null))
                .ReturnsAsync(new RepositoryFindResult<UserMessage>.NotFound());

            // Act
            var result = await _systemUnderTest.FindAllForUserV1("nhsLoginId");

            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<RepositoryFindResult<UserMessage>.NotFound>();

            VerifyAll();
        }


        private void VerifyAll()
        {
            _mockRepository.VerifyAll();
            _mockCanonicalSenderNameService.VerifyAll();
        }
    }
}
