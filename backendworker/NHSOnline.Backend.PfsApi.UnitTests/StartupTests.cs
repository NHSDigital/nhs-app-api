using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NHSOnline.Backend.Repository;
using NHSOnline.Backend.Support.Settings;

namespace NHSOnline.Backend.PfsApi.UnitTests
{
    [TestClass]
    public class StartupTests
    {
        private List<IConfigurationSection> _hubConfigurationSections;
        private Mock<IWebHostEnvironment> _mockHostingEnvironment;
        private Startup _systemUnderTest;
        private IConfiguration _configurationRoot;


        [TestInitialize]
        public void TestInitialize()
        {
            _hubConfigurationSections = new List<IConfigurationSection>();
            _configurationRoot = new ConfigurationBuilder()
                .AddJsonFile("appsettings.test.json")
                .Build();

            _mockHostingEnvironment = new Mock<IWebHostEnvironment>();

            _systemUnderTest = new Startup(_configurationRoot,
                _mockHostingEnvironment.Object,
                new LoggerFactory()
            );

            var section = _configurationRoot.GetSection("AZURE_NOTIFICATION_HUBS");
            _hubConfigurationSections = section.GetChildren().ToList();
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void ConfigureServices_AzureConfiguration_WhenHubPathIsInvalid_ThrowsException(string hubPath)
        {
            _hubConfigurationSections[0]["AZURE_NOTIFICATION_HUB_PATH"] = hubPath;

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
            _hubConfigurationSections[0]["AZURE_NOTIFICATION_HUB_CONNECTION_STRING"] = hubConnectionString;

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
            _hubConfigurationSections[0]["AZURE_NOTIFICATION_HUB_SHARED_ACCESS_KEY"] = sharedKey;

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
            _hubConfigurationSections[0]["AZURE_NOTIFICATION_HUB_READ_CHARACTERS"] = characters;

            // Act
            Action act = () => _systemUnderTest.ConfigureServices(new ServiceCollection());

            // Assert
            act.Should().Throw<ConfigurationNotValidException>()
                .Which.Message.Should().Contain("AZURE_NOTIFICATION_HUB_READ_CHARACTERS");
        }


        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("Non-Integer Value")]
        public void ConfigureServices_AzureConfiguration_WhenHubGenerationIsInvalid_ThrowsException(string generation)
        {
            // Arrange
            _hubConfigurationSections[0]["AZURE_NOTIFICATION_HUB_GENERATION"] = generation;

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
            _configurationRoot["NHSAPP_API_KEY"] = apiKey;

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


        [TestMethod]
        public void ConfigureServices_WhenMongoConnectionStringIsPresent_ShouldRegisterLocalMongoDatabaseClient()
        {
            // Arrange
            _configurationRoot["MONGO_CONNECTION_STRING"] = "TestConnectionString";

            var serviceCollection = new ServiceCollection();

            // Act
            _systemUnderTest.ConfigureServices(serviceCollection);

            //Assert
            var mongoClientDescriptor = new ServiceDescriptor(typeof(IMongoClientService), typeof(LocalMongoService), ServiceLifetime.Singleton);

            var registeredMongoClients = serviceCollection.Where(x => x.ServiceType == typeof(IMongoClientService));

            registeredMongoClients.Count().Should().Be(1);
            registeredMongoClients.First().Should().BeEquivalentTo(mongoClientDescriptor);
        }


        [TestMethod]
        public void ConfigureServices_WhenMongoConnectionStringIsNotPresent_ShouldRegisterAzureMongoDatabaseClient()
        {
            // Arrange
            _configurationRoot["MONGO_CONNECTION_STRING"] = "";

            var serviceCollection = new ServiceCollection();

            // Act
            _systemUnderTest.ConfigureServices(serviceCollection);

            //Assert
            var mongoClientDescriptor = new ServiceDescriptor(typeof(IMongoClientService), typeof(AzureMongoService), ServiceLifetime.Singleton);
            var hostedServiceDescriptor = new ServiceDescriptor(typeof(IHostedService), typeof(AzureMongoConnectionHealthBackgroundService), ServiceLifetime.Singleton);

           var registeredMongoClients = serviceCollection.Where(x => x.ServiceType == typeof(IMongoClientService));
           var registeredHostedServices = serviceCollection.Where(x => x.ImplementationType == typeof(AzureMongoConnectionHealthBackgroundService));

           registeredMongoClients.Count().Should().Be(1);
           registeredMongoClients.First().Should().BeEquivalentTo(mongoClientDescriptor);

           registeredHostedServices.First().Should().BeEquivalentTo(hostedServiceDescriptor);
        }
    }
}