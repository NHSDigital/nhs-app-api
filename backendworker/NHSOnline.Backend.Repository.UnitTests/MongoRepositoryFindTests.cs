using System;
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
            IEnumerable<TestRepositoryRecord>[] values = new[] { new[] { record } };
            using var cursorMock = new MockAsyncCursor<TestRepositoryRecord>(values);
            SetupFind().ReturnsAsync(cursorMock);

            // Act
            var result = await _systemUnderTest.Find(_ => true, "recordName");

            // Assert
            _mongoCollectionMock.VerifyAll();
            result.Should().BeOfType<RepositoryFindResult<TestRepositoryRecord>.Found>()
                .Subject.Records.Should().BeEquivalentTo(new[] { record });
        }

        [TestMethod]
        public async Task Find_WhenManyBatchesOfRecordsExist_Found()
        {
            // Arrange
            var record1 = new TestRepositoryRecord();
            var record2 = new TestRepositoryRecord();
            IEnumerable<TestRepositoryRecord>[] values = new[] { new[] { record1 }, new[] { record2 } };
            using var cursorMock = new MockAsyncCursor<TestRepositoryRecord>(values);
            SetupFind().ReturnsAsync(cursorMock);

            // Act
            var result = await _systemUnderTest.Find(_ => true, "recordName");

            // Assert
            _mongoCollectionMock.VerifyAll();
            result.Should().BeOfType<RepositoryFindResult<TestRepositoryRecord>.Found>()
                .Subject.Records.Should().BeEquivalentTo(new[] { record1, record2 });
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

        private class MockAsyncCursor<T> : IAsyncCursor<T>
        {
            private readonly Queue<IEnumerable<T>> _batches;
            private IEnumerable<T> _current = null;

            public MockAsyncCursor(IEnumerable<IEnumerable<T>> batches)
            {
                _batches = new Queue<IEnumerable<T>>(batches);
            }

            public bool MoveNext(CancellationToken cancellationToken = new CancellationToken())
            {
                if (_batches.TryDequeue(out var next))
                {
                    _current = next;
                    return true;
                }

                _current = null;
                return false;
            }

            public Task<bool> MoveNextAsync(CancellationToken cancellationToken = new CancellationToken())
            {
                return Task.FromResult(MoveNext(cancellationToken));
            }

            public IEnumerable<T> Current => _current ?? throw new InvalidOperationException(
                "Invalid test setup: Current cannot be accessed before the first call to MoveNext or the last call to it");

            public void Dispose()
            {}
        }
    }
}