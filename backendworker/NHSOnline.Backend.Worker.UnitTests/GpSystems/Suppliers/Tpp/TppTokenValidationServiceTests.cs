using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.GpSystems;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Tpp
{
    [TestClass]
    public class TppTokenValidationServiceTests
    {
        private string _invalidConnectionToken = "";
        private string _validConnectionToken = "";
        private ITokenValidationService _sut;
        
        [TestInitialize]
        public void TestInitialize()
        {
            _sut = new TppTokenValidationService();
        }
        
        [TestMethod]
        public void IsValidConnectionTokenFormat_ResultIsFalse_WhenTokenIsNotInAJsonStringFormat()
        {
            _sut.IsValidConnectionTokenFormat("foobar").Should().BeFalse();
        }

        [TestMethod]
        public void IsValidConnectionTokenFormat_ResultIsFalse_WhenTokenIsNull()
        {
            _sut.IsValidConnectionTokenFormat(null).Should().BeFalse();
        }
        
        [TestMethod]
        public void IsValidConnectionTokenFormat_ResultIsFalse_WhenTokenIsEmpty()
        {
            _sut.IsValidConnectionTokenFormat("").Should().BeFalse();
        }
        
        [TestMethod]
        public void IsValidConnectionTokenFormat_ResultIsFalse_WhenTokenIsMissingPassphrase()
        {
            _invalidConnectionToken = "{\"accountid\":\"account_id\"}";
            _sut.IsValidConnectionTokenFormat(_invalidConnectionToken).Should().BeFalse();
        }
        
        [TestMethod]
        public void IsValidConnectionTokenFormat_ResultIsFalse_WhenTokenIsMissingAccountId()
        {
            _invalidConnectionToken = "{\"passphrase\":\"passphrase\"}";;
            _sut.IsValidConnectionTokenFormat(_invalidConnectionToken).Should().BeFalse();
        }
        
        [TestMethod]
        public void IsValidConnectionTokenFormat_ResultIsFalse_WhenTokenPassphraseIsEmpty()
        {
            _invalidConnectionToken = "{\"accountid\":\"account_id\",\"passphrase\":\"\"}";
            _sut.IsValidConnectionTokenFormat(_invalidConnectionToken).Should().BeFalse();
        }
        
        [TestMethod]
        public void IsValidConnectionTokenFormat_ResultIsFalse_WhenTokenAccountIdIsEmpty()
        {
            _invalidConnectionToken = "{\"accountid\":\"\",\"passphrase\":\"\"}";
            _sut.IsValidConnectionTokenFormat(_invalidConnectionToken).Should().BeFalse();
        }

        [TestMethod]
        public void IsValidConnectionTokenFormat_ResultIsTrue_WhenTokenIsInAJsonStringFormat()
        {
            _validConnectionToken = "{\"accountid\":\"account_id\",\"passphrase\":\"passphrase\"}";
            _sut.IsValidConnectionTokenFormat(_validConnectionToken).Should().BeTrue();
        }
        
        [TestMethod]
        public void IsValidConnectionTokenFormat_ResultIsTrue_WhenTokenIsInAJsonCamelCaseStringFormat()
        {
            _validConnectionToken = "{\"AccountId\":\"account_id\",\"Passphrase\":\"passphrase\"}";
            _sut.IsValidConnectionTokenFormat(_validConnectionToken).Should().BeTrue();
        }
    }
}