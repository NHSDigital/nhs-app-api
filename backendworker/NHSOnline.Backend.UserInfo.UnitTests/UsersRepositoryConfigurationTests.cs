using System;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Support.Settings;

namespace NHSOnline.Backend.UserInfo.UnitTests
{
    [TestClass]
    public class UserAndInfoRepositoryConfigurationTests
    {
        private Mock<IConfiguration> _mockConfiguration;

        private const string DatabaseKey = "MONGO_DATABASE_NAME";
        private const string CollectionKey = "USERINFO_MONGO_DATABASE_COLLECTION";

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
            var config = new UserAndInfoRepositoryConfiguration(_mockConfiguration.Object,
                new Mock<ILogger<UserAndInfoRepositoryConfiguration>>().Object);
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
            Action act = () => new UserAndInfoRepositoryConfiguration(_mockConfiguration.Object,
                new Mock<ILogger<UserAndInfoRepositoryConfiguration>>().Object).Validate();

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
            Action act = () => new UserAndInfoRepositoryConfiguration(_mockConfiguration.Object,
                new Mock<ILogger<UserAndInfoRepositoryConfiguration>>().Object).Validate();

            // Assert
            act.Should().Throw<ConfigurationNotValidException>()
                .Which.Message.Should().Contain(CollectionKey);
        }

        private void SetupConfiguration(string databaseName = "DatabaseName",
            string databaseCollection = "DatabaseCollection")
        {
            _mockConfiguration.Setup(x => x[DatabaseKey]).Returns(databaseName);
            _mockConfiguration.Setup(x => x[CollectionKey])
                .Returns(databaseCollection);
        }
    }
}