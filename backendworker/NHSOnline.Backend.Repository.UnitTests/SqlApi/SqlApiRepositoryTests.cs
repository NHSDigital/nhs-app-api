using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Repository.SqlApi;

namespace NHSOnline.Backend.Repository.UnitTests.SqlApi
{
    [TestClass]
    public class SqlApiRepositoryTests
    {
        private SqlApiRepository<TestSqlApiRepositoryConfiguration, TestRepositoryRecord> _systemUnderTest;
        private TestSqlApiRepositoryConfiguration _config;
        private Mock<ILogger<SqlApiRepository<TestSqlApiRepositoryConfiguration, TestRepositoryRecord>>> _logger;
        private Mock<ItemResponse<TestRepositoryRecord>> _itemResponse;

        private Mock<ISqlApiClientService> _sqlApiClientService;

        [TestInitialize]
        public void Initialize()
        {
            _config = new TestSqlApiRepositoryConfiguration
            {
                DatabaseName = "Database name",
                ContainerName = "Container name"
            };

            _sqlApiClientService = new Mock<ISqlApiClientService>(MockBehavior.Strict);
            _logger = new Mock<ILogger<SqlApiRepository<TestSqlApiRepositoryConfiguration, TestRepositoryRecord>>>();

            _itemResponse = new Mock<ItemResponse<TestRepositoryRecord>>();

            _systemUnderTest = new SqlApiRepository<TestSqlApiRepositoryConfiguration, TestRepositoryRecord>(
                _sqlApiClientService.Object, _config, _logger.Object);
        }

        [TestMethod]
        [DataRow(HttpStatusCode.Created)]
        [DataRow(HttpStatusCode.OK)]
        public async Task CreateOrUpdate_Success(HttpStatusCode statusCode)
        {
            // Arrange
            var record = new TestRepositoryRecord();
            var partitionKeyValue = "TestPartitionKeyValue";

            _itemResponse.SetupGet(x => x.StatusCode)
                .Returns(statusCode);
            _itemResponse.SetupGet(x => x.Resource)
                .Returns(record);

            _sqlApiClientService
                .Setup(x => x.UpsertOneAsync(_config, record, partitionKeyValue))
                .ReturnsAsync(_itemResponse.Object);

            // Act
            var result = await _systemUnderTest.CreateOrUpdate(record, "TestRecordName", partitionKeyValue);

            // Assert
            VerifyAll();
            result.Should().BeOfType<RepositoryCreateResult<TestRepositoryRecord>.Created>();
        }

        [TestMethod]
        [DataRow(HttpStatusCode.NoContent)]
        [DataRow(HttpStatusCode.BadRequest)]
        [DataRow(HttpStatusCode.Unauthorized)]
        [DataRow(HttpStatusCode.Forbidden)]
        [DataRow(HttpStatusCode.NotFound)]
        [DataRow(HttpStatusCode.RequestTimeout)]
        [DataRow(HttpStatusCode.Conflict)]
        [DataRow(HttpStatusCode.PreconditionFailed)]
        [DataRow(HttpStatusCode.RequestEntityTooLarge)]
        [DataRow(HttpStatusCode.Locked)]
        [DataRow(HttpStatusCode.FailedDependency)]
        [DataRow(HttpStatusCode.TooManyRequests)]
        [DataRow(HttpStatusCode.InternalServerError)]
        [DataRow(HttpStatusCode.ServiceUnavailable)]
        public async Task CreateOrUpdate_NotCreatedStatusCode_ReturnsRepositoryError(HttpStatusCode statusCode)
        {
            // Arrange
            var record = new TestRepositoryRecord();
            var partitionKeyValue = "TestPartitionKey";

            _itemResponse.SetupGet(x => x.StatusCode)
                .Returns(statusCode);

            _sqlApiClientService
                .Setup(x => x.UpsertOneAsync(_config, record, partitionKeyValue))
                .ReturnsAsync(_itemResponse.Object);

            // Act
            var result = await _systemUnderTest.CreateOrUpdate(record, "TestRecordName", partitionKeyValue);

            // Assert
            VerifyAll();
            result.Should().BeOfType<RepositoryCreateResult<TestRepositoryRecord>.RepositoryError>();
        }

        [TestMethod]
        public async Task CreateOrUpdate_RepositoryThrowsCosmosException_ReturnsRepositoryError()
        {
            // Arrange
            var record = new TestRepositoryRecord();
            var partitionKeyValue = "TestPartitionKey";

            _sqlApiClientService
                .Setup(x => x.UpsertOneAsync(_config, record, partitionKeyValue))
                .ThrowsAsync(new CosmosException("Test", HttpStatusCode.Forbidden, 1234, "activityId", 1.12));

            // Act
            var result = await _systemUnderTest.CreateOrUpdate(record, "TestRecordName", partitionKeyValue);

            // Assert
            VerifyAll();
            result.Should().BeOfType<RepositoryCreateResult<TestRepositoryRecord>.RepositoryError>();
        }

        [TestMethod]
        public async Task Delete_Success()
        {
            // Arrange
            var id = "Nhs Login Id";
            var partitionKeyValue = "partition key value";

            _itemResponse.SetupGet(x => x.StatusCode)
                .Returns(HttpStatusCode.NoContent);

            _sqlApiClientService
                .Setup(x => x.DeleteOneAsync<TestRepositoryRecord>(_config, id, partitionKeyValue))
                .ReturnsAsync(_itemResponse.Object);

            // Act
            var result = await _systemUnderTest.Delete(id, partitionKeyValue, "TestRecordName");

            // Assert
            VerifyAll();
            result.Should().BeOfType<RepositoryDeleteResult<TestRepositoryRecord>.Deleted>();
        }

        [TestMethod]
        public async Task Delete_RepositoryThrowsCosmosExceptionWithStatusCodeNotFound_ReturnsNotFound()
        {
            // Arrange
            var id = "Nhs Login Id";
            var partitionKeyValue = "partition key value";

            var response = new CosmosException(
                "Testing Not Found exception",
                HttpStatusCode.NotFound,
                123,
                "ActivitId",
                1.23);

            _sqlApiClientService
                .Setup(x => x.DeleteOneAsync<TestRepositoryRecord>(_config, id, partitionKeyValue))
                .ThrowsAsync(response);

            // Act
            var result = await _systemUnderTest.Delete(id, partitionKeyValue, "TestRecordName");

            // Assert
            VerifyAll();
            result.Should().BeOfType<RepositoryDeleteResult<TestRepositoryRecord>.NotFound>();
        }

        [TestMethod]
        [DataRow(HttpStatusCode.BadRequest)]
        [DataRow(HttpStatusCode.Unauthorized)]
        [DataRow(HttpStatusCode.Forbidden)]
        [DataRow(HttpStatusCode.OK)]
        [DataRow(HttpStatusCode.RequestTimeout)]
        [DataRow(HttpStatusCode.Conflict)]
        [DataRow(HttpStatusCode.PreconditionFailed)]
        [DataRow(HttpStatusCode.RequestEntityTooLarge)]
        [DataRow(HttpStatusCode.Locked)]
        [DataRow(HttpStatusCode.FailedDependency)]
        [DataRow(HttpStatusCode.TooManyRequests)]
        [DataRow(HttpStatusCode.InternalServerError)]
        [DataRow(HttpStatusCode.ServiceUnavailable)]
        public async Task Delete_AnyStatusCodeOtherThanNoContent_ReturnsRepositoryError(HttpStatusCode statusCode)
        {
            // Arrange
            var id = "Nhs Login Id";
            var partitionKeyValue = "partition key value";

            _itemResponse.SetupGet(x => x.StatusCode)
                .Returns(statusCode);

            _sqlApiClientService
                .Setup(x => x.DeleteOneAsync<TestRepositoryRecord>(_config, id, partitionKeyValue))
                .ReturnsAsync(_itemResponse.Object);

            // Act
            var result = await _systemUnderTest.Delete(id, partitionKeyValue, "TestRecordName");

            // Assert
            VerifyAll();
            result.Should().BeOfType<RepositoryDeleteResult<TestRepositoryRecord>.RepositoryError>();
        }

        [TestMethod]
        [DataRow(HttpStatusCode.BadRequest)]
        [DataRow(HttpStatusCode.Unauthorized)]
        [DataRow(HttpStatusCode.Forbidden)]
        [DataRow(HttpStatusCode.RequestTimeout)]
        [DataRow(HttpStatusCode.Conflict)]
        [DataRow(HttpStatusCode.PreconditionFailed)]
        [DataRow(HttpStatusCode.RequestEntityTooLarge)]
        [DataRow(HttpStatusCode.Locked)]
        [DataRow(HttpStatusCode.FailedDependency)]
        [DataRow(HttpStatusCode.TooManyRequests)]
        [DataRow(HttpStatusCode.InternalServerError)]
        [DataRow(HttpStatusCode.ServiceUnavailable)]
        public async Task Delete_RepositoryThrowsCosmosExceptionWithStatusCode_ReturnsRepositoryError(
            HttpStatusCode statusCode)
        {
            // Arrange
            var id = "Nhs Login Id";
            var partitionKeyValue = "partition key value";

            _sqlApiClientService
                .Setup(x => x.DeleteOneAsync<TestRepositoryRecord>(_config, id, partitionKeyValue))
                .ThrowsAsync(new CosmosException("Test", statusCode, 1234, "activityId", 1.12));

            // Act
            var result = await _systemUnderTest.Delete(id, partitionKeyValue, "TestRecordName");

            // Assert
            VerifyAll();
            result.Should().BeOfType<RepositoryDeleteResult<TestRepositoryRecord>.RepositoryError>();
        }

        [TestMethod]
        public async Task Find_RepositoryThrowsCosmosException_ReturnsRepositoryError()
        {
            // Arrange
            var id = "Nhs Login Id";
            var partitionKeyValue = "partition key value";

            _sqlApiClientService
                .Setup(x => x.FindOneAsync<TestRepositoryRecord>(_config, id, partitionKeyValue))
                .ThrowsAsync(new CosmosException("Test", HttpStatusCode.Forbidden, 1234, "activityId", 1.12));

            // Act
            var result = await _systemUnderTest.Find(id, partitionKeyValue, "TestRecordName");

            // Assert
            VerifyAll();
            result.Should().BeOfType<RepositoryFindResult<TestRepositoryRecord>.RepositoryError>();
        }

        [TestMethod]
        public async Task Find_RepositoryThrowsCosmosExceptionWithNotFound_ReturnsRepositoryNotFound()
        {
            // Arrange
            var id = "Nhs Login Id";
            var partitionKeyValue = "partition key value";

            _sqlApiClientService
                .Setup(x => x.FindOneAsync<TestRepositoryRecord>(_config, id, partitionKeyValue))
                .ThrowsAsync(new CosmosException("Test", HttpStatusCode.NotFound, 0, "activityId", 1.12));

            // Act
            var result = await _systemUnderTest.Find(id, partitionKeyValue, "TestRecordName");

            // Assert
            VerifyAll();
            result.Should().BeOfType<RepositoryFindResult<TestRepositoryRecord>.NotFound>();
        }

        [TestMethod]
        public async Task Find_Success()
        {
            // Arrange
            var id = "Nhs Login Id";
            var partitionKeyValue = "partition key value";

            _sqlApiClientService
                .Setup(x => x.FindOneAsync<TestRepositoryRecord>(_config, id, partitionKeyValue))
                .ReturnsAsync(_itemResponse.Object);

            // Act
            var result = await _systemUnderTest.Find(id, partitionKeyValue, "TestRecordName");

            // Assert
            VerifyAll();
            result.Should().BeOfType<RepositoryFindResult<TestRepositoryRecord>.Found>();
        }


        private void VerifyAll()
        {
            _sqlApiClientService.VerifyAll();
            _itemResponse.VerifyAll();
        }
    }
}