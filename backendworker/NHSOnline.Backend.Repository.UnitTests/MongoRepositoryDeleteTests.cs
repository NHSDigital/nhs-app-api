using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using Moq;

namespace NHSOnline.Backend.Repository.UnitTests
{
    [TestClass]
    public class MongoRepositoryDeleteTests
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
        public async Task Delete_Success()
        {
            // Arrange
            _mongoClientWrapperMock
                .Setup(x => x.DeleteOneAsync(
                    _repositoryConfiguration,
                    It.IsAny<Expression<Func<TestRepositoryRecord, bool>>>()))
                .ReturnsAsync(new DeleteResult.Acknowledged(1));

            // Act
            var result = await _systemUnderTest.Delete(_ => true, "recordName");

            // Assert
            _mongoClientWrapperMock.VerifyAll();
            result.Should().BeOfType<RepositoryDeleteResult<TestRepositoryRecord>.Deleted>();
        }

        [TestMethod]
        public async Task Delete_NotFound()
        {
            // Arrange
            _mongoClientWrapperMock
                .Setup(x => x.DeleteOneAsync(
                    _repositoryConfiguration,
                    It.IsAny<Expression<Func<TestRepositoryRecord, bool>>>()))
                .ReturnsAsync(new DeleteResult.Acknowledged(0));

            // Act
            var result = await _systemUnderTest.Delete(_ => true, "recordName");

            // Assert
            _mongoClientWrapperMock.VerifyAll();
            result.Should().BeOfType<RepositoryDeleteResult<TestRepositoryRecord>.NotFound>();
        }

        [TestMethod]
        public async Task Delete_Unacknowledged_ReturnsRepositoryError()
        {
            // Arrange
            _mongoClientWrapperMock
                .Setup(x => x.DeleteOneAsync(
                    _repositoryConfiguration,
                    It.IsAny<Expression<Func<TestRepositoryRecord, bool>>>())).ReturnsAsync(DeleteResult.Unacknowledged.Instance);

            // Act
            var result = await _systemUnderTest.Delete(_ => true, "recordName");

            // Assert
            _mongoClientWrapperMock.Verify();
            result.Should().BeOfType<RepositoryDeleteResult<TestRepositoryRecord>.RepositoryError>();
        }

        [TestMethod]
        public async Task Delete_RepositoryThrowsException_ReturnsRepositoryError()
        {
            // Arrange
            _mongoClientWrapperMock
                .Setup(x => x.DeleteOneAsync(
                    _repositoryConfiguration,
                    It.IsAny<Expression<Func<TestRepositoryRecord, bool>>>())).ThrowsAsync(new MongoException("Test"));

            // Act
            var result = await _systemUnderTest.Delete(_ => true, "recordName");

            // Assert
            _mongoClientWrapperMock.VerifyAll();
            result.Should().BeOfType<RepositoryDeleteResult<TestRepositoryRecord>.RepositoryError>();
        }
    }
}
