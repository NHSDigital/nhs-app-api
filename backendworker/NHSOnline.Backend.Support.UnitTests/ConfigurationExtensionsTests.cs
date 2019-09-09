using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace NHSOnline.Backend.Support.UnitTests
{
    [TestClass]
    public class ConfigurationExtensionsTests
    {
        private const string ConfigurationKeyName = "configuration_key";
        private IConfigurationBuilder _configurationBuilder;
        private Mock<ILogger<ConfigurationExtensionsTests>> _logger;

        [TestInitialize]
        public void TestInitialize()
        {
            _configurationBuilder = new ConfigurationBuilder();
            _logger = new Mock<ILogger<ConfigurationExtensionsTests>>();
        }

        [TestMethod]
        public void KeyFound_AndIsInteger_ReturnsExpectedValue()
        {
            // Arrange
            _configurationBuilder.AddInMemoryCollection(
                new Dictionary<string, string>() { { ConfigurationKeyName, "1" } });
            var configuration = _configurationBuilder.Build();
            
            // Act
            var result = configuration.GetIntOrDefault(ConfigurationKeyName, _logger.Object);

            // Assert
            result.Should().Be(1);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("abc")]
        public void KeyFound_AndIsNotInteger_ReturnsDefaultIntZero(string configurationValue)
        {
            // Arrange
            _configurationBuilder.AddInMemoryCollection(
                new Dictionary<string, string>() { { ConfigurationKeyName, configurationValue } });
            var configuration = _configurationBuilder.Build();

            // Act
            var result = configuration.GetIntOrDefault(ConfigurationKeyName, _logger.Object);

            // Assert
            result.Should().Be(0);
        }

        [TestMethod]
        public void KeyNotFound_ReturnsDefaultIntZero()
        {
            // Arrange
            _configurationBuilder.AddInMemoryCollection();
            var configuration = _configurationBuilder.Build();

            // Act
            var result = configuration.GetIntOrDefault(ConfigurationKeyName, _logger.Object);

            // Assert
            result.Should().Be(0);
        }

        [TestMethod]
        public void GetApiVersion_ReturnsConcatenatedVersionAndCommit()
        {
            // Arrange
            const string version = "configuration_version_1";
            const string commitId = "commit_id_132321";

            _configurationBuilder.AddInMemoryCollection(
               new Dictionary<string, string>()
               {
                   {
                       Constants.EnvironmentalVariables.VersionTag, version
                   },
                   {
                       Constants.AppConfig.GitCommitId, commitId
                   }
               });

            var configuration = _configurationBuilder.Build();

            // Act
            var result = configuration.GetApiAppVersion();

            // Arrange
            result.Should().Be($"{version} (commit:{commitId})");
        }
    }
}
