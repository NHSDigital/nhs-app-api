using System;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
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
        private Mock<IWebHostEnvironment> _mockHostingEnvironment;
        private Startup _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _mockHostingEnvironment = new Mock<IWebHostEnvironment>();

            _systemUnderTest = new Startup(_mockConfiguration.Object,
                new LoggerFactory(),
                _mockHostingEnvironment.Object);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void ConfigureServices_AzureConfiguration_WhenHubPathIsInvalid_ThrowsException(string hubPath)
        {
            // Arrange
            _mockConfiguration.Setup(x => x["AZURE_NOTIFICATION_HUB_PATH"]).Returns(hubPath);
            _mockConfiguration.Setup(x => x["AZURE_NOTIFICATION_HUB_CONNECTION_STRING"])
                .Returns("Valid Connection String");
            _mockConfiguration.Setup(x => x["AZURE_NOTIFICATION_HUB_SHARED_ACCESS_KEY"])
                .Returns("Valid Shared Access Key");
            _mockConfiguration.Setup(x => x["CITIZEN_ID_CLIENT_ID"]).Returns("Valid ClientID");
            _mockConfiguration.Setup(x => x["CITIZEN_ID_JWT_ISSUER"]).Returns("Valid JWT");
            _mockConfiguration.Setup(x => x["CITIZEN_ID_BASE_URL"]).Returns("Valid BaseURL");
            _mockConfiguration.Setup(x => x["NHSAPP_API_KEY"]).Returns("Valid API Key");
            _mockConfiguration.Setup(x => x["ConfigurationSettings:DefaultHttpTimeoutSeconds"]).Returns("2");

            // Act
            Action act = () => _systemUnderTest.ConfigureServices(new ServiceCollection());

            // Assert
            act.Should().Throw<ConfigurationNotValidException>()
                .Which.Message.Should().Contain("AZURE_NOTIFICATION_HUB_PATH");
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void ConfigureServices_AzureConfiguration_WhenHubConnectionStringIsInvalid_ThrowsException
            (string hubConnectionString)
        {
            // Arrange
            _mockConfiguration.Setup(x => x["AZURE_NOTIFICATION_HUB_PATH"]).Returns("Valid Hub Path");
            _mockConfiguration.Setup(x => x["AZURE_NOTIFICATION_HUB_CONNECTION_STRING"])
                .Returns(hubConnectionString);
            _mockConfiguration.Setup(x => x["AZURE_NOTIFICATION_HUB_SHARED_ACCESS_KEY"])
                .Returns("Valid Shared Access Key");
            _mockConfiguration.Setup(x => x["CITIZEN_ID_CLIENT_ID"]).Returns("Valid ClientID");
            _mockConfiguration.Setup(x => x["CITIZEN_ID_JWT_ISSUER"]).Returns("Valid JWT");
            _mockConfiguration.Setup(x => x["CITIZEN_ID_BASE_URL"]).Returns("Valid BaseURL");
            _mockConfiguration.Setup(x => x["NHSAPP_API_KEY"]).Returns("Valid API Key");
            _mockConfiguration.Setup(x => x["ConfigurationSettings:DefaultHttpTimeoutSeconds"]).Returns("2");

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
            _mockConfiguration.Setup(x => x["AZURE_NOTIFICATION_HUB_PATH"]).Returns("Valid Hub Path");
            _mockConfiguration.Setup(x => x["AZURE_NOTIFICATION_HUB_CONNECTION_STRING"])
                .Returns("Valid Connection String");
            _mockConfiguration.Setup(x => x["AZURE_NOTIFICATION_HUB_SHARED_ACCESS_KEY"])
                .Returns(sharedKey);
            _mockConfiguration.Setup(x => x["CITIZEN_ID_CLIENT_ID"]).Returns("Valid ClientID");
            _mockConfiguration.Setup(x => x["CITIZEN_ID_JWT_ISSUER"]).Returns("Valid JWT");
            _mockConfiguration.Setup(x => x["CITIZEN_ID_BASE_URL"]).Returns("Valid BaseURL");
            _mockConfiguration.Setup(x => x["NHSAPP_API_KEY"]).Returns("Valid API Key");
            _mockConfiguration.Setup(x => x["ConfigurationSettings:DefaultHttpTimeoutSeconds"]).Returns("2");

            // Act
            Action act = () => _systemUnderTest.ConfigureServices(new ServiceCollection());

            // Assert
            act.Should().Throw<ConfigurationNotValidException>()
                .Which.Message.Should().Contain("AZURE_NOTIFICATION_HUB_SHARED_ACCESS_KEY");
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void ConfigureServices_AzureConfiguration_WhenApiKeyKeyIsInvalid_ThrowsException(string apiKey)
        {
            // Arrange
            _mockConfiguration.Setup(x => x["AZURE_NOTIFICATION_HUB_PATH"]).Returns("Valid Hub Path");
            _mockConfiguration.Setup(x => x["AZURE_NOTIFICATION_HUB_CONNECTION_STRING"])
                .Returns("Valid Connection String");
            _mockConfiguration.Setup(x => x["AZURE_NOTIFICATION_HUB_SHARED_ACCESS_KEY"])
                .Returns("Valid Shared Access Key");
            _mockConfiguration.Setup(x => x["CITIZEN_ID_CLIENT_ID"]).Returns("Valid ClientID");
            _mockConfiguration.Setup(x => x["CITIZEN_ID_JWT_ISSUER"]).Returns("Valid JWT");
            _mockConfiguration.Setup(x => x["CITIZEN_ID_BASE_URL"]).Returns("Valid BaseURL");
            _mockConfiguration.Setup(x => x["NHSAPP_API_KEY"]).Returns(apiKey);
            _mockConfiguration.Setup(x => x["ConfigurationSettings:DefaultHttpTimeoutSeconds"]).Returns("2");

            // Act
            Action act = () => _systemUnderTest.ConfigureServices(new ServiceCollection());

            // Assert
            act.Should().Throw<ConfigurationNotValidException>()
                .Which.Message.Should().Contain("NHSAPP_API_KEY");
        }

                [TestMethod]
        public void ConfigureServices_AzureConfiguration_WhenAllValuesAreProvided_MapsToProperties()
        {
            // Arrange
            const string hubPath = "Valid Hub Path";
            const string hubConnectionString = "Valid Connection String";
            const string sharedAccessKey = "Valid Shared Access Key";
            var serviceCollection = new ServiceCollection();

            _mockConfiguration.Setup(x => x["AZURE_NOTIFICATION_HUB_PATH"]).Returns(hubPath);
            _mockConfiguration.Setup(x => x["AZURE_NOTIFICATION_HUB_CONNECTION_STRING"]).Returns(hubConnectionString);
            _mockConfiguration.Setup(x => x["AZURE_NOTIFICATION_HUB_SHARED_ACCESS_KEY"]).Returns(sharedAccessKey);
            _mockConfiguration.Setup(x => x["CITIZEN_ID_CLIENT_ID"]).Returns("Valid ClientID");
            _mockConfiguration.Setup(x => x["CITIZEN_ID_JWT_ISSUER"]).Returns("Valid JWT");
            _mockConfiguration.Setup(x => x["CITIZEN_ID_BASE_URL"]).Returns("Valid BaseURL");
            _mockConfiguration.Setup(x => x["NHSAPP_API_KEY"]).Returns("Valid API Key");
            _mockConfiguration.Setup(x => x["ConfigurationSettings:DefaultHttpTimeoutSeconds"]).Returns("2");

            // Act
            _systemUnderTest.ConfigureServices(serviceCollection);

            // Assert
            _mockConfiguration.VerifyAll();

            serviceCollection.Should().NotBeEmpty();

            var azureConfiguration = serviceCollection.FirstOrDefault(x => x.ImplementationInstance is AzureNotificationConfiguration)
                    ?.ImplementationInstance as AzureNotificationConfiguration;

            using (new AssertionScope())
            {
                azureConfiguration.Should().NotBeNull();
                azureConfiguration.NotificationHubPath.Should().Be(hubPath);
                azureConfiguration.ConnectionString.Should().Be(hubConnectionString);
                azureConfiguration.SharedAccessKey.Should().Be(sharedAccessKey);
            }
        }

        [TestMethod]
        public void ConfigureServices_ConfigureAuth_WhenInProduction_ShouldRequireHttpsMetadata()
        {
            // Arrange
            SetupAllConfiguration();

            _mockHostingEnvironment.SetupGet(x => x.EnvironmentName).Returns(Environments.Production);

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
            SetupAllConfiguration();

            _mockHostingEnvironment.SetupGet(x => x.EnvironmentName).Returns(Environments.Development);

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

        private void SetupAllConfiguration()
        {
            _mockConfiguration.Setup(x => x["AZURE_NOTIFICATION_HUB_PATH"]).Returns("Valid Hub Path");
            _mockConfiguration.Setup(x => x["AZURE_NOTIFICATION_HUB_CONNECTION_STRING"])
                .Returns("Valid Connection String");
            _mockConfiguration.Setup(x => x["AZURE_NOTIFICATION_HUB_SHARED_ACCESS_KEY"])
                .Returns("Valid Shared Access Key");
            _mockConfiguration.Setup(x => x["CITIZEN_ID_CLIENT_ID"]).Returns("Valid ClientID");
            _mockConfiguration.Setup(x => x["CITIZEN_ID_JWT_ISSUER"]).Returns("Valid JWT");
            _mockConfiguration.Setup(x => x["CITIZEN_ID_BASE_URL"]).Returns("https://authority.which.must.be.https.com/");
            _mockConfiguration.Setup(x => x["NHSAPP_API_KEY"]).Returns("Valid Api Key");
            _mockConfiguration.Setup(x => x["ConfigurationSettings:DefaultHttpTimeoutSeconds"]).Returns("2");
        }
    }
}