using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using UnitTestHelper;
using Range = Moq.Range;

namespace NHSOnline.Backend.Repository.UnitTests
{
    [TestClass]
    public sealed class AzureMongoServiceSemaphoreTests: IDisposable
    {
        private AzureMongoService _systemUnderTest;
        private AzureMongoServiceTestContext _context;
        private const int NumberOfSimultaneousActions = 20;
        private const int QueueDelayMilliseconds = 1000;
        private Mock<INamedMongoClient> _rebuiltPrimaryMongoClient;
        private Mock<IMongoDatabase> _rebuiltMongoDatabase;

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

            _rebuiltPrimaryMongoClient = new Mock<INamedMongoClient> { Name = "Rebuilt primary mongo client" };
            _rebuiltMongoDatabase = new Mock<IMongoDatabase> { Name = "Rebuilt mongo database" };

            _context.MockAzureMongoClient
                .Setup(x => x.IsHealthy)
                .Returns(true);

            _context.MockAzureMongoClient
                .SetupGet(x => x.ActiveClient)
                .Returns(_context.MockSecondaryMongoClient.Object);

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

            _context.MockAzureMongoClient.SetupSet(x => x.UsingPrimary = true)
                .Callback((bool newValue) =>
                {
                    _context.MockAzureMongoClient
                        .Setup(x => x.UsingPrimary)
                        .Returns(newValue);

                    _context.MockAzureMongoClient
                        .SetupGet(x => x.ActiveClient)
                        .Returns(_rebuiltPrimaryMongoClient.Object);
                });

            _context.MockAzureMongoClient.SetupSet(x => x.IsHealthy = It.IsAny<bool>())
                .Callback((bool newValue) =>
                {
                    _context.MockAzureMongoClient
                        .Setup(x => x.IsHealthy)
                        .Returns(newValue);
                });

            _context.MockAzureMongoClient.SetupSet(x => x.PrimaryClient = It.IsAny<INamedMongoClient>())
                .Callback((INamedMongoClient newValue) =>
                {
                    _context.MockAzureMongoClient
                        .Setup(x => x.PrimaryClient)
                        .Returns(newValue);
                });

            _context.MockAzureKeyVaultService
                .Setup(x => x.GetConnectionStrings())
                .ReturnsAsync(_context.NewConnectionStrings);

            _context.MockMongoClientCreator
                .Setup(x => x.CreatePrimaryMongoClient(AzureMongoServiceTestContext.NewPrimaryConnectionString))
                .Returns(_rebuiltPrimaryMongoClient.Object);

            _systemUnderTest = new AzureMongoService(
                _context.MockLogger.Object,
                _context.MockMongoClientCreator.Object,
                _context.MockAzureKeyVaultService.Object,
                _context.MockAzureMongoClient.Object);
        }

        [TestMethod]
        public async Task InsertOneAsync_SingleAccessToSemaphore_WhenSecondaryClientFailsWithAuthenticationError()
        {
            // Arrange
            var mockMongoCollection = _context.SetupMongoCollection<TestRepositoryRecord>();
            var record = new TestRepositoryRecord();

            mockMongoCollection
                .Setup(x => x.InsertOneAsync(record, null, default))
                .Returns(async() =>
                {
                    // Make sure requests are queued
                    await Task.Delay(QueueDelayMilliseconds);
                    throw AzureMongoServiceTestContext.GetMongoCommandAuthenticationException();
                });

            var rebuiltMongoCollection = new Mock<IMongoCollection<TestRepositoryRecord>> { Name = "Rebuilt mongo collection" };
            ArrangeDatabaseClient(_rebuiltPrimaryMongoClient, _rebuiltMongoDatabase, rebuiltMongoCollection, AzureMongoClientType.Primary);

            rebuiltMongoCollection
                .Setup(x =>
                    x.InsertOneAsync(record, null, default))
                .Returns(Task.CompletedTask);

            // Act
            var databaseTasks = new List<Task>();
            for (var i = 0; i < NumberOfSimultaneousActions; i++)
            {
                databaseTasks.Add(_systemUnderTest.InsertOneAsync(_context.MockRepositoryConfig.Object, record));
            }
            await Task.WhenAll(databaseTasks);

            // Assert
            rebuiltMongoCollection.VerifyAll();
            PostTestVerifications();
        }

        [TestMethod]
        public async Task ReplaceOneAsync_SingleAccessToSemaphore_WhenSecondaryClientFailsWithAuthenticationError()
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
                .Returns(async() =>
                {
                    await Task.Delay(QueueDelayMilliseconds);
                    throw AzureMongoServiceTestContext.GetMongoCommandAuthenticationException();
                });

            var rebuiltMongoCollection = new Mock<IMongoCollection<TestRepositoryRecord>> { Name = "Rebuilt mongo collection" };
            ArrangeDatabaseClient(_rebuiltPrimaryMongoClient, _rebuiltMongoDatabase, rebuiltMongoCollection, AzureMongoClientType.Primary);

            rebuiltMongoCollection
                .Setup(x =>
                    x.ReplaceOneAsync(
                        It.IsAny<FilterDefinition<TestRepositoryRecord>>(),
                        record,
                        It.IsAny<ReplaceOptions>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ReplaceOneResult.Acknowledged(0, 1, "upsertedId"));

            // Act
            var replaceTasks = new List<Task>();
            for (var i = 0; i < NumberOfSimultaneousActions; i++)
            {
                replaceTasks.Add(_systemUnderTest.ReplaceOneAsync(_context.MockRepositoryConfig.Object, _ => true, record, null));
            }
            await Task.WhenAll(replaceTasks);

            // Assert
            rebuiltMongoCollection.VerifyAll();
            PostTestVerifications();
        }

        [TestMethod]
        public async Task DeleteOneAsync_SingleAccessToSemaphore_WhenSecondaryClientFailsWithAuthenticationError()
        {
            // Arrange
            var mockMongoCollection = _context.SetupMongoCollection<TestRepositoryRecord>();

            mockMongoCollection
                .Setup(x => x.DeleteOneAsync(
                    It.IsAny<FilterDefinition<TestRepositoryRecord>>(), default))
                .Returns(async() =>
                {
                    await Task.Delay(QueueDelayMilliseconds);
                    throw AzureMongoServiceTestContext.GetMongoCommandAuthenticationException();
                });

            var rebuiltMongoCollection = new Mock<IMongoCollection<TestRepositoryRecord>> { Name = "Rebuilt mongo collection" };
            ArrangeDatabaseClient(_rebuiltPrimaryMongoClient, _rebuiltMongoDatabase, rebuiltMongoCollection, AzureMongoClientType.Primary);

            rebuiltMongoCollection
                .Setup(x => x.DeleteOneAsync(
                    It.IsAny<FilterDefinition<TestRepositoryRecord>>(), default))
                .ReturnsAsync(new DeleteResult.Acknowledged(1));

            // Act
            var deleteTasks = new List<Task>();
            for (var i = 0; i < NumberOfSimultaneousActions; i++)
            {
                deleteTasks.Add(_systemUnderTest.DeleteOneAsync<TestRepositoryRecord>(_context.MockRepositoryConfig.Object, _ => true));
            }
            await Task.WhenAll(deleteTasks);

            // Assert
            rebuiltMongoCollection.VerifyAll();
            PostTestVerifications();
        }

        [TestMethod]
        public async Task FindAsync_SingleAccessToSemaphore_WhenSecondaryClientFailsWithAuthenticationError()
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
                .Returns(async() =>
                {
                    await Task.Delay(QueueDelayMilliseconds);
                    throw AzureMongoServiceTestContext.GetMongoCommandAuthenticationException();
                });

             var rebuiltMongoCollection = new Mock<IMongoCollection<TestRepositoryRecord>> { Name = "Rebuilt mongo collection" };
             ArrangeDatabaseClient(_rebuiltPrimaryMongoClient, _rebuiltMongoDatabase, rebuiltMongoCollection, AzureMongoClientType.Primary);

            rebuiltMongoCollection
                .Setup(x => x.FindAsync(
                    It.IsAny<FilterDefinition<TestRepositoryRecord>>(),
                    It.IsAny<FindOptions<TestRepositoryRecord>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockCursor.Object);

            //unique to this specific test
            mockCursor
                .Setup(c => c.Current)
                .Returns(new List<TestRepositoryRecord> { record });

            // Act
            var findTasks = new List<Task>();
            for (var i = 0; i < NumberOfSimultaneousActions; i++)
            {
                findTasks.Add(_systemUnderTest.FindAsync<TestRepositoryRecord>(_context.MockRepositoryConfig.Object, _ => true, default));
            }
            await Task.WhenAll(findTasks);

            // Assert
            rebuiltMongoCollection.VerifyAll();
            PostTestVerifications();
        }


        [TestMethod]
        public async Task UpdateManyAsync_SingleAccessToSemaphore_WhenSecondaryClientFailsWithAuthenticationError()
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
                .Returns(async() =>
                {
                    await Task.Delay(QueueDelayMilliseconds);
                    throw AzureMongoServiceTestContext.GetMongoCommandAuthenticationException();
                });

            var rebuiltMongoCollection = new Mock<IMongoCollection<TestRepositoryRecord>> { Name = "Rebuilt mongo collection" };
            ArrangeDatabaseClient(_rebuiltPrimaryMongoClient, _rebuiltMongoDatabase, rebuiltMongoCollection, AzureMongoClientType.Primary);

            rebuiltMongoCollection
                .Setup(x => x.UpdateManyAsync(
                    It.IsAny<FilterDefinition<TestRepositoryRecord>>(),
                    It.IsAny<UpdateDefinition<TestRepositoryRecord>>(),null, default))
                .ReturnsAsync(new UpdateResult.Acknowledged(0, 1, "upsertedId"));

            // Act
            var updateManyTasks = new List<Task>();
            for (var i = 0; i < NumberOfSimultaneousActions; i++)
            {
                updateManyTasks.Add(_systemUnderTest.UpdateManyAsync(_context.MockRepositoryConfig.Object, _ => true, updateDefinition));
            }
            await Task.WhenAll(updateManyTasks);

            // Assert
            rebuiltMongoCollection.VerifyAll();
            PostTestVerifications();
        }


        [TestMethod]
        public async Task InsertOneDocumentAsync_SingleAccessToSemaphore_WhenSecondaryClientFailsWithAuthenticationError()
        {
            // Arrange
            var record = new BsonDocument();
            var mockMongoCollection = _context.SetupMongoCollection<BsonDocument>();

            mockMongoCollection
                .Setup(x => x.InsertOneAsync(record, null, default))
                .Returns(async() =>
                {
                    await Task.Delay(QueueDelayMilliseconds);
                    throw AzureMongoServiceTestContext.GetMongoCommandAuthenticationException();
                });

            var rebuiltMongoCollection = new Mock<IMongoCollection<BsonDocument>> { Name = "Rebuilt mongo collection" };
            ArrangeDatabaseClient(_rebuiltPrimaryMongoClient, _rebuiltMongoDatabase, rebuiltMongoCollection, AzureMongoClientType.Primary);

            rebuiltMongoCollection
                .Setup(x => x.InsertOneAsync(record, null, default))
                .Returns(Task.CompletedTask);

            // Act
            var insertOneDocumentTasks = new List<Task>();
            for (var i = 0; i < NumberOfSimultaneousActions; i++)
            {
                insertOneDocumentTasks.Add(_systemUnderTest.InsertOneDocumentAsync(_context.MockRepositoryConfig.Object, record));
            }
            await Task.WhenAll(insertOneDocumentTasks);

            // Assert
            rebuiltMongoCollection.VerifyAll();
            PostTestVerifications();
        }

        [TestMethod]
        public async Task UpdateOneDocumentAsync_SingleAccessToSemaphore_WhenSecondaryClientFailsWithAuthenticationError()
        {
            // Arrange
            var mockMongoCollection = _context.SetupMongoCollection<BsonDocument>();
            var updateDefinition = Builders<BsonDocument>.Update.Set("record1", new BsonDocument());

            mockMongoCollection
                .Setup(x => x.UpdateOneAsync(
                    It.IsAny<FilterDefinition<BsonDocument>>(),
                    It.IsAny<UpdateDefinition<BsonDocument>>(), null, default))
                .Returns(async() =>
                {
                    await Task.Delay(QueueDelayMilliseconds);
                    throw AzureMongoServiceTestContext.GetMongoCommandAuthenticationException();
                });

            var rebuiltMongoCollection = new Mock<IMongoCollection<BsonDocument>> { Name = "Rebuilt mongo collection" };
            ArrangeDatabaseClient(_rebuiltPrimaryMongoClient, _rebuiltMongoDatabase, rebuiltMongoCollection, AzureMongoClientType.Primary);

            rebuiltMongoCollection
                .Setup(x => x.UpdateOneAsync(
                    It.IsAny<FilterDefinition<BsonDocument>>(),
                    It.IsAny<UpdateDefinition<BsonDocument>>(), null, default))
                .ReturnsAsync(new UpdateResult.Acknowledged(0, 1, "upsertedId"));

            // Act
            var updateOneDocumentTasks = new List<Task>();
            for (var i = 0; i < NumberOfSimultaneousActions; i++)
            {
                updateOneDocumentTasks.Add(_systemUnderTest.UpdateOneDocumentAsync(_context.MockRepositoryConfig.Object, null, updateDefinition));
            }
            await Task.WhenAll(updateOneDocumentTasks);

            // Assert
            rebuiltMongoCollection.VerifyAll();
            PostTestVerifications();
       }

        [TestMethod]
        public async Task FindOneAndUpdateDocumentAsync_SingleAccessToSemaphore_WhenSecondaryClientFailsWithAuthenticationError()
        {
            // Arrange
            var mockMongoCollection = _context.SetupMongoCollection<BsonDocument>();
            var record = new BsonDocument();
            var updateDefinition = Builders<BsonDocument>.Update.Set("record1", record);

            mockMongoCollection
                .Setup(x => x.FindOneAndUpdateAsync(
                    It.IsAny<FilterDefinition<BsonDocument>>(),
                    It.IsAny<UpdateDefinition<BsonDocument>>(),
                    It.IsAny<FindOneAndUpdateOptions<BsonDocument, BsonDocument>>(),
                    It.IsAny<CancellationToken>()))
                .Returns(async() =>
                {
                    await Task.Delay(QueueDelayMilliseconds);
                    throw AzureMongoServiceTestContext.GetMongoCommandAuthenticationException();
                });

            var rebuiltMongoCollection = new Mock<IMongoCollection<BsonDocument>> { Name = "Rebuilt mongo collection" };
            ArrangeDatabaseClient(_rebuiltPrimaryMongoClient, _rebuiltMongoDatabase, rebuiltMongoCollection, AzureMongoClientType.Primary);

            rebuiltMongoCollection
                .Setup(x => x.FindOneAndUpdateAsync(
                        It.IsAny<FilterDefinition<BsonDocument>>(),
                        It.IsAny<UpdateDefinition<BsonDocument>>(),
                        It.IsAny<FindOneAndUpdateOptions<BsonDocument, BsonDocument>>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(record);

            // Act
            var findAndUpdateTasks = new List<Task>();
            for (var i = 0; i < NumberOfSimultaneousActions; i++)
            {
                findAndUpdateTasks.Add(_systemUnderTest.FindOneAndUpdateDocumentAsync(_context.MockRepositoryConfig.Object, record, updateDefinition));
            }
            await Task.WhenAll(findAndUpdateTasks);

            // Assert
            rebuiltMongoCollection.VerifyAll();
            PostTestVerifications();
        }

        [TestMethod]
        public async Task FindFirstDocument_SingleAccessToSemaphore_WhenSecondaryClientFailsWithAuthenticationError()
        {
            // Arrange
            var mockMongoCollection = _context.SetupMongoCollection<BsonDocument>();
            var record = new BsonDocument();
            var mockBsonDocumentCursor = _context.ArrangeBsonDocumentCursor(record, NumberOfSimultaneousActions);

            mockMongoCollection
                .Setup(x => x.FindAsync(
                    It.IsAny<FilterDefinition<BsonDocument>>(),
                    It.IsAny<FindOptions<BsonDocument>>(),
                    It.IsAny<CancellationToken>()))
                 .Returns(async () =>
                 {
                     await Task.Delay(QueueDelayMilliseconds);
                     throw AzureMongoServiceTestContext.GetMongoCommandAuthenticationException();
                 });

            var rebuiltMongoCollection = new Mock<IMongoCollection<BsonDocument>> { Name = "Rebuilt mongo collection" };
            ArrangeDatabaseClient(_rebuiltPrimaryMongoClient, _rebuiltMongoDatabase, rebuiltMongoCollection, AzureMongoClientType.Primary);

            rebuiltMongoCollection
                .Setup(x => x.FindAsync(
                    It.IsAny<FilterDefinition<BsonDocument>>(),
                    It.IsAny<FindOptions<BsonDocument>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockBsonDocumentCursor.Object);

            // Act
            var findFirstTasks = new List<Task>();
            for (var i = 0; i < NumberOfSimultaneousActions; i++)
            {
                findFirstTasks.Add(_systemUnderTest.FindFirstDocument(_context.MockRepositoryConfig.Object, record));
            }
            await Task.WhenAll(findFirstTasks);

            // Assert
            rebuiltMongoCollection.VerifyAll();
            PostTestVerifications();
        }

        [TestMethod]
        public async Task DeleteOneDocumentAsync_SingleAccessToSemaphore_WhenSecondaryClientFailsWithAuthenticationError()
        {
            // Arrange
            var mockMongoCollection = _context.SetupMongoCollection<BsonDocument>();
            var record = new BsonDocument();

            mockMongoCollection
                .Setup(x => x.DeleteOneAsync(
                    It.IsAny<FilterDefinition<BsonDocument>>(), default))
                .Returns(async() =>
                {
                    await Task.Delay(QueueDelayMilliseconds);
                    throw AzureMongoServiceTestContext.GetMongoCommandAuthenticationException();
                });

            var rebuiltMongoCollection = new Mock<IMongoCollection<BsonDocument>> { Name = "Rebuilt mongo collection" };
            ArrangeDatabaseClient(_rebuiltPrimaryMongoClient, _rebuiltMongoDatabase, rebuiltMongoCollection, AzureMongoClientType.Primary);

            rebuiltMongoCollection
                .Setup(x => x.DeleteOneAsync(
                    It.IsAny<FilterDefinition<BsonDocument>>(), default))
                .ReturnsAsync(new DeleteResult.Acknowledged(1));

            // Act
            var deleteOneDocumentTasks = new List<Task>();
            for (var i = 0; i < NumberOfSimultaneousActions; i++)
            {
                deleteOneDocumentTasks.Add(_systemUnderTest.DeleteOneDocumentAsync(_context.MockRepositoryConfig.Object, record));
            }
            await Task.WhenAll(deleteOneDocumentTasks);

            // Assert
            rebuiltMongoCollection.VerifyAll();
            PostTestVerifications();
        }

        private void PostTestVerifications()
        {
            _rebuiltPrimaryMongoClient.VerifyAll();
            _context.MockSecondaryMongoClient.VerifyAll();
            _context.MockLogger.VerifyLogger(LogLevel.Information, "Rebuilding primary client", Times.Once());
            _context.MockLogger.VerifyLogger(LogLevel.Information, "primary:True secondary:True activeClient:primary", Times.Between(1, NumberOfSimultaneousActions, Range.Inclusive));
            _context.MockLogger.VerifyLogger(LogLevel.Information, "DoWithRecoveryAsync - Successful action after recovery with Primary client", Times.Exactly(NumberOfSimultaneousActions));
        }

        private static void ArrangeDatabaseClient<T>(Mock<INamedMongoClient> mockMongoClient, Mock<IMongoDatabase> mockMongoDatabase, IMock<IMongoCollection<T>> mockMongoCollection, AzureMongoClientType clientType)
        {
            mockMongoClient
                .Setup(x => x.GetDatabase(AzureMongoServiceTestContext.DatabaseName, It.IsAny<MongoDatabaseSettings>()))
                .Returns(mockMongoDatabase.Object);

             mockMongoClient
                 .SetupGet(x => x.ClientType).Returns(clientType);

            mockMongoDatabase
                .Setup(x => x.GetCollection<T>(AzureMongoServiceTestContext.CollectionName, It.IsAny<MongoCollectionSettings>()))
                .Returns(mockMongoCollection.Object);

            var rebuiltCursor = new Mock<IAsyncCursor<string>>();
            rebuiltCursor
                .Setup(c => c.Current)
                .Returns(new List<string> { "collection" });

            mockMongoDatabase
                .Setup(x => x.ListCollectionNamesAsync(null, default))
                .ReturnsAsync(rebuiltCursor.Object);
        }

        public void Dispose()
        {
            _systemUnderTest?.Dispose();
        }
    }
}
