using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.ClinicalDecisionSupportApi.Settings;
using NHSOnline.Backend.Support.Settings;

namespace NHSOnline.Backend.ClinicalDecisionSupportApi.UnitTests.Settings
{
    [TestClass]
    public class OnlineConsultationsProviderSettingsTests
    {
        private OnlineConsultationsProviderSettings _providerSettings;
            
        [TestInitialize]
        public void TestInitialize()
        {
            _providerSettings =
                new OnlineConsultationsProviderSettings
                {
                    Provider = "eConsult",
                    BaseAddress = "http://test.test/test/",
                    BearerToken = "testBearerToken"
                };
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        [ExpectedException(typeof(ConfigurationNotFoundException))]
        public void Validate_WhenProviderIsNullOrEmpty_ThrowsConfigurationNotFoundException(string provider)
        {
            // Arrange
            _providerSettings.Provider = provider;
            
            // Act
            _providerSettings.Validate();
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        [ExpectedException(typeof(ConfigurationNotFoundException))]
        public void Validate_WhenBearerTokenIsNullOrEmpty_ThrowsConfigurationNotFoundException(string bearerToken)
        {
            // Arrange
            _providerSettings.BearerToken = bearerToken;
            
            // Act
            _providerSettings.Validate();
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("   ")]
        [DataRow("randomte.ccccc")]
        [ExpectedException(typeof(UriFormatException))]
        public void Validate_WhenBaseAddressIsInvalidUrl_ThrowsUriFormatException(string baseAddress)
        {
            // Arrange
            _providerSettings.BaseAddress = baseAddress;
            
            // Act
            _providerSettings.Validate();
        }
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Validate_WhenBaseAddressIsNull_ThrowsUriFormatException()
        {
            // Arrange
            _providerSettings.BaseAddress = null;
            
            // Act
            _providerSettings.Validate();
        }
    }
}