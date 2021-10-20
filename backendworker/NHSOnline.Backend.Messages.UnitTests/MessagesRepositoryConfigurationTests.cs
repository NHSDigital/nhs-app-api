using System;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Support.Settings;

namespace NHSOnline.Backend.MessagesApi.UnitTests
{
    [TestClass]
    public class MessagesRepositoryConfigurationTests
    {
        private Mock<IConfiguration> _mockConfiguration;

        private const string DatabaseKey = "MONGO_DATABASE_NAME";
        private const string CollectionKey = "MESSAGES_MONGO_DATABASE_MESSAGES_COLLECTION";

        [TestInitialize]
        public void TestInitialize()
        {
            _mockConfiguration = new Mock<IConfiguration>();
        }

        [TestMethod]
        public void ValidDatabaseCollection()
        {
            var expectedDatabaseName = "ExpectedDatabaseName";
            var expectedDatabaseCollection = "ExpectedDatabaseCollection";

            // Arrange
            SetupConfiguration(
                databaseCollection: expectedDatabaseCollection,
                databaseName: expectedDatabaseName);

            // Act
            var config = new MessagesRepositoryConfiguration(_mockConfiguration.Object,
                new Mock<ILogger<MessagesRepositoryConfiguration>>().Object);
            config.Validate();

            // Assert
            config.DatabaseName.Should().BeEquivalentTo(expectedDatabaseName);
            config.CollectionName.Should().BeEquivalentTo(expectedDatabaseCollection);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void InvalidDatabaseName_ThrowsException(string value)
        {
            // Arrange
            SetupConfiguration(databaseName: value);

            // Act
            Action act = () => new MessagesRepositoryConfiguration(_mockConfiguration.Object,
                new Mock<ILogger<MessagesRepositoryConfiguration>>().Object).Validate();

            // Assert
            act.Should().Throw<ConfigurationNotValidException>()
                .Which.Message.Should().Contain(DatabaseKey);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void InvalidDatabaseCollection_ThrowsException(string value)
        {
            // Arrange
            SetupConfiguration(databaseCollection: value);

            // Act
            Action act = () => new MessagesRepositoryConfiguration(_mockConfiguration.Object,
                new Mock<ILogger<MessagesRepositoryConfiguration>>().Object).Validate();

            // Assert
            act.Should().Throw<ConfigurationNotValidException>()
                .Which.Message.Should().Contain(CollectionKey);
        }

        private void SetupConfiguration(
            string databaseName = "DatabaseName",
            string databaseCollection = "DatabaseCollection")
        {
            _mockConfiguration.Setup(x => x[DatabaseKey]).Returns(databaseName);
            _mockConfiguration.Setup(x => x[CollectionKey])
                .Returns(databaseCollection);
        }
    }
}