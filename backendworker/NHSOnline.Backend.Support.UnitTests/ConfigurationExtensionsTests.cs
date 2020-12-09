using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Support.Settings;

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
        public void GetIntOrDefault_KeyFoundAndIsInteger_ReturnsExpectedValue()
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
        public void GetIntOrDefault_KeyFoundAndIsNotInteger_ReturnsDefaultIntZero(string configurationValue)
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
        public void GetIntOrDefault_KeyNotFound_ReturnsDefaultIntZero()
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
        public void GetBoolOrDefault_KeyNotFound_ReturnsFalse()
        {
            // Arrange
            _configurationBuilder.AddInMemoryCollection();
            var configuration = _configurationBuilder.Build();

            // Act
            var result = configuration.GetBoolOrDefault(ConfigurationKeyName, _logger.Object);

            // Assert
            result.Should().BeFalse();
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow("Not a false string")]
        public void GetBoolOrDefault_KeyFoundUnparsableString_ReturnsFalse(string value)
        {
            // Arrange
            _configurationBuilder.AddInMemoryCollection(
                new Dictionary<string, string>() { { ConfigurationKeyName, value } });
            var configuration = _configurationBuilder.Build();

            // Act
            var result = configuration.GetBoolOrDefault(ConfigurationKeyName, _logger.Object);

            // Assert
            result.Should().BeFalse();
        }

        [DataTestMethod]
        [DataRow("false")]
        [DataRow("False")]
        [DataRow("FALSE")]
        public void GetBoolOrDefault_KeyFoundAndFalseString_ReturnsFalse(string value)
        {
            // Arrange
            _configurationBuilder.AddInMemoryCollection(
                new Dictionary<string, string>() { { ConfigurationKeyName, value } });
            var configuration = _configurationBuilder.Build();

            // Act
            var result = configuration.GetBoolOrDefault(ConfigurationKeyName, _logger.Object);

            // Assert
            result.Should().BeFalse();
        }

        [DataTestMethod]
        [DataRow("true")]
        [DataRow("True")]
        [DataRow("TRUE")]
        public void GetBoolOrDefault_KeyFoundAndTrueString_ReturnsTrue(string value)
        {
            // Arrange
            _configurationBuilder.AddInMemoryCollection(
                new Dictionary<string, string>() { { ConfigurationKeyName, value } });
            var configuration = _configurationBuilder.Build();

            // Act
            var result = configuration.GetBoolOrDefault(ConfigurationKeyName, _logger.Object);

            // Assert
            result.Should().BeTrue();
        }

        [TestMethod]
        public void GetBoolOrThrow_KeyNotFound_ThrowsConfigurationNotValidException()
        {
            // Arrange
            var configuration = _configurationBuilder.Build();

            // Act
            Action act = () => configuration.GetBoolOrThrow(ConfigurationKeyName, _logger.Object);

            // Assert
            act.Should().Throw<ConfigurationNotValidException>()
                .And.Message.Should().Be("Configuration value 'configuration_key' is not valid");
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow("Not a false string")]
        public void GetBoolOrThrow_KeyFoundUnparsableString_ThrowsConfigurationNotValidException(string value)
        {
            // Arrange
            _configurationBuilder.AddInMemoryCollection(
                new Dictionary<string, string>() { { ConfigurationKeyName, value } });
            var configuration = _configurationBuilder.Build();

            // Act
            Action act = () => configuration.GetBoolOrThrow(ConfigurationKeyName, _logger.Object);

            // Assert
            act.Should().Throw<ConfigurationNotValidException>()
                .And.Message.Should().Be("Configuration value 'configuration_key' is not valid");
        }

        [DataTestMethod]
        [DataRow("false")]
        [DataRow("False")]
        [DataRow("FALSE")]
        public void GetBoolOrThrow_KeyFoundAndFalseString_ReturnsFalse(string value)
        {
            // Arrange
            _configurationBuilder.AddInMemoryCollection(
                new Dictionary<string, string>() { { ConfigurationKeyName, value } });
            var configuration = _configurationBuilder.Build();

            // Act
            var result = configuration.GetBoolOrThrow(ConfigurationKeyName, _logger.Object);

            // Assert
            result.Should().BeFalse();
        }

        [DataTestMethod]
        [DataRow("true")]
        [DataRow("True")]
        [DataRow("TRUE")]
        public void GetBoolOrThrow_KeyFoundAndTrueString_ReturnsTrue(string value)
        {
            // Arrange
            _configurationBuilder.AddInMemoryCollection(
                new Dictionary<string, string>() { { ConfigurationKeyName, value } });
            var configuration = _configurationBuilder.Build();

            // Act
            var result = configuration.GetBoolOrThrow(ConfigurationKeyName, _logger.Object);

            // Assert
            result.Should().BeTrue();
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
