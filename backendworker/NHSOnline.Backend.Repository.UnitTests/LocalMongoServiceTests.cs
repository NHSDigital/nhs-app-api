using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;

namespace NHSOnline.Backend.Repository.UnitTests
{
    [TestClass]
    public sealed class LocalMongoServiceTests
    {
        private Mock<ILogger<LocalMongoService>> _mockLogger;
        private Mock<IConfiguration> _mockConfiguration;
        private Mock<IMongoClientCreator> _mockMongoClientCreator;
        private Mock<INamedMongoClient> _mockMongoClient;
        private Mock<IRepositoryConfiguration> _mockRepositoryConfig;

        private Mock<IMongoDatabase> _mockMongoDatabase;

        private const string DatabaseName = "DatabaseName";
        private const string ConnectionString = "ConnectionString";
        internal const string CollectionName = "CollectionName";

        private LocalMongoService _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockLogger =new Mock<ILogger<LocalMongoService>>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockMongoClientCreator =new Mock<IMongoClientCreator>();
            _mockMongoClient = new Mock<INamedMongoClient>();
            _mockMongoDatabase = new Mock<IMongoDatabase>();
            _mockRepositoryConfig  = new Mock<IRepositoryConfiguration>();

            _mockMongoClient
                .Setup(x => x.GetDatabase(DatabaseName, It.IsAny<MongoDatabaseSettings>()))
                .Returns(_mockMongoDatabase.Object);

            _mockMongoClientCreator
                .Setup(x => x.CreatePrimaryMongoClient(ConnectionString))
                .Returns(_mockMongoClient.Object);

            _mockConfiguration.Setup(x => x["MONGO_DATABASE_NAME"]).Returns(DatabaseName);
            _mockConfiguration.Setup(x => x["MONGO_CONNECTION_STRING"]).Returns(ConnectionString);

            _mockRepositoryConfig
                .Setup(x => x.DatabaseName)
                .Returns(DatabaseName);

            _mockRepositoryConfig
                .Setup(x => x.CollectionName)
                .Returns(CollectionName);

            _systemUnderTest = new LocalMongoService(
                _mockLogger.Object,
                _mockConfiguration.Object,
                _mockMongoClientCreator.Object);
        }

        [TestMethod]
        public async Task InsertOneAsync_CompletesSuccessfully()
        {
            var mockMongoCollection = SetupMongoCollection<TestRepositoryRecord>();

            var record = new TestRepositoryRecord();
            mockMongoCollection
                .Setup(x => x.InsertOneAsync(record, null, default))
                .Returns(Task.CompletedTask);

            // Act
            await _systemUnderTest.InsertOneAsync(_mockRepositoryConfig.Object, record);

            // Assert
            mockMongoCollection.VerifyAll();
            _mockMongoClient.VerifyAll();
        }

               [TestMethod]
        public async Task ReplaceOneAsync_CompletesSuccessfully_WithPrimaryClient()
        {
            // Arrange
            var mockMongoCollection = SetupMongoCollection<TestRepositoryRecord>();

            var record = new TestRepositoryRecord();
            mockMongoCollection
                .Setup(x => x.ReplaceOneAsync(
                    It.IsAny<FilterDefinition<TestRepositoryRecord>>(),
                    record,
                    It.IsAny<ReplaceOptions>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ReplaceOneResult.Acknowledged(0, 1, "upsertedId"));

            // Act
            var result = await _systemUnderTest.ReplaceOneAsync(_mockRepositoryConfig.Object, _ => true, record, null);

            // Assert
            mockMongoCollection.VerifyAll();
            _mockMongoClient.VerifyAll();
            result.Should().BeOfType<ReplaceOneResult.Acknowledged>();
        }

        [TestMethod]
        public async Task DeleteOneAsync_CompletesSuccessfully_WithPrimaryClient()
        {
            // Arrange
            var mockMongoCollection = SetupMongoCollection<TestRepositoryRecord>();

            mockMongoCollection
                .Setup(x => x.DeleteOneAsync(
                    It.IsAny<FilterDefinition<TestRepositoryRecord>>(), default))
                .ReturnsAsync(new DeleteResult.Acknowledged(1));

            // Act
            var result = await _systemUnderTest.DeleteOneAsync<TestRepositoryRecord>(_mockRepositoryConfig.Object, _ => true);

            // Assert
            mockMongoCollection.VerifyAll();
            _mockMongoClient.VerifyAll();
            result.Should().BeOfType<DeleteResult.Acknowledged>();
        }

        [TestMethod]
        public async Task FindAsync_CompletesSuccessfully_WithPrimaryClient()
        {
            // Arrange
            var mockMongoCollection = SetupMongoCollection<TestRepositoryRecord>();
            var record = new TestRepositoryRecord();
            var mockCursor = new Mock<IAsyncCursor<TestRepositoryRecord>>();

            mockMongoCollection
                .Setup(x => x.FindAsync(
                    It.IsAny<FilterDefinition<TestRepositoryRecord>>(),
                    It.IsAny<FindOptions<TestRepositoryRecord>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockCursor.Object);

            mockCursor
                .Setup(c => c.Current)
                .Returns(new List<TestRepositoryRecord> { record });

            // Act
            var result =
                await _systemUnderTest.FindAsync<TestRepositoryRecord>(_mockRepositoryConfig.Object, _ => true, default);

            // Assert
            mockMongoCollection.VerifyAll();
            _mockMongoClient.VerifyAll();
            result.Current.Should().BeEquivalentTo(new []{record});
        }

        [TestMethod]
        public async Task UpdateManyAsync_CompletesSuccessfully_WithPrimaryClient()
        {
            // Arrange
            var mockMongoCollection = SetupMongoCollection<TestRepositoryRecord>();

            var updateDefinition = Builders<TestRepositoryRecord>.Update
                .Set("record1", new TestRepositoryRecord())
                .Set("record2", new TestRepositoryRecord());

            mockMongoCollection
                .Setup(x => x.UpdateManyAsync(
                    It.IsAny<FilterDefinition<TestRepositoryRecord>>(),
                    It.IsAny<UpdateDefinition<TestRepositoryRecord>>(),null, default))
                .ReturnsAsync(new UpdateResult.Acknowledged(0, 1, "upsertedId"));

            // Act
            var result = await _systemUnderTest.UpdateManyAsync(_mockRepositoryConfig.Object, _ => true, updateDefinition);

            // Assert
            mockMongoCollection.VerifyAll();
            _mockMongoClient.VerifyAll();
            result.Should().BeOfType<UpdateResult.Acknowledged>();
        }

        [TestMethod]
        public async Task InsertOneDocumentAsync_CompletesSuccessfully_WithPrimaryClient()
        {
            // Arrange
            var mockMongoCollection = SetupMongoCollection<BsonDocument>();
            var record = new BsonDocument();

            mockMongoCollection
                .Setup(x => x.InsertOneAsync(record, null, default))
                .Returns(Task.CompletedTask);

            // Act
            await _systemUnderTest.InsertOneDocumentAsync(_mockRepositoryConfig.Object, record);

            // Assert
            mockMongoCollection.VerifyAll();
            _mockMongoClient.Verify(
                x => x.GetDatabase(AzureMongoServiceTestContext.DatabaseName, It.IsAny<MongoDatabaseSettings>()));
        }

        [TestMethod]
        public async Task UpdateOneDocumentAsync_CompletesSuccessfully_WithPrimaryClient()
        {
            //Arrange
            var mockMongoCollection = SetupMongoCollection<BsonDocument>();

            var updateDefinition = Builders<BsonDocument>.Update
                .Set("record1", new BsonDocument());

            mockMongoCollection
                .Setup(x => x.UpdateOneAsync(
                    It.IsAny<FilterDefinition<BsonDocument>>(),
                    It.IsAny<UpdateDefinition<BsonDocument>>(),null, default))
                .ReturnsAsync(new UpdateResult.Acknowledged(0, 1, "upsertedId"));

            // Act
            var result = await _systemUnderTest.UpdateOneDocumentAsync(_mockRepositoryConfig.Object, null, updateDefinition);

            // Assert
            mockMongoCollection.VerifyAll();
            _mockMongoClient.Verify(
               x => x.GetDatabase(AzureMongoServiceTestContext.DatabaseName, It.IsAny<MongoDatabaseSettings>()));
            result.Should().BeOfType<UpdateResult.Acknowledged>();
        }

        [TestMethod]
        public async Task FindOneAndUpdateDocumentAsync_CompletesSuccessfully_WithPrimaryClient()
        {
            // Arrange
            var mockMongoCollection = SetupMongoCollection<BsonDocument>();
            var record = new BsonDocument();
            var updateDefinition = Builders<BsonDocument>.Update
                .Set("record1", record);

            mockMongoCollection
                .Setup(x => x.FindOneAndUpdateAsync(
                    It.IsAny<FilterDefinition<BsonDocument>>(),
                    It.IsAny<UpdateDefinition<BsonDocument>>(),
                    It.IsAny<FindOneAndUpdateOptions<BsonDocument, BsonDocument>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(record);

            // Act
            var result = await _systemUnderTest.FindOneAndUpdateDocumentAsync(_mockRepositoryConfig.Object, record, updateDefinition);

            // Assert
            mockMongoCollection.VerifyAll();
            _mockMongoClient.Verify(
                x => x.GetDatabase(AzureMongoServiceTestContext.DatabaseName, It.IsAny<MongoDatabaseSettings>()));
            result.Should().BeEquivalentTo(record);
        }

        [TestMethod]
        public async Task FindFirstDocument_CompletesSuccessfully_WithPrimaryClient()
        {
            var mockMongoCollection = SetupMongoCollection<BsonDocument>();
            var record = new BsonDocument();

            var mockBsonDocumentCursor = ArrangeBsonDocumentCursor(record);

            mockMongoCollection
                .Setup(x => x.FindAsync(
                    It.IsAny<FilterDefinition<BsonDocument>>(),
                    It.IsAny<FindOptions<BsonDocument>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockBsonDocumentCursor.Object);

            // Act
            var result = await _systemUnderTest.FindFirstDocument(_mockRepositoryConfig.Object, record);

            // Assert
            mockMongoCollection.VerifyAll();
            _mockMongoClient.Verify(
                x => x.GetDatabase(AzureMongoServiceTestContext.DatabaseName, It.IsAny<MongoDatabaseSettings>()));

            result.Should().BeEquivalentTo(record);
        }

        [TestMethod]
        public async Task DeleteOneDocumentAsync_CompletesSuccessfully_WithPrimaryClient()
        {
            var mockMongoCollection = SetupMongoCollection<BsonDocument>();
            var record = new BsonDocument();

            mockMongoCollection
                .Setup(x => x.DeleteOneAsync(
                    It.IsAny<FilterDefinition<BsonDocument>>(), default))
                .ReturnsAsync(new DeleteResult.Acknowledged(1));

            // Act
            var result = await _systemUnderTest.DeleteOneDocumentAsync(_mockRepositoryConfig.Object, record);

            // Assert
            mockMongoCollection.VerifyAll();
            _mockMongoClient.Verify(
                x => x.GetDatabase(AzureMongoServiceTestContext.DatabaseName, It.IsAny<MongoDatabaseSettings>()));
            result.Should().BeOfType<DeleteResult.Acknowledged>();
        }

        [TestMethod]
        public void RebuildIfNecessary_ThrowsNotImplementedException()
        {
            Action act = () => _systemUnderTest.RebuildIfNecessary(DatabaseName, It.IsAny<Guid>(), It.IsAny<uint>());
            act.Should().Throw<NotImplementedException>();
        }

        [TestMethod]
        public void SupportsConnectionRecovery_ShouldBeFalse()
        {
            _systemUnderTest.SupportsConnectionRecovery.Should().BeFalse();
        }

        private Mock<IMongoCollection<T>> SetupMongoCollection<T>()
        {
            var mockMongoCollection = new Mock<IMongoCollection<T>>();

            _mockMongoDatabase
                .Setup(x => x.GetCollection<T>(CollectionName, It.IsAny<MongoCollectionSettings>() ))
                .Returns(mockMongoCollection.Object);

            return mockMongoCollection;
        }

        private Mock<IAsyncCursor<BsonDocument>> ArrangeBsonDocumentCursor(BsonDocument record, int limit = 1)
        {
            var count = 0;
            var mockBsonDocumentCursor = new Mock<IAsyncCursor<BsonDocument>>();

            mockBsonDocumentCursor
                .Setup(c => c.Current)
                .Returns(new List<BsonDocument> { record });

            mockBsonDocumentCursor
                .Setup(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
                .Returns(() => {
                    count++;
                    return Task.FromResult(count <= limit);
                });

            return mockBsonDocumentCursor;
        }

    }
}
