using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.GpSystems;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Emis
{
    [TestClass]
    public class EmisTokenValidationServiceTests
    {
        private ITokenValidationService _sut;

        [TestInitialize]
        public void TestInitialize()
        {
            _sut = new EmisTokenValidationService();
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
            _sut.IsValidConnectionTokenFormat("2fc69313-a4c6-4a46-a617-56cdb423c122").Should().BeTrue();
        }
    }
}
