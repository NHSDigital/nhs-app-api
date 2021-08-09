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
    public class MongoRepositoryCreateOrUpdateTests
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
        public async Task CreateOrUpdate_Success()
        {
            // Arrange
            var record = new TestRepositoryRecord();

            _mongoClientWrapperMock
                .Setup(x => x.ReplaceOneAsync(
                    _repositoryConfiguration,
                    It.IsAny<Expression<Func<TestRepositoryRecord, bool>>>(),
                    record,
                    It.IsAny<ReplaceOptions>()))
                .ReturnsAsync(new ReplaceOneResult.Acknowledged(0, 1, "upsertedId"));

            // Act
            var result = await _systemUnderTest.CreateOrUpdate(_ => true, record, "recordName");

            // Assert
            _mongoClientWrapperMock.Verify();
            result.Should().BeOfType<RepositoryCreateResult<TestRepositoryRecord>.Created>();
        }

        [TestMethod]
        public async Task CreateOrUpdate_Unacknowledged_ReturnsRepositoryError()
        {
            // Arrange
            var record = new TestRepositoryRecord();

            _mongoClientWrapperMock
                .Setup(x => x.ReplaceOneAsync(
                    _repositoryConfiguration,
                    It.IsAny<Expression<Func<TestRepositoryRecord, bool>>>(),
                    record,
                    It.IsAny<ReplaceOptions>()))
                .ReturnsAsync(ReplaceOneResult.Unacknowledged.Instance);

            // Act
            var result = await _systemUnderTest.CreateOrUpdate(_ => true, record, "recordName");

            // Assert
            _mongoClientWrapperMock.Verify();
            result.Should().BeOfType<RepositoryCreateResult<TestRepositoryRecord>.RepositoryError>();
        }

        [TestMethod]
        public async Task CreateOrUpdate_RepositoryThrowsException_ReturnsRepositoryError()
        {
            // Arrange
            var record = new TestRepositoryRecord();

            _mongoClientWrapperMock
                .Setup(x => x.ReplaceOneAsync(
                    _repositoryConfiguration,
                It.IsAny<Expression<Func<TestRepositoryRecord, bool>>>(),
                    record,
                    It.IsAny<ReplaceOptions>()))
                .ThrowsAsync(new MongoException("Test"));

            // Act
            var result = await _systemUnderTest.CreateOrUpdate(_ => true, record, "recordName");

            // Assert
            _mongoClientWrapperMock.VerifyAll();
            result.Should().BeOfType<RepositoryCreateResult<TestRepositoryRecord>.RepositoryError>();
        }
    }
}
