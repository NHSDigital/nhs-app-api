using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using Moq;

namespace NHSOnline.Backend.Repository.UnitTests
{
    [TestClass]
    public class MongoRepositoryCreateTests
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
        public async Task Create_Created()
        {
            // Arrange
            var record = new TestRepositoryRecord();
            _mongoCollectionMock.Setup(x => x.InsertOneAsync(record, null, default))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _systemUnderTest.Create(record, "recordName");

            // Assert
            _mongoCollectionMock.VerifyAll();
            result.Should().BeOfType<RepositoryCreateResult<TestRepositoryRecord>.Created>();
        }

        [TestMethod]
        public async Task Create_RepositoryThrowsException_ReturnsRepositoryError()
        {
            // Arrange
            var record = new TestRepositoryRecord();
            _mongoCollectionMock.Setup(x => x.InsertOneAsync(record, null, default))
                .ThrowsAsync(new MongoException("Test"));

            // Act
            var result = await _systemUnderTest.Create(record, "recordName");

            // Assert
            _mongoCollectionMock.VerifyAll();
            result.Should().BeOfType<RepositoryCreateResult<TestRepositoryRecord>.RepositoryError>();
        }
    }
}