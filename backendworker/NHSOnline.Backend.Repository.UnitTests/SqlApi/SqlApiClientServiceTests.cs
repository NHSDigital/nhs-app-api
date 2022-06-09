using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Azure.Cosmos;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Repository.SqlApi;

namespace NHSOnline.Backend.Repository.UnitTests.SqlApi
{
    [TestClass]
    public class SqlApiClientServiceTests
    {
        private Mock<ICosmosClientWrapper> _cosmosClientWrapper;
        private TestSqlApiRepositoryConfiguration _config;

        private SqlApiClientService _systemUnderTest;
        private Mock<Container> _container;

        private Mock<ItemResponse<TestRepositoryRecord>> _itemResponse;

        [TestInitialize]
        public void TestInitialize()
        {
            _config = new TestSqlApiRepositoryConfiguration
            {
                DatabaseName = "Database name",
                ContainerName = "Container name"
            };

            _container = new Mock<Container>();
            _itemResponse = new Mock<ItemResponse<TestRepositoryRecord>>(MockBehavior.Strict);

            _cosmosClientWrapper = new Mock<ICosmosClientWrapper>(MockBehavior.Strict);
            _cosmosClientWrapper.Setup(c => c.GetContainer(_config.DatabaseName, _config.ContainerName))
                .Returns(_container.Object);

            _systemUnderTest = new SqlApiClientService(_cosmosClientWrapper.Object);
        }

        [TestMethod]
        public async Task UpsertOneAsync_Success()
        {
            // Arrange
            var record = new TestRepositoryRecord();
            var partitionKeyValue = "TestPartitionKeyValue";

            _container.Setup(c => c
                    .UpsertItemAsync(record, new PartitionKey(partitionKeyValue), null, default))
                .ReturnsAsync(_itemResponse.Object);

            // Act
            var result = await _systemUnderTest.UpsertOneAsync(_config, record, partitionKeyValue);

            // Assert
            VerifyAll();
            result.Should().BeEquivalentTo(_itemResponse.Object);
        }

        [TestMethod]
        public async Task UpsertOneAsync_WhenClientThrowsException_ShouldThrow()
        {
            // Arrange
            var record = new TestRepositoryRecord();
            var partitionKeyValue = "TestPartitionKeyValue";

            _container.Setup(c => c
                    .UpsertItemAsync(record, new PartitionKey(partitionKeyValue), null, default))
                .ThrowsAsync(new CosmosException("Testing a failure", HttpStatusCode.Forbidden, 1234, "activityId",
                    1.12));

            // Act
            await FluentActions.Awaiting(() => _systemUnderTest.UpsertOneAsync(_config, record, partitionKeyValue))
                .Should().ThrowAsync<CosmosException>()
                .WithMessage("Testing a failure");

            // Assert
            VerifyAll();
        }

        [TestMethod]
        public async Task DeleteOneAsync_Success()
        {
            // Arrange
            var id = "Id value";
            var partitionKeyValue = "Partition Key Value";

            _container.Setup(c => c
                    .DeleteItemAsync<TestRepositoryRecord>(id, new PartitionKey(partitionKeyValue), null, default))
                .ReturnsAsync(_itemResponse.Object);

            // Act
            var result = await _systemUnderTest.DeleteOneAsync<TestRepositoryRecord>(_config, id, partitionKeyValue);

            // Assert
            VerifyAll();
            result.Should().BeEquivalentTo(_itemResponse.Object);
        }

        [TestMethod]
        public async Task DeleteOneAsync_WhenClientThrowsException_ShouldThrow()
        {
            // Arrange
            var id = "Id value";
            var partitionKeyValue = "Partition Key Value";

            _container.Setup(c => c
                    .DeleteItemAsync<TestRepositoryRecord>(id, new PartitionKey(partitionKeyValue), null, default))
                .ThrowsAsync(new CosmosException("Testing a failure", HttpStatusCode.Forbidden, 1234, "activityId",
                    1.12));

            // Act
            await FluentActions.Awaiting(() =>
                    _systemUnderTest.DeleteOneAsync<TestRepositoryRecord>(_config, id, partitionKeyValue))
                .Should().ThrowAsync<CosmosException>()
                .WithMessage("Testing a failure");

            // Assert
            VerifyAll();
        }

        [TestMethod]
        public async Task FindOneAsync_WhenClientThrowsException_ShouldThrow()
        {
            // Arrange
            var partitionKeyValue = "TestPartitionKeyValue";

            _container.Setup(c => c
                    .ReadItemAsync<TestRepositoryRecord>(It.IsAny<string>(), It.IsAny<PartitionKey>(),
                        It.IsAny<ItemRequestOptions>(), It.IsAny<CancellationToken>())
                )
                .ThrowsAsync(new CosmosException("Testing a failure", HttpStatusCode.Forbidden, 1234, "activityId",
                    1.12));

            // Act
            await FluentActions.Awaiting(() =>
                    _systemUnderTest.FindOneAsync<TestRepositoryRecord>(_config, "id", partitionKeyValue))
                .Should().ThrowAsync<CosmosException>()
                .WithMessage("Testing a failure");

            // Assert
            VerifyAll();
        }

        [TestMethod]
        public async Task FindOneAsync_Success()
        {
            //Arrange
            var id = "Id value";
            var partitionKeyValue = "TestPartitionKeyValue";

            _container.Setup(c => c
                .ReadItemAsync<TestRepositoryRecord>(id, new PartitionKey(partitionKeyValue),
                    It.IsAny<ItemRequestOptions>(), It.IsAny<CancellationToken>())
            ).ReturnsAsync(_itemResponse.Object);

            //Act
            var result = await _systemUnderTest.FindOneAsync<TestRepositoryRecord>(_config, id, partitionKeyValue);

            //Assert
            VerifyAll();
            result.Should().BeEquivalentTo(_itemResponse.Object);
        }

        [TestMethod]
        public async Task CheckHealthAsync_Success()
        {
            // Arrange
            var containerResponse = new Mock<ContainerResponse>(MockBehavior.Strict);

            _container.Setup(c => c
                    .ReadContainerAsync(null, default))
                .ReturnsAsync(containerResponse.Object);

            // Act
            var result = await _systemUnderTest.CheckHealthAsync(_config);

            // Assert
            VerifyAll();
            result.Should().BeEquivalentTo(containerResponse.Object);
        }

        [TestMethod]
        public async Task CheckHealthAsync_WhenClientThrowsException_ShouldThrow()
        {
            // Arrange
            _container.Setup(c => c
                    .ReadContainerAsync(null, default))
                .ThrowsAsync(new CosmosException("Testing a failure", HttpStatusCode.Forbidden, 1234, "activityId",
                    1.12));

            // Act
            await FluentActions.Awaiting(() =>
                    _systemUnderTest.CheckHealthAsync(_config))
                .Should().ThrowAsync<CosmosException>()
                .WithMessage("Testing a failure");

            // Assert
            VerifyAll();
        }

        private void VerifyAll()
        {
            _itemResponse.VerifyAll();
            _container.VerifyAll();
            _cosmosClientWrapper.VerifyAll();
        }
    }
}