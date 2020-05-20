using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;
using NHSOnline.Backend.MessagesApi.Repository;
using NHSOnline.Backend.Repository;

namespace NHSOnline.Backend.MessagesApi.UnitTests.Repository
{
    [TestClass]
    public class MessageRepositoryTests
    {
        private MessageRepository _systemUnderTest;
        private Mock<IRepository<UserMessage>> _mockRepository;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockRepository = new Mock<IRepository<UserMessage>>();

            _systemUnderTest = new MessageRepository(new Mock<ILogger<MessageRepository>>().Object, _mockRepository.Object);
        }

        [TestMethod]
        public void Create_WhenUserMessageIsNull_ThrowsException()
        {
            // Act
            Func<Task> act = async () => await _systemUnderTest.Create(null);

            // Assert
            act.Should().Throw<AggregateException>()
                .And.InnerExceptions.Should().HaveCount(1)
                .And.AllBeOfType<ArgumentNullException>()
                .And.Contain(x =>
                    ((ArgumentNullException) x).ParamName.Equals("userMessage", StringComparison.Ordinal));
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
        }

        [TestMethod]
        [DataRow(null, "test", "nhsLoginId")]
        [DataRow("test", null, "sender")]
        public void FindMessagesFromSender_WhenArgumentsAreNotValid_ThrowsException(string nhsLoginId, string sender, string paramName)
        {
            // Act
            Func<Task> act = async () => await _systemUnderTest.FindMessagesFromSender(nhsLoginId, sender);

            // Assert
            act.Should().Throw<AggregateException>()
                .And.InnerExceptions.Should().HaveCount(1)
                .And.AllBeOfType<ArgumentNullException>()
                .And.Contain(x => ((ArgumentNullException) x).ParamName.Equals(paramName, StringComparison.Ordinal));
        }

        [TestMethod]
        public async Task FindMessagesFromSender_ReturnsMessages()
        {
            // Arrange
            var messages = new List<UserMessage> { new UserMessage(), new UserMessage() };
            _mockRepository.Setup(x =>
                    x.Find( It.IsAny<Expression<Func<UserMessage, bool>>>(), It.IsAny<string>()))
                .ReturnsAsync(new RepositoryFindResult<UserMessage>.Found(messages));

            // Act
            var result = await _systemUnderTest.FindMessagesFromSender("value", "value");

            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<RepositoryFindResult<UserMessage>.Found>().Subject.Records.Should().BeEquivalentTo(messages);
        }

        [TestMethod]
        public async Task FindMessagesFromSender_WhenCannotFindMatchingRecords_ShouldReturnEmptyList()
        {
            // Arrange
            _mockRepository.Setup(x =>
                    x.Find(It.IsAny<Expression<Func<UserMessage, bool>>>(), It.IsAny<string>()))
                .ReturnsAsync(new RepositoryFindResult<UserMessage>.NotFound());

            // Act
            var result = await _systemUnderTest.FindMessagesFromSender("value", "value");

            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<RepositoryFindResult<UserMessage>.NotFound>();
        }

        [TestMethod]
        public void FindMessage_WhenMessageIdIsNull_ThrowsException()
        {
            // Act
            Func<Task> act = async () => await _systemUnderTest.FindMessage("value", null);

            // Assert
            act.Should().Throw<AggregateException>()
                .And.InnerExceptions.Should().HaveCount(1)
                .And.AllBeOfType<ArgumentNullException>()
                .And.Contain(x =>
                    ((ArgumentNullException) x).ParamName.Equals("messageId", StringComparison.Ordinal));
        }

        [TestMethod]
        public void FindMessage_WhenNhsLoginidIsNull_ThrowsException()
        {
            // Act
            Func<Task> act = async () => await _systemUnderTest.FindMessage(null, "value");

            // Assert
            act.Should().Throw<AggregateException>()
                .And.InnerExceptions.Should().HaveCount(1)
                .And.AllBeOfType<ArgumentNullException>()
                .And.Contain(x =>
                    ((ArgumentNullException) x).ParamName.Equals("nhsLoginId", StringComparison.Ordinal));
        }

        [TestMethod]
        public async Task FindMessage_ReturnMessage()
        {
            // Arrange
            var userMessage = new UserMessage();
            var nhsLoginId = "value";
            _mockRepository.Setup(x =>
                    x.Find(It.IsAny<Expression<Func<UserMessage, bool>>>(), It.IsAny<string>()))
                .ReturnsAsync(new RepositoryFindResult<UserMessage>.Found(new []{userMessage}));

            // Act
            var result = await _systemUnderTest.FindMessage(nhsLoginId, userMessage.Id.ToString());

            // Assert
            result.Should().BeOfType<RepositoryFindResult<UserMessage>.Found>().Subject.Records.Should().BeEquivalentTo(new[]{userMessage});
        }

        [TestMethod]
        public async Task FindMessage_WhenRecordDoesNotExist_ShouldNotReturnRecord()
        {
            // Arrange
            _mockRepository.Setup(x =>
                    x.Find(It.IsAny<Expression<Func<UserMessage, bool>>>(), It.IsAny<string>()))
                .ReturnsAsync(new RepositoryFindResult<UserMessage>.NotFound());

            // Act
            var result = await _systemUnderTest.FindMessage("nhsLoginId", "fd9fb3db27402da79fe66515");

            // Assert;
            result.Should().BeAssignableTo<RepositoryFindResult<UserMessage>.NotFound>();
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
            act.Should().Throw<AggregateException>()
                .And.InnerExceptions.Should().HaveCount(1)
                .And.AllBeOfType<ArgumentNullException>()
                .And.Contain(x =>
                    ((ArgumentNullException)x).ParamName.Equals("messageId", StringComparison.Ordinal));
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
            act.Should().Throw<AggregateException>()
                .And.InnerExceptions.Should().HaveCount(1)
                .And.AllBeOfType<ArgumentNullException>()
                .And.Contain(x =>
                    ((ArgumentNullException)x).ParamName.Equals("nhsLoginId", StringComparison.Ordinal));
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
            result.Should().BeOfType< RepositoryUpdateResult<UserMessage>.Updated>();
        }

        [TestMethod]
        public void Summary_WhenNhsLoginIdIsNull_ThrowsException()
        {
            // Act
            Func<Task> act = async () => await _systemUnderTest.FindAllForUser(null);

            // Assert
            act.Should().Throw<AggregateException>()
                .And.InnerExceptions.Should().HaveCount(1)
                .And.AllBeOfType<ArgumentNullException>()
                .And.Contain(x => ((ArgumentNullException) x).ParamName.Equals("nhsLoginId", StringComparison.Ordinal));
        }

        [TestMethod]
        public async Task Summary_ReturnsMessages()
        {
            // Arrange
            var messages = new List<UserMessage> { new UserMessage(), new UserMessage() };
            _mockRepository.Setup(x =>
                    x.Find(It.IsAny<Expression<Func<UserMessage, bool>>>(), It.IsAny<string>()))
                .ReturnsAsync(new RepositoryFindResult<UserMessage>.Found(messages));

            // Act
            var result = await _systemUnderTest.FindAllForUser("nhsLoginId");

            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<RepositoryFindResult<UserMessage>.Found>().Subject.Records.Should().BeEquivalentTo(messages );
        }

        [TestMethod]
        public async Task Summary_WhenCannotFindMatchingRecords_ShouldReturnEmptyList()
        {
            // Arrange
            _mockRepository.Setup(x =>
                    x.Find(It.IsAny<Expression<Func<UserMessage, bool>>>(), It.IsAny<string>()))
                .ReturnsAsync(new RepositoryFindResult<UserMessage>.NotFound());

            // Act
            var result = await _systemUnderTest.FindAllForUser("nhsLoginId");

            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<RepositoryFindResult<UserMessage>.NotFound>();
        }
    }
}
