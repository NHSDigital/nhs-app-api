using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
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
        public void FindOne_WhenMessageIdIsNull_ThrowsException()
        {
            // Act
            Func<Task> act = async () => await _systemUnderTest.FindOne(null);

            // Assert
            act.Should().Throw<AggregateException>()
                .And.InnerExceptions.Should().HaveCount(1)
                .And.AllBeOfType<ArgumentNullException>()
                .And.Contain(x =>
                    ((ArgumentNullException) x).ParamName.Equals("messageId", StringComparison.Ordinal));
        }

        [TestMethod]
        public async Task FindOne_ReturnMessage()
        {
            // Arrange
            var userMessage = _fixture.Create<UserMessage>();
            var cursorMock = MongoHelper.CreateCursorMockFind(_fixture, new[] { userMessage });

            _mongoCollectionMock
                .Setup(x => x.FindAsync(It.IsAny<FilterDefinition<UserMessage>>(),
                    It.IsAny<FindOptions<UserMessage, UserMessage>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cursorMock.Object);

            // Act
            var result = await _systemUnderTest.FindOne(userMessage.Id.ToString());

            // Assert
            _mongoCollectionMock.VerifyAll();
            result.Should().BeEquivalentTo(userMessage);
        }

        [TestMethod]
        public async Task FindOne_WhenRecordDoesNotExist_ShouldNotReturnRecord()
        {
            // Arrange
            var cursorMock = MongoHelper.CreateCursorMockFindNone<UserMessage>(_fixture);
            var id = new ObjectId(_fixture.CreateStringFromRegex("^[0-9a-f]{24}$")); //24 digit hex regex

            _mongoCollectionMock
                .Setup(x => x.FindAsync(It.IsAny<FilterDefinition<UserMessage>>(),
                    It.IsAny<FindOptions<UserMessage, UserMessage>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cursorMock.Object);

            // Act
            var result = await _systemUnderTest.FindOne(id.ToString());

            // Assert
            _mongoCollectionMock.VerifyAll();
            result.Should().BeNull();
        }

        [TestMethod]
        public void UpdateOne_WhenMessageIsNull_ThrowsException()
        {
            // Act
            Func<Task> act = async () => await _systemUnderTest.UpdateOne(null);

            // Assert
            act.Should().Throw<AggregateException>()
                .And.InnerExceptions.Should().HaveCount(1)
                .And.AllBeOfType<ArgumentNullException>()
                .And.Contain(x =>
                    ((ArgumentNullException) x).ParamName.Equals("userMessage", StringComparison.Ordinal));
        }

        [TestMethod]
        public async Task UpdateOne_UpdateMessage()
        {
            // Arrange
            var userMessage = _fixture.Create<UserMessage>();
            userMessage.ReadTime = null;
            var cursorMock = MongoHelper.CreateCursorMockFind(_fixture, new[] { userMessage });

            _mongoCollectionMock
                .Setup(x => x.FindAsync(It.IsAny<FilterDefinition<UserMessage>>(),
                    It.IsAny<FindOptions<UserMessage, UserMessage>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cursorMock.Object);

            Assert.AreEqual(null, userMessage.ReadTime);
            
            // Act
            userMessage.ReadTime = DateTime.UtcNow;
            await _systemUnderTest.UpdateOne(userMessage);
            var result = await _systemUnderTest.FindOne(userMessage.Id.ToString());

            // Assert
            _mongoCollectionMock.VerifyAll();
            Assert.AreEqual(userMessage.ReadTime, result.ReadTime);
        }

        [TestMethod]
        public async Task UpdateOne_WhenRecordDoesNotExist_ShouldNotUpdateRecord()
        {
            // Arrange
            var cursorMock = MongoHelper.CreateCursorMockFindNone<UserMessage>(_fixture);
            var userMessage = _fixture.Create<UserMessage>();

            _mongoCollectionMock
                .Setup(x => x.FindAsync(It.IsAny<FilterDefinition<UserMessage>>(),
                    It.IsAny<FindOptions<UserMessage, UserMessage>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cursorMock.Object);

            // Act
            await _systemUnderTest.UpdateOne(userMessage);
            var result = await _systemUnderTest.FindOne(userMessage.Id.ToString());

            // Assert
            _mongoCollectionMock.VerifyAll();
            result.Should().BeNull();           
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
            var latestFirstSenderMessage = _fixture.Build<UserMessage>()
                .With(x => x.Sender, "Sender 1")
                .With(x => x.SentTime, DateTime.UtcNow)
                .With(x => x.ReadTime, default(DateTime?))
                .Create();

            var latestSecondSenderMessage = _fixture.Build<UserMessage>()
                .With(x => x.Sender, "Sender 2")
                .With(x => x.SentTime, DateTime.UtcNow.AddSeconds(5))
                .With(x => x.ReadTime, default(DateTime?))
                .Create();

            var latestThirdSenderMessage = _fixture.Build<UserMessage>()
                .With(x => x.Sender, "Sender 3")
                .With(x => x.SentTime, DateTime.UtcNow.AddSeconds(-5))
                .With(x => x.ReadTime, DateTime.UtcNow)
                .Create();

            var messages = new List<UserMessage>
            {
                latestFirstSenderMessage,
                _fixture.Build<UserMessage>()
                    .With(x => x.Sender, latestSecondSenderMessage.Sender)
                    .With(x => x.SentTime, latestSecondSenderMessage.SentTime.AddSeconds(-1))
                    .With(x => x.ReadTime, default(DateTime?))
                    .Create(),
                latestSecondSenderMessage,
                _fixture.Build<UserMessage>()
                    .With(x => x.Sender, latestSecondSenderMessage.Sender)
                    .With(x => x.SentTime, latestSecondSenderMessage.SentTime)
                    .With(x => x.ReadTime, DateTime.UtcNow)
                    .Create(),
                _fixture.Build<UserMessage>()
                    .With(x => x.Sender, latestThirdSenderMessage.Sender)
                    .With(x => x.SentTime, latestThirdSenderMessage.SentTime.AddSeconds(-10))
                    .With(x => x.ReadTime, DateTime.UtcNow)
                    .Create(),
                latestThirdSenderMessage,
                _fixture.Build<UserMessage>()
                    .With(x => x.Sender, latestThirdSenderMessage.Sender)
                    .With(x => x.SentTime, latestThirdSenderMessage.SentTime.AddSeconds(-5))
                    .With(x => x.ReadTime, DateTime.UtcNow)
                    .Create(),
            };

            var cursorMock = MongoHelper.CreateCursorMockFind(_fixture, messages);

            _mongoCollectionMock
                .Setup(x => x.FindAsync(It.IsAny<FilterDefinition<UserMessage>>(),
                    It.IsAny<FindOptions<UserMessage, UserMessage>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cursorMock.Object);

            // Act
            var result = await _systemUnderTest.Summary(_fixture.Create<string>());

            // Assert
            _mongoCollectionMock.VerifyAll();
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(new List<SummaryMessage>
            {
                MapToSummaryMessage(latestSecondSenderMessage, 2),
                MapToSummaryMessage(latestFirstSenderMessage, 1),
                MapToSummaryMessage(latestThirdSenderMessage, 0),
            });
        }

        [TestMethod]
        public async Task Summary_WhenCannotFindMatchingRecords_ShouldReturnEmptyList()
        {
            // Arrange
            var cursorMock = MongoHelper.CreateCursorMockFindNone<SummaryMessage>(_fixture);

            _mongoCollectionMock
                .Setup(x => x.FindAsync(It.IsAny<FilterDefinition<UserMessage>>(),
                    It.IsAny<FindOptions<UserMessage, UserMessage>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cursorMock.Object);

            // Act
            var result = await _systemUnderTest.Summary(_fixture.Create<string>());

            // Assert
            _mongoCollectionMock.VerifyAll();
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        private SummaryMessage MapToSummaryMessage(UserMessage userMessage, int unreadCount)
            => new SummaryMessage
            {
                UnreadCount = unreadCount,
                Id = userMessage.Id,
                NhsLoginId = userMessage.NhsLoginId,
                Sender = userMessage.Sender,
                Version = userMessage.Version,
                Body = userMessage.Body,
                ReadTime = userMessage.ReadTime,
                SentTime = userMessage.SentTime
            };
    }
}