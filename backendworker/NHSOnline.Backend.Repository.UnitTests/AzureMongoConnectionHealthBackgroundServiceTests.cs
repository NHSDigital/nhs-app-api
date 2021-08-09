using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace NHSOnline.Backend.Repository.UnitTests
{
    [TestClass]
    public class AzureMongoConnectionHealthBackgroundServiceTests: IDisposable
    {
        private Mock<IMongoClientService> _mockMongoClientService;
        private Mock<ILogger<AzureMongoConnectionHealthBackgroundService>> _mockLogger;
        private Mock<IConfiguration> _mockConfiguration;

        private AzureMongoConnectionHealthBackgroundService _systemUnderTest;
        private const string DatabaseName = "DatabaseName";

        [TestInitialize]
        public void TestInitialize()
        {
            _mockMongoClientService = new Mock<IMongoClientService>();
            _mockLogger = new Mock<ILogger<AzureMongoConnectionHealthBackgroundService>>();
            _mockConfiguration = new Mock<IConfiguration>();

            _mockConfiguration.Setup(x => x["MONGO_DB_CONNECTION_CHECK_INTERVAL_IN_SECONDS"]).Returns("10");
            _mockConfiguration.Setup(x => x["MONGO_DATABASE_NAME"]).Returns(DatabaseName);

            _systemUnderTest = new AzureMongoConnectionHealthBackgroundService(
                _mockMongoClientService.Object,
                _mockLogger.Object,
                _mockConfiguration.Object);
        }

        [TestMethod]
        public async Task ExecuteAsync_DoesNotCallRebuildIfNecessary_WhenTypeIsNotAzureMongoService()
        {
            //Arrange
            _mockMongoClientService.SetupGet(x => x.SupportsConnectionRecovery).Returns(false);

            //Act
            await _systemUnderTest.StartAsync(new CancellationToken());

            //Assert
            _mockMongoClientService.VerifyAll();
        }

        [TestMethod]
        public async Task ExecuteAsync_CallsRebuildIfNecessary_WhenTypeIsAzureMongoService()
        {
            // Arrange
            _mockMongoClientService.SetupGet(x => x.SupportsConnectionRecovery).Returns(true);

            // Act
            await _systemUnderTest.StartAsync(new CancellationToken());

            // Assert
           _mockMongoClientService.Verify(x => x.RebuildIfNecessary(
               DatabaseName,
               It.IsAny<Guid>(),
               It.IsAny<uint>()));
        }

        public void Dispose()
        {
             GC.SuppressFinalize(this);
             _systemUnderTest.Dispose();
        }
    }
}
