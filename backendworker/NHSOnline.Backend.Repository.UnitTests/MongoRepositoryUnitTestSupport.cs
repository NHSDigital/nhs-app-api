using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Moq;

namespace NHSOnline.Backend.Repository.UnitTests
{
    public static class MongoRepositoryUnitTestSupport
    {
        public static MongoRepository<IRepositoryConfiguration, TestRepositoryRecord> CreateRepository(
            Mock<IMongoCollection<TestRepositoryRecord>> mongoCollectionMock)
        {
            var databaseName = "MockDatabaseName";
            var collectionName = "MockCollectionName";

            var mongoDatabaseMock = new Mock<IMongoDatabase>();
            mongoDatabaseMock.Setup(x => x.GetCollection<TestRepositoryRecord>(collectionName, null))
                .Returns(mongoCollectionMock.Object);

            var mockMongoClient = new Mock<IMongoClientService>();

            var mongoConfiguration = new TestRepositoryConfiguration
            {
                DatabaseName = databaseName,
                CollectionName = collectionName
            };

            return new MongoRepository<IRepositoryConfiguration, TestRepositoryRecord>(
                mockMongoClient.Object,
                mongoConfiguration,
                new Mock<ILogger<MongoRepository<IRepositoryConfiguration, TestRepositoryRecord>>>().Object);
        }
    }

    public class TestRepositoryConfiguration : IRepositoryConfiguration
    {
        public string DatabaseName { get; set; }
        public string CollectionName { get; set; }
        public void Validate()
        {
            throw new System.NotImplementedException();
        }
    }
}