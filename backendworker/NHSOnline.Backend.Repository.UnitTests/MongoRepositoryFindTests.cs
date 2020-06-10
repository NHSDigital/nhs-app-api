using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using Moq;
using Moq.Language.Flow;

namespace NHSOnline.Backend.Repository.UnitTests
{
    [TestClass]
    public class MongoRepositoryFindTests
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
        public async Task Find_WhenRecordsExist_Found()
        {
            // Arrange
            var record = new TestRepositoryRecord();
            var cursorMock = CreateCursorMockFind(new[] { record });
            SetupFind().ReturnsAsync(cursorMock.Object);

            // Act
            var result = await _systemUnderTest.Find(_ => true, "recordName");

            // Assert
            _mongoCollectionMock.VerifyAll();
            result.Should().BeOfType<RepositoryFindResult<TestRepositoryRecord>.Found>()
                .Subject.Records.Should().BeEquivalentTo(new[] { record });
        }

        [TestMethod]
        public async Task Find_WhenNoRecordsExist_NotFound()
        {
            // Arrange
            var cursorMock = CreateCursorMockFindNone();
            SetupFind().ReturnsAsync(cursorMock.Object);

            // Act
            var result = await _systemUnderTest.Find(_ => true, "recordName");

            // Assert
            _mongoCollectionMock.VerifyAll();
            result.Should().BeOfType<RepositoryFindResult<TestRepositoryRecord>.NotFound>();
        }

        [TestMethod]
        public async Task Find_RepositoryThrowsException_ReturnsRepositoryError()
        {
            // Arrange
            SetupFind().ThrowsAsync(new MongoException("Test"));

            // Act
            var result = await _systemUnderTest.Find(_ => true, "recordName");

            // Assert
            _mongoCollectionMock.VerifyAll();
            result.Should().BeOfType<RepositoryFindResult<TestRepositoryRecord>.RepositoryError>();
        }

        private ISetup<IMongoCollection<TestRepositoryRecord>, Task<IAsyncCursor<TestRepositoryRecord>>> SetupFind()
        {
           return _mongoCollectionMock.Setup(x => x.FindAsync(It.IsAny<FilterDefinition<TestRepositoryRecord>>(),
                It.IsAny<FindOptions<TestRepositoryRecord, TestRepositoryRecord>>(), It.IsAny<CancellationToken>()));
        }

        private static Mock<IAsyncCursor<TestRepositoryRecord>> CreateCursorMockFindNone()
        {
            var cursorMock = new Mock<IAsyncCursor<TestRepositoryRecord>>();
            cursorMock.Setup(x => x.MoveNext(It.IsAny<CancellationToken>())).Returns(false);
            return cursorMock;
        }

        private static Mock<IAsyncCursor<TestRepositoryRecord>> CreateCursorMockFind(IEnumerable<TestRepositoryRecord> values)
        {
            var cursorMock = new Mock<IAsyncCursor<TestRepositoryRecord>>();
            var mockReturn = true;

            cursorMock.Setup(x => x.MoveNext(It.IsAny<CancellationToken>())).Returns(() => mockReturn)
                .Callback<CancellationToken>(t => mockReturn = false);

            cursorMock.SetupGet(x => x.Current).Returns(values);
            return cursorMock;
        }
    }
}