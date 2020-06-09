using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using Moq;

namespace NHSOnline.Backend.Repository.UnitTests
{
    [TestClass]
    public class MongoRepositoryCreateOrUpdateTests
    {
        private MongoRepository<IMongoConfiguration, TestRepositoryRecord> _systemUnderTest;
        private Mock<IMongoCollection<TestRepositoryRecord>> _mongoCollectionMock;

        [TestInitialize]
        public void TestInitialize()
        {
            _mongoCollectionMock = new Mock<IMongoCollection<TestRepositoryRecord>>();
            _systemUnderTest = MongoRepositoryUnitTestSupport.CreateRepository(_mongoCollectionMock);
        }

        [TestMethod]
        public async Task CreateOrUpdate_Success()
        {
            // Arrange
            var record = new TestRepositoryRecord();
            _mongoCollectionMock.Setup(x => x.ReplaceOneAsync(
                    It.IsAny<FilterDefinition<TestRepositoryRecord>>(),
                    record,
                    It.IsAny<ReplaceOptions>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ReplaceOneResult.Acknowledged(0, 1, "upsertedId"));

            // Act
            var result = await _systemUnderTest.CreateOrUpdate(_ => true, record, "recordName");

            // Assert
            _mongoCollectionMock.Verify();
            result.Should().BeOfType<RepositoryCreateResult<TestRepositoryRecord>.Created>();
        }

        [TestMethod]
        public async Task CreateOrUpdate_Unacknowledged_ReturnsRepositoryError()
        {
            // Arrange
            var record = new TestRepositoryRecord();
            _mongoCollectionMock.Setup(x => x.ReplaceOneAsync(
                    It.IsAny<FilterDefinition<TestRepositoryRecord>>(),
                    record,
                    It.IsAny<ReplaceOptions>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(ReplaceOneResult.Unacknowledged.Instance);

            // Act
            var result = await _systemUnderTest.CreateOrUpdate(_ => true, record, "recordName");

            // Assert
            _mongoCollectionMock.Verify();
            result.Should().BeOfType<RepositoryCreateResult<TestRepositoryRecord>.RepositoryError>();
        }

        [TestMethod]
        public async Task CreateOrUpdate_RepositoryThrowsException_ReturnsRepositoryError()
        {
            // Arrange
            var record = new TestRepositoryRecord();
            _mongoCollectionMock.Setup(x => x.ReplaceOneAsync(
                It.IsAny<FilterDefinition<TestRepositoryRecord>>(),
                record,
                It.IsAny<ReplaceOptions>(),
                It.IsAny<CancellationToken>())).ThrowsAsync(new MongoException("Test"));

            // Act
            var result = await _systemUnderTest.CreateOrUpdate(_ => true, record, "recordName");

            // Assert
            _mongoCollectionMock.VerifyAll();
            result.Should().BeOfType<RepositoryCreateResult<TestRepositoryRecord>.RepositoryError>();
        }
    }
}