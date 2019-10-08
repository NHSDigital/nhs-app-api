using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using Moq;
using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;
using NHSOnline.Backend.MessagesApi.Repository;
using NHSOnline.Backend.Support.Repository;
using UnitTestHelper;

namespace NHSOnline.Backend.MessagesApi.UnitTests.Repository
{
    [TestClass]
    public class MongoMessageRepositoryTests
    {
        private IFixture _fixture;
        private MongoMessageRepository _systemUnderTest;
        private Mock<IApiMongoClient<IMongoConfiguration>> _mockMongoClient;
        private Mock<IMongoCollection<UserMessage>> _mongoCollectionMock;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new MessagesApiCustomization());

            _mongoCollectionMock = _fixture.Create<Mock<IMongoCollection<UserMessage>>>();

            var mongoDatabaseMock = _fixture.Create<Mock<IMongoDatabase>>();
            mongoDatabaseMock.Setup(x => x.GetCollection<UserMessage>(It.IsAny<string>(), null))
                .Returns(_mongoCollectionMock.Object);

            _mockMongoClient = _fixture.Freeze<Mock<IApiMongoClient<IMongoConfiguration>>>();
            _mockMongoClient.Setup(x => x.GetDatabase(It.IsAny<string>(), null))
                .Returns(mongoDatabaseMock.Object);

            _systemUnderTest = _fixture.Create<MongoMessageRepository>();
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
            var userMessage = _fixture.Create<UserMessage>();

            var mongoCollectionMock = _fixture.Create<Mock<IMongoCollection<UserMessage>>>();

            var mongoDatabaseMock = _fixture.Create<Mock<IMongoDatabase>>();
            mongoDatabaseMock.Setup(x => x.GetCollection<UserMessage>(It.IsAny<string>(), null))
                .Returns(mongoCollectionMock.Object);

            _mockMongoClient.Setup(x => x.GetDatabase(It.IsAny<string>(), null))
                .Returns(mongoDatabaseMock.Object);
            mongoCollectionMock.Setup(x => x.InsertOneAsync(userMessage, null, default))
                .Returns(Task.CompletedTask);

            // Act
            await _systemUnderTest.Create(userMessage);

            // Assert
            mongoCollectionMock.Verify(x => x.InsertOneAsync(userMessage, null, default));
        }

        [TestMethod]
        [DataRow(null, "test", "nhsLoginId")]
        [DataRow("test", null, "sender")]
        public void Find_WhenArgumentsAreNotValid_ThrowsException(string nhsLoginId, string sender, string paramName)
        {
            // Act
            Func<Task> act = async () => await _systemUnderTest.Find(nhsLoginId, sender);

            // Assert
            act.Should().Throw<AggregateException>()
                .And.InnerExceptions.Should().HaveCount(1)
                .And.AllBeOfType<ArgumentNullException>()
                .And.Contain(x => ((ArgumentNullException) x).ParamName.Equals(paramName, StringComparison.Ordinal));
        }

        [TestMethod]
        public async Task Find_ReturnsMessages()
        {
            // Arrange
            var messages = new List<UserMessage> { _fixture.Create<UserMessage>(), _fixture.Create<UserMessage>() };
            var cursorMock = MongoHelper.CreateCursorMockFind(_fixture, messages);

            _mongoCollectionMock
                .Setup(x => x.FindAsync(It.IsAny<FilterDefinition<UserMessage>>(),
                    It.IsAny<FindOptions<UserMessage, UserMessage>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cursorMock.Object);

            // Act
            var result = await _systemUnderTest.Find(_fixture.Create<string>(), _fixture.Create<string>());

            // Assert
            _mongoCollectionMock.VerifyAll();
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(messages);
        }

        [TestMethod]
        public async Task Find_WhenCannotFindMatchingRecords_ShouldReturnEmptyList()
        {
            // Arrange
            var cursorMock = MongoHelper.CreateCursorMockFindNone<UserMessage>(_fixture);

            _mongoCollectionMock
                .Setup(x => x.FindAsync(It.IsAny<FilterDefinition<UserMessage>>(),
                    It.IsAny<FindOptions<UserMessage, UserMessage>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cursorMock.Object);

            // Act
            var result = await _systemUnderTest.Find(_fixture.Create<string>(), _fixture.Create<string>());

            // Assert
            _mongoCollectionMock.VerifyAll();
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [TestMethod]
        public void Summary_WhenNhsLoginIdIsNull_ThrowsException()
        {
            // Act
            Func<Task> act = async () => await _systemUnderTest.Summary(null);

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
            var messages = new List<SummaryMessage>
            {
                _fixture.Create<SummaryMessage>(),
                _fixture.Create<SummaryMessage>()
            };

            var cursorMock = MongoHelper.CreateCursorMockFind(_fixture, messages);

            _mongoCollectionMock.Setup(x => x.AggregateAsync(
                    It.IsAny<PipelineDefinition<UserMessage, SummaryMessage>>(),
                    It.IsAny<AggregateOptions>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cursorMock.Object);

            // Act
            var result = await _systemUnderTest.Summary(_fixture.Create<string>());

            // Assert
            _mongoCollectionMock.VerifyAll();
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(messages);
        }

        [TestMethod]
        public async Task Summary_WhenCannotFindMatchingRecords_ShouldReturnEmptyList()
        {
            // Arrange
            var cursorMock = MongoHelper.CreateCursorMockFindNone<SummaryMessage>(_fixture);

            _mongoCollectionMock.Setup(x => x.AggregateAsync(
                    It.IsAny<PipelineDefinition<UserMessage, SummaryMessage>>(),
                    It.IsAny<AggregateOptions>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cursorMock.Object);

            // Act
            var result = await _systemUnderTest.Summary(_fixture.Create<string>());

            // Assert
            _mongoCollectionMock.VerifyAll();
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }
    }
}