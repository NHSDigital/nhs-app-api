using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Support.Settings;

namespace NHSOnline.Backend.UsersApi.UnitTests
{
    [TestClass]
    public class StartupTests
    {
        private Mock<IConfiguration> _mockConfiguration;
        private List<Mock<IConfigurationSection>> _mockHubConfigurationSections;
        private Mock<IWebHostEnvironment> _mockHostingEnvironment;
        private Startup _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockConfiguration = new Mock<IConfiguration>(MockBehavior.Strict);
            _mockHubConfigurationSections = new List<Mock<IConfigurationSection>>();
            _mockHostingEnvironment = new Mock<IWebHostEnvironment>(MockBehavior.Strict);

            _systemUnderTest = new Startup(_mockConfiguration.Object,
                new LoggerFactory(),
                _mockHostingEnvironment.Object
            );
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void ConfigureServices_AzureConfiguration_WhenHubPathIsInvalid_ThrowsException(string hubPath)
        {
            // Arrange
            SetupConfiguration();

            _mockHubConfigurationSections.First()
                .Setup(x => x["AZURE_NOTIFICATION_HUB_PATH"])
                .Returns(hubPath);

            // Act
            Action act = () => _systemUnderTest.ConfigureServices(new ServiceCollection());

            // Assert
            act.Should().Throw<ConfigurationNotValidException>()
                .Which.Message.Should().Contain("AZURE_NOTIFICATION_HUB_PATH");
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void ConfigureServices_AzureConfiguration_WhenHubConnectionStringIsInvalid_ThrowsException(string hubConnectionString)
        {
            // Arrange
            SetupConfiguration();

            _mockHubConfigurationSections.First()
                .Setup(x => x["AZURE_NOTIFICATION_HUB_CONNECTION_STRING"])
                .Returns(hubConnectionString);

            // Act
            Action act = () => _systemUnderTest.ConfigureServices(new ServiceCollection());

            // Assert
            act.Should().Throw<ConfigurationNotValidException>()
                .Which.Message.Should().Contain("AZURE_NOTIFICATION_HUB_CONNECTION_STRING");
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void ConfigureServices_AzureConfiguration_WhenHubSharedKeyIsInvalid_ThrowsException(string sharedKey)
        {
            // Arrange
            SetupConfiguration();

            _mockHubConfigurationSections.First()
                .Setup(x => x["AZURE_NOTIFICATION_HUB_SHARED_ACCESS_KEY"])
                .Returns(sharedKey);

            // Act
            Action act = () => _systemUnderTest.ConfigureServices(new ServiceCollection());

            // Assert
            act.Should().Throw<ConfigurationNotValidException>()
                .Which.Message.Should().Contain("AZURE_NOTIFICATION_HUB_SHARED_ACCESS_KEY");
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void ConfigureServices_AzureConfiguration_WhenHubReadCharactersIsInvalid_ThrowsException(string characters)
        {
            // Arrange
            SetupConfiguration();

            _mockHubConfigurationSections.First()
                .Setup(x => x["AZURE_NOTIFICATION_HUB_READ_CHARACTERS"])
                .Returns(characters);

            // Act
            Action act = () => _systemUnderTest.ConfigureServices(new ServiceCollection());

            // Assert
            act.Should().Throw<ConfigurationNotValidException>()
                .Which.Message.Should().Contain("AZURE_NOTIFICATION_HUB_READ_CHARACTERS");
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void ConfigureServices_AzureConfiguration_WhenHubWriteCharactersIsInvalid_ThrowsException(string characters)
        {
            // Arrange
            SetupConfiguration();

            _mockHubConfigurationSections.First()
                .Setup(x => x["AZURE_NOTIFICATION_HUB_WRITE_CHARACTERS"])
                .Returns(characters);

            // Act
            Action act = () => _systemUnderTest.ConfigureServices(new ServiceCollection());

            // Assert
            act.Should().Throw<ConfigurationNotValidException>()
                .Which.Message.Should().Contain("AZURE_NOTIFICATION_HUB_WRITE_CHARACTERS");
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("Non-Integer Value")]
        public void ConfigureServices_AzureConfiguration_WhenHubGenerationIsInvalid_ThrowsException(string generation)
        {
            // Arrange
            SetupConfiguration();

            _mockHubConfigurationSections.First()
                .Setup(x => x["AZURE_NOTIFICATION_HUB_GENERATION"])
                .Returns(generation);

            // Act
            Action act = () => _systemUnderTest.ConfigureServices(new ServiceCollection());

            // Assert
            act.Should().Throw<ConfigurationNotValidException>()
                .Which.Message.Should().Contain("AZURE_NOTIFICATION_HUB_GENERATION");
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void ConfigureServices_AzureConfiguration_WhenApiKeyKeyIsInvalid_ThrowsException(string apiKey)
        {
            // Arrange
            SetupConfiguration();

            _mockConfiguration
                .Setup(x => x["NHSAPP_API_KEY"])
                .Returns(apiKey);

            // Act
            Action act = () => _systemUnderTest.ConfigureServices(new ServiceCollection());

            // Assert
            act.Should().Throw<ConfigurationNotValidException>()
                .Which.Message.Should().Contain("NHSAPP_API_KEY");
        }

        [TestMethod]
        public void ConfigureServices_ConfigureAuth_WhenInProduction_ShouldRequireHttpsMetadata()
        {
            // Arrange
            SetupConfiguration();

            _mockHostingEnvironment
                .SetupGet(x => x.EnvironmentName)
                .Returns(Environments.Production);

            var serviceCollection = new ServiceCollection();

            // Act
            _systemUnderTest.ConfigureServices(serviceCollection);

            // Assert
            var jwtBearerOptions = serviceCollection
                .BuildServiceProvider()
                .GetRequiredService<IOptionsSnapshot<JwtBearerOptions>>()
                .Get("Bearer");

            jwtBearerOptions.RequireHttpsMetadata.Should().BeTrue();
        }

        [TestMethod]
        public void ConfigureServices_ConfigureAuth_WhenInDevelopment_ShouldNotRequireHttpsMetadata()
        {
            // Arrange
            SetupConfiguration();

            _mockHostingEnvironment
                .SetupGet(x => x.EnvironmentName)
                .Returns(Environments.Development);

            var serviceCollection = new ServiceCollection();

            // Act
            _systemUnderTest.ConfigureServices(serviceCollection);

            // Assert
            var jwtBearerOptions = serviceCollection
                .BuildServiceProvider()
                .GetRequiredService<IOptionsSnapshot<JwtBearerOptions>>()
                .Get("Bearer");

            jwtBearerOptions.RequireHttpsMetadata.Should().BeFalse();
        }

        private void SetupConfiguration()
        {
            _mockConfiguration
                .Setup(x => x["CITIZEN_ID_CLIENT_ID"])
                .Returns("Valid ClientID");

            _mockConfiguration
                .Setup(x => x["CITIZEN_ID_JWT_ISSUER"])
                .Returns("Valid JWT");

            _mockConfiguration
                .Setup(x => x["CITIZEN_ID_BASE_URL"])
                .Returns("https://authority.which.must.be.https.com/");

            _mockConfiguration
                .Setup(x => x["NHSAPP_API_KEY"])
                .Returns("Valid Api Key");

            _mockConfiguration
                .Setup(x => x["ENVIRONMENT_NAME"])
                .Returns("Valid Value");

            _mockConfiguration
                .Setup(x => x["COMMS_HUB_EVENT_HUB_PID_CONNECTION_STRING"])
                .Returns("Endpoint=sb://example.servicebus.windows.net/;SharedAccessKeyName=example-sender;SharedAccessKey=fake;EntityPath=example-events");

            _mockConfiguration
                .Setup(x => x["COMMS_HUB_EVENT_HUB_CONNECTION_STRING"])
                .Returns("Endpoint=sb://example.servicebus.windows.net/;SharedAccessKeyName=example-sender;SharedAccessKey=fake;EntityPath=example-events");

            _mockConfiguration
                .Setup(x => x["ConfigurationSettings:DefaultHttpTimeoutSeconds"])
                .Returns("2");

            SetupNotificationHubConfigurationFields();
        }

        private void SetupNotificationHubConfigurationFields()
        {
            var section = new Mock<IConfigurationSection>(MockBehavior.Strict);

            section
                .Setup(x => x["AZURE_NOTIFICATION_HUB_PATH"])
                .Returns("Valid Hub Path");

            section
                .Setup(x => x["AZURE_NOTIFICATION_HUB_CONNECTION_STRING"])
                .Returns("Valid Connection String");

            section
                .Setup(x => x["AZURE_NOTIFICATION_HUB_SHARED_ACCESS_KEY"])
                .Returns("Valid Shared Access Key");

            section
                .Setup(x => x["AZURE_NOTIFICATION_HUB_READ_CHARACTERS"])
                .Returns("0123456789ABCDEF");

            section
                .Setup(x => x["AZURE_NOTIFICATION_HUB_WRITE_CHARACTERS"])
                .Returns("0123456789ABCDEF");

            section
                .Setup(x => x["AZURE_NOTIFICATION_HUB_GENERATION"])
                .Returns("1");

            _mockHubConfigurationSections.Add(section);

            var mockSection = new Mock<IConfigurationSection>();

            mockSection
                .Setup(x => x.GetChildren())
                .Returns(_mockHubConfigurationSections.Select(x => x.Object));

            _mockConfiguration
                .Setup(x => x.GetSection("AZURE_NOTIFICATION_HUBS"))
                .Returns(mockSection.Object);
        }
    }
}