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
    public class UserAndInfoRepositoryByOdsCodeConfigurationTests
    {
        private Mock<IConfiguration> _mockConfiguration;

        private const string DatabaseKey = "COMMS_SQL_API_DATABASE_NAME";
        private const string CollectionKey = "USERINFO_ODS_CODE_CONTAINER";

        [TestInitialize]
        public void TestInitialize()
        {
            _mockConfiguration = new Mock<IConfiguration>();
        }

        [TestMethod]
        public void Validate_WhenConfigValuesAreValid_ShouldSuccess()
        {
            var expectedDatabaseName = "ExpectedDatabaseName";
            var expectedDatabaseContainer = "ExpectedDatabaseContainer";

            // Arrange
            SetupConfiguration(
                databaseContainer: expectedDatabaseContainer,
                databaseName: expectedDatabaseName);

            // Act
            var config = new UserAndInfoRepositoryByOdsCodeConfiguration(_mockConfiguration.Object,
                new Mock<ILogger<UserAndInfoRepositoryByOdsCodeConfiguration>>().Object);
            config.Validate();

            // Assert
            config.DatabaseName.Should().BeEquivalentTo(expectedDatabaseName);
            config.ContainerName.Should().BeEquivalentTo(expectedDatabaseContainer);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void Validate_WhenDatabaseNameInvalid_ThrowsException(string value)
        {
            // Arrange
            SetupConfiguration(databaseName: value);

            // Act
            Action act = () => new UserAndInfoRepositoryByOdsCodeConfiguration(_mockConfiguration.Object,
                new Mock<ILogger<UserAndInfoRepositoryByOdsCodeConfiguration>>().Object).Validate();

            // Assert
            act.Should().Throw<ConfigurationNotValidException>()
                .Which.Message.Should().Contain(DatabaseKey);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void Validate_WhenContainerInvalid_ThrowsException(string value)
        {
            // Arrange
            SetupConfiguration(databaseContainer: value);

            // Act
            Action act = () => new UserAndInfoRepositoryByOdsCodeConfiguration(_mockConfiguration.Object,
                new Mock<ILogger<UserAndInfoRepositoryByOdsCodeConfiguration>>().Object).Validate();

            // Assert
            act.Should().Throw<ConfigurationNotValidException>()
                .Which.Message.Should().Contain(CollectionKey);
        }

        private void SetupConfiguration(string databaseName = "DatabaseName",
            string databaseContainer = "DatabaseContainer")
        {
            _mockConfiguration.Setup(x => x[DatabaseKey]).Returns(databaseName);
            _mockConfiguration.Setup(x => x[CollectionKey])
                .Returns(databaseContainer);
        }
    }
}