using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp
{
    [TestClass]
    public class TppTokenValidationServiceTests
    {
        private string _invalidConnectionToken = "";
        private string _validConnectionToken = "";
        private ITokenValidationService _sut;
        private ILogger<TppTokenValidationService> _logger;

        [TestInitialize]
        public void TestInitialize()
        {
            _logger = Mock.Of<ILogger<TppTokenValidationService>>();
            _sut = new TppTokenValidationService(_logger);
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
            _invalidConnectionToken = "{\"accountId\":\"account_id\",\"providerId\":\"providerId\"}";
            _sut.IsValidConnectionTokenFormat(_invalidConnectionToken).Should().BeFalse();
        }

        [TestMethod]
        public void IsValidConnectionTokenFormat_ResultIsFalse_WhenTokenIsMissingAccountId()
        {
            _invalidConnectionToken = "{\"passphrase\":\"passphrase\",\"providerId\":\"providerId\"}";
            _sut.IsValidConnectionTokenFormat(_invalidConnectionToken).Should().BeFalse();
        }

        [TestMethod]
        public void IsValidConnectionTokenFormat_ResultIsFalse_WhenTokenIsMissingProviderId()
        {
            _invalidConnectionToken = "{\"passphrase\":\"passphrase\",\"accountId\":\"account_id\"}";
            _sut.IsValidConnectionTokenFormat(_invalidConnectionToken).Should().BeFalse();
        }

        [TestMethod]
        public void IsValidConnectionTokenFormat_ResultIsFalse_WhenTokenPassphraseIsEmpty()
        {
            _invalidConnectionToken = "{\"accountId\":\"account_id\",\"passphrase\":\"\",\"providerId\":\"providerId\"}";
            _sut.IsValidConnectionTokenFormat(_invalidConnectionToken).Should().BeFalse();
        }

        [TestMethod]
        public void IsValidConnectionTokenFormat_ResultIsFalse_WhenTokenAccountIdIsEmpty()
        {
            _invalidConnectionToken = "{\"accountId\":\"\",\"passphrase\":\"\",\"providerId\":\"providerId\"}";
            _sut.IsValidConnectionTokenFormat(_invalidConnectionToken).Should().BeFalse();
        }

        [TestMethod]
        public void IsValidConnectionTokenFormat_ResultIsFalse_WhenTokenProviderIdIsEmpty()
        {
            _invalidConnectionToken = "{\"accountId\":\"account_id\",\"passphrase\":\"passphrase\",\"providerId\":\"\"}";
            _sut.IsValidConnectionTokenFormat(_invalidConnectionToken).Should().BeFalse();
        }

        [TestMethod]
        public void IsValidConnectionTokenFormat_ResultIsTrue_WhenTokenIsInAJsonStringFormat()
        {
            _validConnectionToken = "{\"accountId\":\"account_id\",\"passphrase\":\"passphrase\",\"providerId\":\"provder_id\"}";
            _sut.IsValidConnectionTokenFormat(_validConnectionToken).Should().BeTrue();
        }
        
        [TestMethod]
        public void IsValidConnectionTokenFormat_ResultIsTrue_WhenTokenIsInAJsonCamelCaseStringFormat()
        {
            _validConnectionToken = "{\"AccountId\":\"account_id\",\"Passphrase\":\"passphrase\",\"ProviderId\":\"provder_id\"}";
            _sut.IsValidConnectionTokenFormat(_validConnectionToken).Should().BeTrue();
        }
    }
}