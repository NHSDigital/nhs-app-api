using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Clusters;
using MongoDB.Driver.Core.Connections;
using MongoDB.Driver.Core.Servers;
using Moq;
using NHSOnline.Backend.Support.AzureManagement;

namespace NHSOnline.Backend.Repository.UnitTests
{
    internal sealed class AzureMongoServiceTestContext
    {
        internal Mock<ILogger<AzureMongoService>> MockLogger { get; } = new Mock<ILogger<AzureMongoService>>();
        internal Mock<INamedMongoClient> MockPrimaryMongoClient { get; } = new Mock<INamedMongoClient> { Name = "Primary mongo client" };
        internal Mock<INamedMongoClient> MockSecondaryMongoClient { get; } = new Mock<INamedMongoClient> { Name = "Secondary mongo client" };
        internal Mock<IMongoDatabase> MockMongoDatabase { get; } = new Mock<IMongoDatabase> { Name = "Mongo database" };
        internal Mock<IRepositoryConfiguration> MockRepositoryConfig { get; }  = new Mock<IRepositoryConfiguration>();
        internal Mock<IAsyncCursor<string>> MockCursor { get; } = new Mock<IAsyncCursor<string>>();
        internal Mock<IAzureKeyVaultService> MockAzureKeyVaultService { get; } = new Mock<IAzureKeyVaultService>();
        internal Mock<IMongoClientCreator> MockMongoClientCreator { get; } = new Mock<IMongoClientCreator>();
        internal Mock<IConfiguration> MockConfiguration { get; } = new Mock<IConfiguration>();
        internal Mock<IAzureMongoClient> MockAzureMongoClient { get; } = new Mock<IAzureMongoClient>();

        internal ConnectionStringResponse NewConnectionStrings { get; private set; }

        private const string PrimaryConnectionString = "mongodb://primary";
        private const string SecondaryConnectionString = "mongodb://secondary";

        internal const string NewPrimaryConnectionString = "mongodb://primary-new";
        internal const string NewSecondaryConnectionString = "mongodb://secondary-new";

        internal const string DatabaseName = "DatabaseName";
        internal const string CollectionName = "CollectionName";

        public AzureMongoServiceTestContext()
        {
            MockConfiguration.Setup(x => x["MONGO_DATABASE_NAME"]).Returns(DatabaseName);

            MockRepositoryConfig
                .Setup(x => x.DatabaseName)
                .Returns(DatabaseName);

            MockRepositoryConfig
                .Setup(x => x.CollectionName)
                .Returns(CollectionName);

            var connectionStrings = new ConnectionStringResponse
            {
                PrimaryConnectionString = PrimaryConnectionString,
                SecondaryConnectionString = SecondaryConnectionString,
            };

            NewConnectionStrings = new ConnectionStringResponse
            {
                PrimaryConnectionString = NewPrimaryConnectionString,
                SecondaryConnectionString = NewSecondaryConnectionString
            };

            MockPrimaryMongoClient
                .Setup(x => x.GetDatabase(DatabaseName, It.IsAny<MongoDatabaseSettings>()))
                .Returns(MockMongoDatabase.Object);

            MockPrimaryMongoClient
                .SetupGet(x => x.ClientType).Returns(AzureMongoClientType.Primary);

            MockSecondaryMongoClient
                .Setup(x => x.GetDatabase(DatabaseName, It.IsAny<MongoDatabaseSettings>()))
                .Returns(MockMongoDatabase.Object);

            MockSecondaryMongoClient
                .SetupGet(x => x.ClientType).Returns(AzureMongoClientType.Secondary);

            MockAzureKeyVaultService
                .Setup(x => x.GetConnectionStrings())
                .ReturnsAsync(connectionStrings);

            MockMongoClientCreator
                .Setup(x => x.CreatePrimaryMongoClient(PrimaryConnectionString))
                .Returns(MockPrimaryMongoClient.Object);

            MockMongoClientCreator
                .Setup(x => x.CreateSecondaryMongoClient(SecondaryConnectionString))
                .Returns(MockSecondaryMongoClient.Object);
        }

        internal void ArrangeHealthChecks()
        {
            MockMongoDatabase
                .Setup(x => x.ListCollectionNamesAsync(null, default))
                .ReturnsAsync(MockCursor.Object);

            MockCursor
                .Setup(c => c.Current)
                .Returns(new List<string> { "collection" });
        }

        internal void ArrangeDatabaseClient<T>(Mock<IMongoClient> mockMongoClient, Mock<IMongoDatabase> mockMongoDatabase, IMock<IMongoCollection<T>> mockMongoCollection)
        {
            mockMongoClient
                .Setup(x => x.GetDatabase(DatabaseName, It.IsAny<MongoDatabaseSettings>()))
                .Returns(mockMongoDatabase.Object);

            mockMongoDatabase
                .Setup(x => x.GetCollection<T>(CollectionName, It.IsAny<MongoCollectionSettings>()))
                .Returns(mockMongoCollection.Object);

            var rebuiltCursor = new Mock<IAsyncCursor<string>>();
            rebuiltCursor
                .Setup(c => c.Current)
                .Returns(new List<string> { "collection" });

            mockMongoDatabase
                .Setup(x => x.ListCollectionNamesAsync(null, default))
                .ReturnsAsync(rebuiltCursor.Object);
        }

        internal static IEnumerable<object[]> ExceptionData
        {
            get
            {
                yield return new object[] { GetMongoCommandAuthenticationException() };
                yield return new object[] { GetMongoAuthenticationException() };
                yield return new object[] { CreateMongoWriteException() };
            }
        }

        internal static Exception GetMongoCommandAuthenticationException()
        {
            return new MongoCommandException(
                new ConnectionId(new ServerId(new ClusterId(), new UriEndPoint(new Uri("http://google.com")))),
                "error",
                null,
                new BsonDocument(new BsonElement("code", BsonValue.Create(13))));
        }

        private static Exception GetMongoAuthenticationException()
        {
            return new MongoAuthenticationException(
                 new ConnectionId(new ServerId(new ClusterId(), new UriEndPoint(new Uri("http://google.com")))),
                 "error");
        }

        private static MongoWriteException CreateMongoWriteException()
        {
            var connectionId = new ConnectionId(new ServerId(new ClusterId(), new UriEndPoint(new Uri("http://google.com"))));
            var writeConcernError = CreateWriteConcernError();
            var writeError = CreateWriteError(new ServerErrorCategory());
            return new MongoWriteException(connectionId, writeError, writeConcernError, new TimeoutException());
        }

        private static WriteError CreateWriteError(ServerErrorCategory serverErrorCategory)
        {
            var ctor = typeof (WriteError).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)[0];
            var writeError = (WriteError)ctor.Invoke(new object[] {serverErrorCategory, 13, "writeError", new BsonDocument("details", "writeError")});
            return writeError;
        }

        private static WriteConcernError CreateWriteConcernError()
        {
            var ctor = typeof(WriteConcernError).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)[0];
            return (WriteConcernError)ctor.Invoke(new object[] { 1, "writeConcernError","message", new BsonDocument("details", "writeConcernError"), new List<string>() });
        }

        internal Mock<IAsyncCursor<BsonDocument>> ArrangeBsonDocumentCursor(BsonDocument record, int limit = 1)
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

        internal Mock<IMongoCollection<T>> SetupMongoCollection<T>()
        {
            var mockMongoCollection = new Mock<IMongoCollection<T>> { Name = "Mongo Collection" };

            MockMongoDatabase
                .Setup(x => x.GetCollection<T>(CollectionName, It.IsAny<MongoCollectionSettings>()))
                .Returns(mockMongoCollection.Object);

            return mockMongoCollection;
        }
    }
}
