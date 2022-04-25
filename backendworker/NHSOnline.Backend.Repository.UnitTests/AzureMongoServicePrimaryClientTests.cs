using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;

namespace NHSOnline.Backend.Repository.UnitTests
{
    [TestClass]
    public sealed class AzureMongoServicePrimaryClientTests: IDisposable
    {
        private AzureMongoServiceTestContext _context;

        private AzureMongoService _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _context = new AzureMongoServiceTestContext();

            _context.MockAzureMongoClient.SetupGet(x => x.IsHealthy).Returns(true);
            _context.MockAzureMongoClient.SetupGet(x => x.ActiveClient).Returns(_context.MockPrimaryMongoClient.Object);

            _systemUnderTest = new AzureMongoService(
                _context.MockLogger.Object,
                _context.MockMongoClientCreator.Object,
                _context.MockAzureKeyVaultService.Object,
                _context.MockAzureMongoClient.Object);
        }

        [TestMethod]
        public async Task InsertOneAsync_CompletesSuccessfully_WithPrimaryClient()
        {
            // Arrange
            var mockMongoCollection = _context.SetupMongoCollection<TestRepositoryRecord>();

            var record = new TestRepositoryRecord();
            mockMongoCollection
                .Setup(x => x.InsertOneAsync(record, null, default))
                .Returns(Task.CompletedTask);

            // Act
            await _systemUnderTest.InsertOneAsync(_context.MockRepositoryConfig.Object, record);

            // Assert
            mockMongoCollection.VerifyAll();
            _context.MockPrimaryMongoClient.VerifyAll();
        }

        [TestMethod]
        public async Task ReplaceOneAsync_CompletesSuccessfully_WithPrimaryClient()
        {
            // Arrange
            var mockMongoCollection = _context.SetupMongoCollection<TestRepositoryRecord>();

            var record = new TestRepositoryRecord();
            mockMongoCollection
                .Setup(x => x.ReplaceOneAsync(
                    It.IsAny<FilterDefinition<TestRepositoryRecord>>(),
                    record,
                    It.IsAny<ReplaceOptions>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ReplaceOneResult.Acknowledged(0, 1, "upsertedId"));

            // Act
            var result = await _systemUnderTest.ReplaceOneAsync(_context.MockRepositoryConfig.Object, _ => true, record, null);

            // Assert
            mockMongoCollection.VerifyAll();
            _context.MockPrimaryMongoClient.VerifyAll();
            result.Should().BeOfType<ReplaceOneResult.Acknowledged>();
        }

        [TestMethod]
        public async Task DeleteOneAsync_CompletesSuccessfully_WithPrimaryClient()
        {
            // Arrange
            var mockMongoCollection = _context.SetupMongoCollection<TestRepositoryRecord>();

            mockMongoCollection
                .Setup(x => x.DeleteOneAsync(
                    It.IsAny<FilterDefinition<TestRepositoryRecord>>(), default))
                .ReturnsAsync(new DeleteResult.Acknowledged(1));

            // Act
            var result = await _systemUnderTest.DeleteOneAsync<TestRepositoryRecord>(_context.MockRepositoryConfig.Object, _ => true);

            // Assert
            mockMongoCollection.VerifyAll();
            _context.MockPrimaryMongoClient.VerifyAll();
            result.Should().BeOfType<DeleteResult.Acknowledged>();
        }

        [TestMethod]
        public async Task FindAsync_CompletesSuccessfully_WithPrimaryClient()
        {
            // Arrange
            var mockMongoCollection = _context.SetupMongoCollection<TestRepositoryRecord>();
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
                await _systemUnderTest.FindAsync<TestRepositoryRecord>(_context.MockRepositoryConfig.Object, _ => true, default);

            // Assert
            mockMongoCollection.VerifyAll();
            _context.MockPrimaryMongoClient.VerifyAll();
            result.Current.Should().BeEquivalentTo(new []{record});
        }

        [TestMethod]
        public async Task UpdateManyAsync_CompletesSuccessfully_WithPrimaryClient()
        {
            // Arrange
            var mockMongoCollection = _context.SetupMongoCollection<TestRepositoryRecord>();

            var updateDefinition = Builders<TestRepositoryRecord>.Update
                .Set("record1", new TestRepositoryRecord())
                .Set("record2", new TestRepositoryRecord());

            mockMongoCollection
                .Setup(x => x.UpdateManyAsync(
                    It.IsAny<FilterDefinition<TestRepositoryRecord>>(),
                    It.IsAny<UpdateDefinition<TestRepositoryRecord>>(),null, default))
                .ReturnsAsync(new UpdateResult.Acknowledged(0, 1, "upsertedId"));

            // Act
            var result = await _systemUnderTest.UpdateManyAsync(_context.MockRepositoryConfig.Object, _ => true, updateDefinition);

            // Assert
            mockMongoCollection.VerifyAll();
            _context.MockPrimaryMongoClient.VerifyAll();
            result.Should().BeOfType<UpdateResult.Acknowledged>();
        }

        [TestMethod]
        public async Task InsertOneDocumentAsync_CompletesSuccessfully_WithPrimaryClient()
        {
            // Arrange
            var mockMongoCollection = _context.SetupMongoCollection<BsonDocument>();
            var record = new BsonDocument();

            mockMongoCollection
                .Setup(x => x.InsertOneAsync(record, null, default))
                .Returns(Task.CompletedTask);

            // Act
            await _systemUnderTest.InsertOneDocumentAsync(_context.MockRepositoryConfig.Object, record);

            // Assert
            mockMongoCollection.VerifyAll();
            _context.MockPrimaryMongoClient.Verify(
                x => x.GetDatabase(AzureMongoServiceTestContext.DatabaseName, It.IsAny<MongoDatabaseSettings>()));
        }

        [TestMethod]
        public async Task UpdateOneDocumentAsync_CompletesSuccessfully_WithPrimaryClient()
        {
            //Arrange
            var mockMongoCollection = _context.SetupMongoCollection<BsonDocument>();

            var updateDefinition = Builders<BsonDocument>.Update
                .Set("record1", new BsonDocument());

            mockMongoCollection
                .Setup(x => x.UpdateOneAsync(
                    It.IsAny<FilterDefinition<BsonDocument>>(),
                    It.IsAny<UpdateDefinition<BsonDocument>>(),null, default))
                .ReturnsAsync(new UpdateResult.Acknowledged(0, 1, "upsertedId"));

            // Act
            var result = await _systemUnderTest.UpdateOneDocumentAsync(_context.MockRepositoryConfig.Object, null, updateDefinition);

            // Assert
            mockMongoCollection.VerifyAll();
            _context.MockPrimaryMongoClient.Verify(
               x => x.GetDatabase(AzureMongoServiceTestContext.DatabaseName, It.IsAny<MongoDatabaseSettings>()));
            result.Should().BeOfType<UpdateResult.Acknowledged>();
        }

        [TestMethod]
        public async Task FindOneAndUpdateDocumentAsync_CompletesSuccessfully_WithPrimaryClient()
        {
            // Arrange
            var mockMongoCollection = _context.SetupMongoCollection<BsonDocument>();
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
            var result = await _systemUnderTest.FindOneAndUpdateDocumentAsync(_context.MockRepositoryConfig.Object, record, updateDefinition);

            // Assert
            mockMongoCollection.VerifyAll();
            _context.MockPrimaryMongoClient.Verify(
                x => x.GetDatabase(AzureMongoServiceTestContext.DatabaseName, It.IsAny<MongoDatabaseSettings>()));
            result.Should().BeEquivalentTo(record);
        }

        [TestMethod]
        public async Task FindFirstDocument_CompletesSuccessfully_WithPrimaryClient()
        {
            var mockMongoCollection = _context.SetupMongoCollection<BsonDocument>();
            var record = new BsonDocument();

            var mockBsonDocumentCursor = _context.ArrangeBsonDocumentCursor(record);

            mockMongoCollection
                .Setup(x => x.FindAsync(
                    It.IsAny<FilterDefinition<BsonDocument>>(),
                    It.IsAny<FindOptions<BsonDocument>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockBsonDocumentCursor.Object);

            // Act
            var result = await _systemUnderTest.FindFirstDocument(_context.MockRepositoryConfig.Object, record);

            // Assert
            mockMongoCollection.VerifyAll();
            _context.MockPrimaryMongoClient.Verify(
                x => x.GetDatabase(AzureMongoServiceTestContext.DatabaseName, It.IsAny<MongoDatabaseSettings>()));

            result.Should().BeEquivalentTo(record);
        }

        [TestMethod]
        public async Task DeleteOneDocumentAsync_CompletesSuccessfully_WithPrimaryClient()
        {
            var mockMongoCollection = _context.SetupMongoCollection<BsonDocument>();
            var record = new BsonDocument();

            mockMongoCollection
                .Setup(x => x.DeleteOneAsync(
                    It.IsAny<FilterDefinition<BsonDocument>>(), default))
                .ReturnsAsync(new DeleteResult.Acknowledged(1));

            // Act
            var result = await _systemUnderTest.DeleteOneDocumentAsync(_context.MockRepositoryConfig.Object, record);

            // Assert
            mockMongoCollection.VerifyAll();
            _context.MockPrimaryMongoClient.Verify(
                x => x.GetDatabase(AzureMongoServiceTestContext.DatabaseName, It.IsAny<MongoDatabaseSettings>()));
            result.Should().BeOfType<DeleteResult.Acknowledged>();
        }

        public void Dispose()
        {
            _systemUnderTest?.Dispose();
        }
    }
}
