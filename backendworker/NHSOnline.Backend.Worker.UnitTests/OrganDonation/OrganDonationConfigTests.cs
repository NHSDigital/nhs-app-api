using System;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.OrganDonation;
using NHSOnline.Backend.Support.Settings;

namespace NHSOnline.Backend.Worker.UnitTests.OrganDonation
{
    [TestClass]
    public class OrganDonationConfigTests
    {
        private Mock<IConfiguration> _mockConfiguration;
        private IFixture _fixture;
        private ILogger<OrganDonationConfig> _logger;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _mockConfiguration = _fixture.Freeze<Mock<IConfiguration>>();
            _logger = _fixture.Create<ILogger<OrganDonationConfig>>();
            SetupConfigurationValue("ORGAN_DONATION_BASE_URL", "http://nhs.uk/");
            SetupConfigurationValue("ORGAN_DONATION_CLIENT_ID", "ABCD");
            SetupConfigurationValue("ORGAN_DONATION_OCP_APIM_SUBSCRIPTION_KEY", "1234");
            SetupConfigurationValue("ORGAN_DONATION_REFERENCE_DATA_EXPIRY_HOURS", "24");
;        }

        [TestMethod]
        public void Constructor_WhenConfigurationHasNoValueForReferenceDataExpiryHours_ThrowsAnException()
        {
            // Arrange
            SetupConfigurationValue("ORGAN_DONATION_REFERENCE_DATA_EXPIRY_HOURS", null);
            
            // Act
            Action create = () => new OrganDonationConfig(_mockConfiguration.Object, _logger);
            
            // Assert
            create
                .Should()
                .Throw<ConfigurationNotFoundException>()
                .WithMessage("Configuration value 'ORGAN_DONATION_REFERENCE_DATA_EXPIRY_HOURS' not found");
        }
        
        [TestMethod]
        public void Constructor_WhenConfigurationValueForReferenceDataExpiryHoursIsNotAnInteger_ThrowsAnException()
        {
            // Arrange
            SetupConfigurationValue("ORGAN_DONATION_REFERENCE_DATA_EXPIRY_HOURS", "boo");
            
            // Act
            Action create = () => new OrganDonationConfig(_mockConfiguration.Object, _logger);
            
            // Assert
            create
                .Should()
                .Throw<FormatException>();
        }

        private void SetupConfigurationValue(string key, string value)
        {
            _mockConfiguration.SetupGet(x => x[key]).Returns(value);
        }
    }
}