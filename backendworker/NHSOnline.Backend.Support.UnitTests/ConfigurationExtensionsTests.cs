using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace NHSOnline.Backend.Support.UnitTests
{
    [TestClass]
    public class ConfigurationExtensionsTests
    {
        const string ConfigurationKeyName = "configuration_key";
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
            Assert.AreEqual(1, result);
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
            Assert.AreEqual(0, result);
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
            Assert.AreEqual(0, result);
        }
    }
}
