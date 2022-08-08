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
    public class MongoRepositoryCountTests
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
        public async Task Count_Found()
        {
            // Arrange
            _mongoClientWrapperMock
                .Setup(x => x.CountAsync<TestRepositoryRecord>(
                    _repositoryConfiguration,
                    It.IsAny<Expression<Func<TestRepositoryRecord, bool>>>()))
                .ReturnsAsync(5);

            // Act
            var result = await _systemUnderTest.Count(_ => true, "recordName");

            // Assert
            _mongoClientWrapperMock.VerifyAll();
            result.Should().BeOfType<RepositoryCountResult.Found>();
        }

        [TestMethod]
        public async Task Count_RepositoryThrowsException_ReturnsRepositoryError()
        {
            // Arrange
            _mongoClientWrapperMock
                .Setup(x => x.CountAsync<TestRepositoryRecord>(
                    _repositoryConfiguration,
                    It.IsAny<Expression<Func<TestRepositoryRecord, bool>>>()))
                .ThrowsAsync(new MongoException("Test"));

            // Act
            var result = await _systemUnderTest.Count(_ => true, "recordName");

            // Assert
            _mongoClientWrapperMock.VerifyAll();
            result.Should().BeOfType<RepositoryCountResult.RepositoryError>();
        }
    }
}