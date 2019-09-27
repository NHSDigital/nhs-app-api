using System;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using Moq;
using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;
using NHSOnline.Backend.MessagesApi.Repository;

namespace NHSOnline.Backend.MessagesApi.UnitTests.Repository
{
    [TestClass]
    public class MongoMessageRepositoryTests
    {
        private IFixture _fixture;
        private MongoMessageRepository _systemUnderTest;
        private Mock<IMongoClient> _mockMongoClient;
        
        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            _mockMongoClient = _fixture.Freeze<Mock<IMongoClient>>();

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
                .And.Contain(x => ((ArgumentNullException) x).ParamName.Equals("userMessage", StringComparison.Ordinal));
        }

        [TestMethod]
        public async Task Create_WithUserMessage_AddsToCollection()
        {
            // Arrange
            var userMessage = _fixture.Create<UserMessage>();

            var mongoCollectionMock = _fixture.Create<Mock<IMongoCollection<UserMessage>>>();
            mongoCollectionMock.Setup(x => x.InsertOneAsync(userMessage, null, default))
                .Returns(Task.CompletedTask);

            var mongoDatabaseMock = _fixture.Create<Mock<IMongoDatabase>>();
            mongoDatabaseMock.Setup(x => x.GetCollection<UserMessage>(It.IsAny<string>(), null))
                .Returns(mongoCollectionMock.Object);
            
            _mockMongoClient.Setup(x => x.GetDatabase(It.IsAny<string>(), null))
                .Returns(mongoDatabaseMock.Object);

            // Act
            await _systemUnderTest.Create(userMessage);

            // Assert
            mongoCollectionMock.Verify(x => x.InsertOneAsync(userMessage, null, default));
        }
    }
}