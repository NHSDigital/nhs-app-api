using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using Moq;

namespace NHSOnline.Backend.Repository.UnitTests
{
    [TestClass]
    public class MongoRepositoryDeleteTests
    {
        private MongoRepository<IRepositoryConfiguration, TestRepositoryRecord> _systemUnderTest;
        private Mock<IMongoCollection<TestRepositoryRecord>> _mongoCollectionMock;

        [TestInitialize]
        public void TestInitialize()
        {
            _mongoCollectionMock = new Mock<IMongoCollection<TestRepositoryRecord>>();
            _systemUnderTest = MongoRepositoryUnitTestSupport.CreateRepository(_mongoCollectionMock);
        }

        [TestMethod]
        public async Task Delete_Success()
        {
            // Arrange
            _mongoCollectionMock
                .Setup(x => x.DeleteOneAsync(It.IsAny<FilterDefinition<TestRepositoryRecord>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new DeleteResult.Acknowledged(1));

            // Act
            var result = await _systemUnderTest.Delete(_ => true, "recordName");

            // Assert
            _mongoCollectionMock.VerifyAll();
            result.Should().BeOfType<RepositoryDeleteResult<TestRepositoryRecord>.Deleted>();
        }
        
        [TestMethod]
        public async Task Delete_NotFound()
        {
            // Arrange
            _mongoCollectionMock
                .Setup(x => x.DeleteOneAsync(It.IsAny<FilterDefinition<TestRepositoryRecord>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new DeleteResult.Acknowledged(0));

            // Act
            var result = await _systemUnderTest.Delete(_ => true, "recordName");

            // Assert
            _mongoCollectionMock.VerifyAll();
            result.Should().BeOfType<RepositoryDeleteResult<TestRepositoryRecord>.NotFound>();
        }

        [TestMethod]
        public async Task Delete_Unacknowledged_ReturnsRepositoryError()
        {
            // Arrange
            _mongoCollectionMock
                .Setup(x => x.DeleteOneAsync(It.IsAny<FilterDefinition<TestRepositoryRecord>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(DeleteResult.Unacknowledged.Instance);

            // Act
            var result = await _systemUnderTest.Delete(_ => true, "recordName");

            // Assert
            _mongoCollectionMock.Verify();
            result.Should().BeOfType<RepositoryDeleteResult<TestRepositoryRecord>.RepositoryError>();
        }

        [TestMethod]
        public async Task Delete_RepositoryThrowsException_ReturnsRepositoryError()
        {
            // Arrange
            _mongoCollectionMock
                .Setup(x => x.DeleteOneAsync(It.IsAny<FilterDefinition<TestRepositoryRecord>>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new MongoException("Test"));

            // Act
            var result = await _systemUnderTest.Delete(_ => true, "recordName");

            // Assert
            _mongoCollectionMock.VerifyAll();
            result.Should().BeOfType<RepositoryDeleteResult<TestRepositoryRecord>.RepositoryError>();
        }
    }
}