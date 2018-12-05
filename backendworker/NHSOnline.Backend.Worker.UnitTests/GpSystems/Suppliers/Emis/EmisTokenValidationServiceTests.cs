using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.GpSystems;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Emis
{
    [TestClass]
    public class EmisTokenValidationServiceTests
    {
        private ILogger<EmisTokenValidationService> _logger;
        private ITokenValidationService _sut;

        private const string Guid = "2fc69313-a4c6-4a46-a617-56cdb423c122";

        [TestInitialize]
        public void TestInitialize()
        {
            _logger = Mock.Of<ILogger<EmisTokenValidationService>>();
            _sut = new EmisTokenValidationService(_logger);
        }

        [TestMethod]
        public void IsValidConnectionTokenFormat_ResultIsFalse_WhenTokenIsNotInAGuidFormat()
        {
            _sut.IsValidConnectionTokenFormat("foobar").Should().BeFalse();
        }

        [TestMethod]
        public void IsValidConnectionTokenFormat_ResultIsFalse_WhenTokenIsNull()
        {
            _sut.IsValidConnectionTokenFormat(null).Should().BeFalse();
        }

        [TestMethod]
        public void IsValidConnectionTokenFormat_ResultIsTrue_WhenTokenIsInAGuidFormat()
        {
            _sut.IsValidConnectionTokenFormat(Guid).Should().BeTrue();
        }

        [TestMethod]
        public void IsValidConnectionTokenFormat_ResultIsTrue_WhenTokenIsInExpectedJsonFormat()
        {
            var token = new EmisConnectionToken { AccessIdentityGuid = Guid, Im1CacheKey = "foo" }.SerializeJson();

            _sut.IsValidConnectionTokenFormat(token).Should().BeTrue();
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

            _sut.IsValidConnectionTokenFormat(token).Should().BeFalse();
        }
    }
}
