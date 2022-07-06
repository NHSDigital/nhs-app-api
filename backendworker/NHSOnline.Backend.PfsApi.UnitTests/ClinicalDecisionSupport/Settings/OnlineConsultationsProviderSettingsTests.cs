using FluentAssertions;
 using Microsoft.VisualStudio.TestTools.UnitTesting;
 using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.Settings;
 using NHSOnline.Backend.Support.Settings;

 namespace NHSOnline.Backend.PfsApi.UnitTests.ClinicalDecisionSupport.Settings
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
                     ProviderName = "econsult Health Ltd"
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
         public void Validate_WhenBaseAddressIsNullOrEmpty_ThrowsConfigurationNotFoundException(string baseAddress)
         {
             // Arrange
             _providerSettings.BaseAddress = baseAddress;

             // Act
             _providerSettings.Validate();
         }

         [TestMethod]
         [DataRow("randomte.ccccc")]
         public void Validate_WhenBaseAddressIsInvalidUrl_ThrowsConfigurationNotValidException(string baseAddress)
         {
             // Arrange
             _providerSettings.BaseAddress = baseAddress;

             // Act
             FluentActions
                 .Invoking(_providerSettings.Validate)
                 .Should().Throw<ConfigurationNotValidException>()
                 .And.Message.Should().Contain("OnlineConsultationsProvider.BaseAddress");
         }

         [TestMethod]
         [DataRow(null)]
         [DataRow("")]
         [DataRow("   ")]
         [ExpectedException(typeof(ConfigurationNotFoundException))]
         public void Validate_WhenProviderNameIsNullOrEmpty_ThrowsConfigurationNotFoundException(string provider)
         {
             // Arrange
             _providerSettings.ProviderName = provider;

             // Act
             _providerSettings.Validate();
         }
     }
 }