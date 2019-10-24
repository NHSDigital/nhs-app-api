using System;
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
using NHSOnline.Backend.MessagesApi.UnitTests.Areas.Messages;
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
                .Customize(new AutoMoqCustomization());

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
            var userMessage = MessageHelpers.MockUserMessage(_fixture);

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
        public void Find_WhenNhsLoginIdIsNull_ThrowsException()
        {
            // Act
            Func<Task> act = async () => await _systemUnderTest.Find(null);

            // Assert
            act.Should().Throw<AggregateException>()
                .And.InnerExceptions.Should().HaveCount(1)
                .And.AllBeOfType<ArgumentNullException>()
                .And.Contain(x => ((ArgumentNullException) x).ParamName.Equals("nhsLoginId", StringComparison.Ordinal));
        }

        [TestMethod]
        public async Task Find_ReturnsMessages()
        {
            // Arrange
            var userMessage1 = MessageHelpers.MockUserMessage(_fixture);
            var userMessage2 = MessageHelpers.MockUserMessage(_fixture);
            var cursorMock = MongoHelper.CreateCursorMockFind(_fixture, new[] { userMessage1, userMessage2 });

            _mongoCollectionMock
                .Setup(x => x.FindAsync(It.IsAny<FilterDefinition<UserMessage>>(),
                    It.IsAny<FindOptions<UserMessage, UserMessage>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cursorMock.Object);

            // Act
            var result = await _systemUnderTest.Find(_fixture.Create<string>());

            // Assert
            _mongoCollectionMock.VerifyAll();
            result.Count.Should().Be(2);
            result[0].Should().BeEquivalentTo(userMessage1);
            result[1].Should().BeEquivalentTo(userMessage2);
        }

        [TestMethod]
        public async Task Find_WhenRecordDoesNotExist_ShouldNotReturnRecord()
        {
            // Arrange
            var cursorMock = MongoHelper.CreateCursorMockFindNone<UserMessage>(_fixture);

            _mongoCollectionMock
                .Setup(x => x.FindAsync(It.IsAny<FilterDefinition<UserMessage>>(),
                    It.IsAny<FindOptions<UserMessage, UserMessage>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cursorMock.Object);

            // Act
            var result = await _systemUnderTest.Find(_fixture.Create<string>());

            // Assert
            _mongoCollectionMock.VerifyAll();
            result.Should().BeEmpty();
        }
    }
}