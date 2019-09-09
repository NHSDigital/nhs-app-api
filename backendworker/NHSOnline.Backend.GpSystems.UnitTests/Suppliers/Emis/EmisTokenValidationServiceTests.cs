using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Emis
{
    [TestClass]
    public class EmisTokenValidationServiceTests
    {
        private ILogger<EmisTokenValidationService> _logger;
        private ITokenValidationService _systemUnderTest;

        private static readonly string Guid = System.Guid.NewGuid().ToString();

        [TestInitialize]
        public void TestInitialize()
        {
            _logger = Mock.Of<ILogger<EmisTokenValidationService>>();
            _systemUnderTest = new EmisTokenValidationService(_logger);
        }

        [TestMethod]
        public void IsValidConnectionTokenFormat_ResultIsFalse_WhenTokenIsNotInAGuidFormat()
        {
            // Act
            var result = _systemUnderTest.IsValidConnectionTokenFormat("foobar");
                
            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsValidConnectionTokenFormat_ResultIsFalse_WhenTokenIsNull()
        {
            // Act
            var result = _systemUnderTest.IsValidConnectionTokenFormat(null);
                
            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsValidConnectionTokenFormat_ResultIsTrue_WhenTokenIsInAGuidFormat()
        {
            // Act
            var result = _systemUnderTest.IsValidConnectionTokenFormat(Guid);
                
            // Assert
            result.Should().BeTrue();
        }

        [TestMethod]
        public void IsValidConnectionTokenFormat_ResultIsTrue_WhenTokenIsInExpectedJsonFormat()
        {
            // Arrange
            var token = new EmisConnectionToken { AccessIdentityGuid = Guid, Im1CacheKey = "foo" }.SerializeJson();

            // Act
            var result = _systemUnderTest.IsValidConnectionTokenFormat(token);
                
            // Assert
            result.Should().BeTrue();
        }

        [DataTestMethod]
        [DataRow(null, null)]
        [DataRow("", "")]
        [DataRow(" ", " ")]
        [DataRow("foo", "bar")]
        public void IsValidConnectionTokenFormat_ResultIsFalse_WhenTokenHasInvalidProperties(
            string accessIdentityGuid, string retrievalKey )
        {
            // Arrange
            var token = new EmisConnectionToken
            {
                AccessIdentityGuid = accessIdentityGuid,
                Im1CacheKey = retrievalKey
            }.SerializeJson();

            // Act
            var result = _systemUnderTest.IsValidConnectionTokenFormat(token);
                
            // Assert
            result.Should().BeFalse();
        }
    }
}
