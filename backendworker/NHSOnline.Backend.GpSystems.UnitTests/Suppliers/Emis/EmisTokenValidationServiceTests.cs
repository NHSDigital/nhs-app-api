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

        private const string Guid = "2fc69313-a4c6-4a46-a617-56cdb423c122";

        [TestInitialize]
        public void TestInitialize()
        {
            _logger = Mock.Of<ILogger<EmisTokenValidationService>>();
            _systemUnderTest = new EmisTokenValidationService(_logger);
        }

        [TestMethod]
        public void IsValidConnectionTokenFormat_ResultIsFalse_WhenTokenIsNotInAGuidFormat()
        {
            _systemUnderTest.IsValidConnectionTokenFormat("foobar").Should().BeFalse();
        }

        [TestMethod]
        public void IsValidConnectionTokenFormat_ResultIsFalse_WhenTokenIsNull()
        {
            _systemUnderTest.IsValidConnectionTokenFormat(null).Should().BeFalse();
        }

        [TestMethod]
        public void IsValidConnectionTokenFormat_ResultIsTrue_WhenTokenIsInAGuidFormat()
        {
            _systemUnderTest.IsValidConnectionTokenFormat(Guid).Should().BeTrue();
        }

        [TestMethod]
        public void IsValidConnectionTokenFormat_ResultIsTrue_WhenTokenIsInExpectedJsonFormat()
        {
            var token = new EmisConnectionToken { AccessIdentityGuid = Guid, Im1CacheKey = "foo" }.SerializeJson();

            _systemUnderTest.IsValidConnectionTokenFormat(token).Should().BeTrue();
        }

        [DataTestMethod]
        [DataRow(null, null)]
        [DataRow("", "")]
        [DataRow(" ", " ")]
        [DataRow("foo", "bar")]
        public void IsValidConnectionTokenFormat_ResultIsFalse_WhenTokenHasInvalidProperties(
            string accessIdentityGuid, string retrievalKey )
        {
            var token = new EmisConnectionToken
            {
                AccessIdentityGuid = accessIdentityGuid,
                Im1CacheKey = retrievalKey
            }.SerializeJson();

            _systemUnderTest.IsValidConnectionTokenFormat(token).Should().BeFalse();
        }
    }
}
