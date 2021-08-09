using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using Moq;

namespace NHSOnline.Backend.Repository.UnitTests
{
    [TestClass]
    public class MongoRepositoryCreateTests
    {
        private MongoRepository<IRepositoryConfiguration, TestRepositoryRecord> _systemUnderTest;
        private TestRepositoryConfiguration _repositoryConfiguration;

        private Mock<IMongoClientService> _mongoClientWrapperMock;

        [TestInitialize]
        public void TestInitialize()
        {
            _mongoClientWrapperMock = new Mock<IMongoClientService>();

            _repositoryConfiguration = new TestRepositoryConfiguration
            {
                DatabaseName = "MockDatabaseName",
                CollectionName = "MockCollectionName"
            };

            _systemUnderTest = new MongoRepository<IRepositoryConfiguration, TestRepositoryRecord>(
                _mongoClientWrapperMock.Object,
                _repositoryConfiguration,
                new Mock<ILogger<MongoRepository<IRepositoryConfiguration, TestRepositoryRecord>>>().Object);
        }

        [TestMethod]
        public async Task Create_Created()
        {
            // Arrange
            var record = new TestRepositoryRecord();
            _mongoClientWrapperMock
                .Setup(x => x.InsertOneAsync(_repositoryConfiguration, record))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _systemUnderTest.Create(record, "recordName");

            // Assert
            _mongoClientWrapperMock.VerifyAll();
            result.Should().BeOfType<RepositoryCreateResult<TestRepositoryRecord>.Created>();
        }

        [TestMethod]
        public async Task Create_RepositoryThrowsException_ReturnsRepositoryError()
        {
            // Arrange
            var record = new TestRepositoryRecord();
            _mongoClientWrapperMock
                .Setup(x => x.InsertOneAsync(_repositoryConfiguration, record))
                .ThrowsAsync(new MongoException("Test"));

            // Act
            var result = await _systemUnderTest.Create(record, "recordName");

            // Assert
            _mongoClientWrapperMock.VerifyAll();
            result.Should().BeOfType<RepositoryCreateResult<TestRepositoryRecord>.RepositoryError>();
        }
    }
}
