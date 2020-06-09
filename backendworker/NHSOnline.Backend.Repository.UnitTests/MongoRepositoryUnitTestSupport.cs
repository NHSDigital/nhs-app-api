using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Moq;

namespace NHSOnline.Backend.Repository.UnitTests
{
    public static class MongoRepositoryUnitTestSupport
    {
        public static MongoRepository<IMongoConfiguration, TestRepositoryRecord> CreateRepository(
            Mock<IMongoCollection<TestRepositoryRecord>> mongoCollectionMock)
        {
            var databaseName = "MockDatabaseName";
            var collectionName = "MockCollectionName";

            var mongoDatabaseMock = new Mock<IMongoDatabase>();
            mongoDatabaseMock.Setup(x => x.GetCollection<TestRepositoryRecord>(collectionName, null))
                .Returns(mongoCollectionMock.Object);

            var mockMongoClient = new Mock<IApiMongoClient<IMongoConfiguration>>();
            mockMongoClient.Setup(x => x.GetDatabase(databaseName, null))
                .Returns(mongoDatabaseMock.Object);

            var mongoConfiguration = new MongoConfiguration("connectionString", databaseName, collectionName);

            return new MongoRepository<IMongoConfiguration, TestRepositoryRecord>(
                mockMongoClient.Object,
                mongoConfiguration,
                new Mock<ILogger<MongoRepository<IMongoConfiguration, TestRepositoryRecord>>>().Object);
        }
    }
}