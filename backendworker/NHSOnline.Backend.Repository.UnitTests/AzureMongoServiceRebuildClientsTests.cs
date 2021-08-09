using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using UnitTestHelper;

namespace NHSOnline.Backend.Repository.UnitTests
{
    [TestClass]
    public sealed class AzureMongoServiceRebuildClientsTests: IDisposable
    {
        private AzureMongoServiceTestContext _context;
        private AzureMongoService _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _context = new AzureMongoServiceTestContext();

            _context.MockAzureMongoClient
                .SetupGet(x => x.PrimaryClient)
                .Returns(_context.MockPrimaryMongoClient.Object);

            _context.MockAzureMongoClient
                .SetupGet(x => x.SecondaryClient)
                .Returns(_context.MockSecondaryMongoClient.Object);

            _context.MockAzureMongoClient
                .Setup(x => x.IsHealthy)
                .Returns(true);

            _context.MockAzureMongoClient
                .SetupSequence(x => x.ActiveClient)
                .Returns(_context.MockSecondaryMongoClient.Object)
                .Returns(_context.MockPrimaryMongoClient.Object);

            _context.ArrangeHealthChecks();

            _context.MockAzureMongoClient
                .Setup(x => x.UsingPrimary)
                .Returns(false);

            _context.MockAzureMongoClient
                .Setup(x => x.ReportAuthenticationFailure(AzureMongoClientType.Secondary))
                .Callback(() =>
                {
                    _context.MockAzureMongoClient
                        .Setup(y => y.IsHealthy)
                        .Returns(false);
                });

            _context.MockAzureMongoClient.SetupSet(x => x.UsingPrimary = It.IsAny<bool>())
                .Callback((bool newValue) =>
                {
                    _context.MockAzureMongoClient
                        .Setup(x => x.UsingPrimary)
                        .Returns(newValue);
                });

            _context.MockAzureMongoClient.SetupSet(x => x.IsHealthy = It.IsAny<bool>())
                .Callback((bool newValue) =>
                {
                    _context.MockAzureMongoClient
                        .Setup(x => x.IsHealthy)
                        .Returns(newValue);
                });

            _systemUnderTest = new AzureMongoService(
                _context.MockLogger.Object,
                _context.MockMongoClientCreator.Object,
                _context.MockAzureKeyVaultService.Object,
                _context.MockAzureMongoClient.Object);
        }

        [TestMethod]
        [DynamicData(nameof(AzureMongoServiceTestContext.ExceptionData), typeof(AzureMongoServiceTestContext))]
        public async Task InsertOneAsync_RebuildsClients_WhenSecondaryClientFailsWithAuthenticationError(Exception exception)
        {
            // Arrange
            var mockMongoCollection = _context.SetupMongoCollection<TestRepositoryRecord>();
            var record = new TestRepositoryRecord();

            mockMongoCollection
                .SetupSequence(x => x.InsertOneAsync(record, null, It.IsAny<CancellationToken>()))
                .Throws(exception) // first call
                .Returns(Task.CompletedTask); // second call

            // Act
            await _systemUnderTest.InsertOneAsync(_context.MockRepositoryConfig.Object, record);

            // Assert
            mockMongoCollection.Verify(x =>
                x.InsertOneAsync(record, null, It.IsAny<CancellationToken>()), Times.Exactly(2));
            PostTestVerifications();
        }

        [TestMethod]
        [DynamicData(nameof(AzureMongoServiceTestContext.ExceptionData), typeof(AzureMongoServiceTestContext))]
        public async Task ReplaceOneAsync_RebuildsClients_WhenSecondaryClientFailsWithAuthenticationError(Exception exception)
        {
            // Arrange
            var mockMongoCollection = _context.SetupMongoCollection<TestRepositoryRecord>();
            var record = new TestRepositoryRecord();

            mockMongoCollection
                .SetupSequence(x => x.ReplaceOneAsync(
                    It.IsAny<FilterDefinition<TestRepositoryRecord>>(),
                    record,
                    It.IsAny<ReplaceOptions>(),
                    It.IsAny<CancellationToken>()))
                .Throws(exception) // first call
                .ReturnsAsync(new ReplaceOneResult.Acknowledged(0, 1, "upsertedId")); //second call

            // Act
            var result = await _systemUnderTest.ReplaceOneAsync(_context.MockRepositoryConfig.Object, _ => true, record, null);

            // Assert
            mockMongoCollection.Verify(x =>
                x.ReplaceOneAsync(
                    It.IsAny<FilterDefinition<TestRepositoryRecord>>(),
                    record,
                    It.IsAny<ReplaceOptions>(),
                    It.IsAny<CancellationToken>()), Times.Exactly(2));

            result.Should().BeOfType<ReplaceOneResult.Acknowledged>();
            PostTestVerifications();
        }

        [TestMethod]
        [DynamicData(nameof(AzureMongoServiceTestContext.ExceptionData), typeof(AzureMongoServiceTestContext))]
        public async Task DeleteOneAsync_RebuildsClients_WhenSecondaryClientFailsWithAuthenticationError(Exception exception)
        {
            // Arrange
            var mockMongoCollection = _context.SetupMongoCollection<TestRepositoryRecord>();

            mockMongoCollection
                .SetupSequence(x => x.DeleteOneAsync(
                    It.IsAny<FilterDefinition<TestRepositoryRecord>>(), default))
                .Throws(exception) // first call
                .ReturnsAsync(new DeleteResult.Acknowledged(1));

            // Act
            var result = await _systemUnderTest.DeleteOneAsync<TestRepositoryRecord>(_context.MockRepositoryConfig.Object, _ => true);

            // Assert
            mockMongoCollection.Verify(
                x => x.DeleteOneAsync(
                    It.IsAny<FilterDefinition<TestRepositoryRecord>>(), default),
                Times.Exactly(2));

            result.Should().BeOfType<DeleteResult.Acknowledged>();
            PostTestVerifications();
        }

        [TestMethod]
        [DynamicData(nameof(AzureMongoServiceTestContext.ExceptionData), typeof(AzureMongoServiceTestContext))]
        public async Task FindAsync_RebuildsClients_WhenSecondaryClientFailsWithAuthenticationError(Exception exception)
        {
            // Arrange
            var mockMongoCollection = _context.SetupMongoCollection<TestRepositoryRecord>();
            var record = new TestRepositoryRecord();
            var mockCursor = new Mock<IAsyncCursor<TestRepositoryRecord>>();

            mockMongoCollection
                .SetupSequence(x => x.FindAsync(
                    It.IsAny<FilterDefinition<TestRepositoryRecord>>(),
                    It.IsAny<FindOptions<TestRepositoryRecord>>(),
                    It.IsAny<CancellationToken>()))
                .Throws(exception) // first call
                .ReturnsAsync(mockCursor.Object);

            mockCursor
                .Setup(c => c.Current)
                .Returns(new List<TestRepositoryRecord> { record });

            // Act
            var result =
                await _systemUnderTest.FindAsync<TestRepositoryRecord>(_context.MockRepositoryConfig.Object, _ => true, default);

            // Assert
            mockMongoCollection.Verify(
                x => x.FindAsync(
                    It.IsAny<FilterDefinition<TestRepositoryRecord>>(),
                    It.IsAny<FindOptions<TestRepositoryRecord>>(),
                    It.IsAny<CancellationToken>()),
                Times.Exactly(2));

            result.Current.Should().BeEquivalentTo(record);
            PostTestVerifications();
        }

        [TestMethod]
        [DynamicData(nameof(AzureMongoServiceTestContext.ExceptionData), typeof(AzureMongoServiceTestContext))]
        public async Task UpdateManyAsync_RebuildsClients_WhenSecondaryClientFailsWithAuthenticationError(Exception exception)
        {
            // Arrange
            var mockMongoCollection = _context.SetupMongoCollection<TestRepositoryRecord>();
            var updateDefinition = Builders<TestRepositoryRecord>.Update
                .Set("record1", new TestRepositoryRecord())
                .Set("record2", new TestRepositoryRecord());

            mockMongoCollection
                .SetupSequence(x => x.UpdateManyAsync(
                    It.IsAny<FilterDefinition<TestRepositoryRecord>>(),
                    It.IsAny<UpdateDefinition<TestRepositoryRecord>>(),null, default))
                .Throws(exception) // first call
                .ReturnsAsync(new UpdateResult.Acknowledged(0, 1, "upsertedId"));

            // Act
            var result = await _systemUnderTest.UpdateManyAsync(_context.MockRepositoryConfig.Object, _ => true, updateDefinition);

            // Assert
            mockMongoCollection.Verify(
                x => x.UpdateManyAsync(
                    It.IsAny<FilterDefinition<TestRepositoryRecord>>(),
                    It.IsAny<UpdateDefinition<TestRepositoryRecord>>(),null, default),
                Times.Exactly(2));

            result.Should().BeOfType<UpdateResult.Acknowledged>();
            PostTestVerifications();
        }

        [TestMethod]
        [DynamicData(nameof(AzureMongoServiceTestContext.ExceptionData), typeof(AzureMongoServiceTestContext))]
        public async Task InsertOneDocumentAsync_RebuildsClients_WhenSecondaryClientFailsWithAuthenticationError(Exception exception)
        {
            // Arrange
            var mockMongoCollection = _context.SetupMongoCollection<BsonDocument>();
            var record = new BsonDocument();

            mockMongoCollection
                .SetupSequence(x =>
                    x.InsertOneAsync(record, null, default))
                .Throws(exception)
                .Returns(Task.CompletedTask);

            // Act
            await _systemUnderTest.InsertOneDocumentAsync(_context.MockRepositoryConfig.Object, record);

            // Assert
            mockMongoCollection.Verify(x =>
                    x.InsertOneAsync(record, null, default), Times.Exactly(2));
            PostTestVerifications();
        }

        [TestMethod]
        [DynamicData(nameof(AzureMongoServiceTestContext.ExceptionData), typeof(AzureMongoServiceTestContext))]
        public async Task UpdateOneDocumentAsync_RebuildsClients_WhenSecondaryClientFailsWithAuthenticationError(Exception exception)
        {
            // Arrange
            var mockMongoCollection = _context.SetupMongoCollection<BsonDocument>();
            var updateDefinition = Builders<BsonDocument>.Update.Set("record1", new BsonDocument());

            mockMongoCollection
                .SetupSequence(x => x.UpdateOneAsync(
                    It.IsAny<FilterDefinition<BsonDocument>>(),
                    It.IsAny<UpdateDefinition<BsonDocument>>(), null, default))
                .Throws(exception) // first call
                .ReturnsAsync(new UpdateResult.Acknowledged(0, 1, "upsertedId"));

            // Act
            var result = await _systemUnderTest.UpdateOneDocumentAsync(_context.MockRepositoryConfig.Object, null, updateDefinition);

            // Assert
            mockMongoCollection.Verify(x => x.UpdateOneAsync(
                    It.IsAny<FilterDefinition<BsonDocument>>(),
                    It.IsAny<UpdateDefinition<BsonDocument>>(), null, default),
                Times.Exactly(2));

            result.Should().BeOfType<UpdateResult.Acknowledged>();
            PostTestVerifications();
        }

        [TestMethod]
        [DynamicData(nameof(AzureMongoServiceTestContext.ExceptionData), typeof(AzureMongoServiceTestContext))]
        public async Task FindOneAndUpdateDocumentAsync_RebuildsClients_WhenSecondaryClientFailsWithAuthenticationError(Exception exception)
        {
            // Arrange
            var mockMongoCollection = _context.SetupMongoCollection<BsonDocument>();
            var record = new BsonDocument();
            var updateDefinition = Builders<BsonDocument>.Update.Set("record1", record);

            mockMongoCollection
                .SetupSequence(x => x.FindOneAndUpdateAsync(
                    It.IsAny<FilterDefinition<BsonDocument>>(),
                    It.IsAny<UpdateDefinition<BsonDocument>>(),
                    It.IsAny<FindOneAndUpdateOptions<BsonDocument, BsonDocument>>(),
                    It.IsAny<CancellationToken>()))
                .Throws(exception)
                .ReturnsAsync(record);

            // Act
            var result = await _systemUnderTest.FindOneAndUpdateDocumentAsync(_context.MockRepositoryConfig.Object, record, updateDefinition);

            // Assert
            mockMongoCollection.Verify(
                x => x.FindOneAndUpdateAsync(
                    It.IsAny<FilterDefinition<BsonDocument>>(),
                    It.IsAny<UpdateDefinition<BsonDocument>>(),
                    It.IsAny<FindOneAndUpdateOptions<BsonDocument, BsonDocument>>(),
                    It.IsAny<CancellationToken>()),
                Times.Exactly(2));

            result.Should().BeEquivalentTo(record);
            PostTestVerifications();
        }

        [TestMethod]
        [DynamicData(nameof(AzureMongoServiceTestContext.ExceptionData), typeof(AzureMongoServiceTestContext))]
        public async Task FindFirstDocument_RebuildsClients_WhenSecondaryClientFailsWithAuthenticationError(Exception exception)
        {
            var mockMongoCollection = _context.SetupMongoCollection<BsonDocument>();
            var record = new BsonDocument();

            var mockBsonDocumentCursor = _context.ArrangeBsonDocumentCursor(record);

            mockMongoCollection
                .SetupSequence(x => x.FindAsync(
                    It.IsAny<FilterDefinition<BsonDocument>>(),
                    It.IsAny<FindOptions<BsonDocument>>(),
                    It.IsAny<CancellationToken>()))
                .Throws(exception)
                .ReturnsAsync(mockBsonDocumentCursor.Object);

            // Act
            var result = await _systemUnderTest.FindFirstDocument(_context.MockRepositoryConfig.Object, record);

            // Assert
            mockMongoCollection.Verify(
                x => x.FindAsync(
                    It.IsAny<FilterDefinition<BsonDocument>>(),
                    It.IsAny<FindOptions<BsonDocument>>(),
                    It.IsAny<CancellationToken>()),
                Times.Exactly(2));

            result.Should().BeEquivalentTo(record);
            PostTestVerifications();
        }

        [TestMethod]
        [DynamicData(nameof(AzureMongoServiceTestContext.ExceptionData), typeof(AzureMongoServiceTestContext))]
        public async Task DeleteOneDocumentAsync_RebuildsClients_WhenSecondaryClientFailsWithAuthenticationError(Exception exception)
        {
            // Arrange
            var mockMongoCollection = _context.SetupMongoCollection<BsonDocument>();
            var record = new BsonDocument();

            _context.MockMongoDatabase
                .Setup(x => x.GetCollection<BsonDocument>(AzureMongoServiceTestContext.CollectionName, It.IsAny<MongoCollectionSettings>()))
                .Returns(mockMongoCollection.Object);

            mockMongoCollection
                .SetupSequence(x => x.DeleteOneAsync(
                    It.IsAny<FilterDefinition<BsonDocument>>(), default))
                .Throws(exception)
                .ReturnsAsync(new DeleteResult.Acknowledged(1));

            // Act
            var result = await _systemUnderTest.DeleteOneDocumentAsync(_context.MockRepositoryConfig.Object, record);

            // Assert
            mockMongoCollection.Verify(
                x => x.DeleteOneAsync(
                    It.IsAny<FilterDefinition<BsonDocument>>(), default),
                Times.Exactly(2));

            result.Should().BeOfType<DeleteResult.Acknowledged>();
            PostTestVerifications();
        }

        private void PostTestVerifications()
        {
            _context.MockPrimaryMongoClient.VerifyAll();
            _context.MockSecondaryMongoClient.VerifyAll();
            _context.MockLogger.VerifyLogger(LogLevel.Information, "primary:True secondary:True activeClient:primary", Times.Once());
            _context.MockLogger.VerifyLogger(LogLevel.Information, "DoWithRecoveryAsync - Authentication error talking to database with Secondary client", Times.Once());
            _context.MockLogger.VerifyLogger(LogLevel.Information, "DoWithRecoveryAsync - Successful action after recovery with Primary client", Times.Once());
        }

        public void Dispose()
        {
            _systemUnderTest?.Dispose();
        }
    }
}
