using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
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
        public async Task Find_WhenRecordsExist_Found()
        {
            // Arrange
            var record = new TestRepositoryRecord();
            IEnumerable<TestRepositoryRecord>[] values = { new[] { record } };
            using var cursorMock = new MockAsyncCursor<TestRepositoryRecord>(values);
            SetupFind().ReturnsAsync(cursorMock);

            // Act
            var result = await _systemUnderTest.Find(_ => true, "recordName");

            // Assert
            _mongoClientWrapperMock.VerifyAll();
            result.Should().BeOfType<RepositoryFindResult<TestRepositoryRecord>.Found>()
                .Subject.Records.Should().BeEquivalentTo(record);
        }

        [TestMethod]
        public async Task Find_WhenManyBatchesOfRecordsExist_Found()
        {
            // Arrange
            var record1 = new TestRepositoryRecord();
            var record2 = new TestRepositoryRecord();
            IEnumerable<TestRepositoryRecord>[] values = { new[] { record1 }, new[] { record2 } };
            using var cursorMock = new MockAsyncCursor<TestRepositoryRecord>(values);
            SetupFind().ReturnsAsync(cursorMock);

            // Act
            var result = await _systemUnderTest.Find(_ => true, "recordName");

            // Assert
            _mongoClientWrapperMock.VerifyAll();
            result.Should().BeOfType<RepositoryFindResult<TestRepositoryRecord>.Found>()
                .Subject.Records.Should().BeEquivalentTo(record1, record2);
        }

        [TestMethod]
        public async Task Find_WhenMaxRecordsIsPassed_CallsMongoWithFindOptionsLimitSetToMaxRecords()
        {
            // Arrange
            var maxRecords = 3;
            FindOptions<TestRepositoryRecord, TestRepositoryRecord> findOptionsUsed = null;
            IEnumerable<TestRepositoryRecord>[] values = { Enumerable.Empty<TestRepositoryRecord>() };
            using var cursorMock = new MockAsyncCursor<TestRepositoryRecord>(values);
            SetupFind()
                .Callback<
                    IRepositoryConfiguration,
                    Expression<Func<TestRepositoryRecord, bool>>,
                    FindOptions<TestRepositoryRecord>
                >( (config, filter, options) => findOptionsUsed = options)
                .ReturnsAsync(cursorMock);

            // Act
            await _systemUnderTest.Find(_ => true, "recordName", maxRecords);

            // Assert
            _mongoClientWrapperMock.VerifyAll();
            findOptionsUsed.Limit.Should().Be(maxRecords);
        }

        [TestMethod]
        public async Task Find_WhenMaxRecordsIsNotPassed_CallsMongoWithFindOptionsLimitSetToNull()
        {
            // Arrange
            FindOptions<TestRepositoryRecord, TestRepositoryRecord> findOptionsUsed = null;
            IEnumerable<TestRepositoryRecord>[] values = { Enumerable.Empty<TestRepositoryRecord>() };
            using var cursorMock = new MockAsyncCursor<TestRepositoryRecord>(values);
            SetupFind()
                .Callback<
                    IRepositoryConfiguration,
                    Expression<Func<TestRepositoryRecord, bool>>,
                    FindOptions<TestRepositoryRecord>
                >( (config, filter, options) => findOptionsUsed = options)
                .ReturnsAsync(cursorMock);

            // Act
            await _systemUnderTest.Find(_ => true, "recordName");

            // Assert
            _mongoClientWrapperMock.VerifyAll();
            findOptionsUsed.Limit.Should().BeNull();
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
            _mongoClientWrapperMock.VerifyAll();
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
            _mongoClientWrapperMock.VerifyAll();
            result.Should().BeOfType<RepositoryFindResult<TestRepositoryRecord>.RepositoryError>();
        }

        private ISetup<IMongoClientService, Task<IAsyncCursor<TestRepositoryRecord>>> SetupFind()
        {
            return _mongoClientWrapperMock
               .Setup(x => x.FindAsync(
                   _repositoryConfiguration,
                   It.IsAny<Expression<Func<TestRepositoryRecord, bool>>>(),
                It.IsAny<FindOptions<TestRepositoryRecord>>()));
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
            private IEnumerable<T> _current;

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
