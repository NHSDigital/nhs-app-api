using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Support.Settings;

namespace NHSOnline.Backend.UsersApi.UnitTests
{
    [TestClass]
    public class StartupTests
    {
        private IFixture _fixture;
        private Mock<IConfiguration> _mockConfiguration;
        private Startup _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            _mockConfiguration = _fixture.Freeze<Mock<IConfiguration>>();

            _systemUnderTest = _fixture.Create<Startup>();
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void ConfigureServices_MongoConfiguration_WhenDatabaseNameIsInvalid_ThrowsException(string databaseName)
        {
            // Arrange
            _mockConfiguration.Setup(x => x["USERS_MONGO_DATABASE_NAME"]).Returns(databaseName);
            _mockConfiguration.Setup(x => x["USERS_MONGO_DATABASE_USER_DEVICE_COLLECTION"])
                .Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["USERS_MONGO_DATABASE_HOST"]).Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["USERS_MONGO_DATABASE_PORT"]).Returns($"{_fixture.Create<int>()}");
            _mockConfiguration.Setup(x => x["AZURE_NOTIFICATION_HUB_PATH"]).Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["AZURE_NOTIFICATION_HUB_CONNECTION_STRING"]).Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["AZURE_NOTIFICATION_HUB_SHARED_ACCESS_KEY"]).Returns(_fixture.Create<string>());

            // Act
            Action act = () => _fixture.Do<IServiceCollection>(x => _systemUnderTest.ConfigureServices(x));

            // Assert
            act.Should().Throw<ConfigurationNotFoundException>()
                .Which.Message.Should().Contain("USERS_MONGO_DATABASE_NAME");
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void ConfigureServices_MongoConfiguration_WhenUserDeviceCollectionIsInvalid_ThrowsException(
            string userDevicesCollection)
        {
            // Arrange
            _mockConfiguration.Setup(x => x["USERS_MONGO_DATABASE_NAME"]).Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["USERS_MONGO_DATABASE_USER_DEVICE_COLLECTION"])
                .Returns(userDevicesCollection);
            _mockConfiguration.Setup(x => x["USERS_MONGO_DATABASE_HOST"]).Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["USERS_MONGO_DATABASE_PORT"]).Returns($"{_fixture.Create<int>()}");
            _mockConfiguration.Setup(x => x["AZURE_NOTIFICATION_HUB_PATH"]).Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["AZURE_NOTIFICATION_HUB_CONNECTION_STRING"]).Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["AZURE_NOTIFICATION_HUB_SHARED_ACCESS_KEY"]).Returns(_fixture.Create<string>());

            // Act
            Action act = () => _fixture.Do<IServiceCollection>(x => _systemUnderTest.ConfigureServices(x));

            // Assert
            act.Should().Throw<ConfigurationNotFoundException>()
                .Which.Message.Should().Contain("USERS_MONGO_DATABASE_USER_DEVICE_COLLECTION");
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void ConfigureServices_MongoConfiguration_WhenHostIsInvalid_ThrowsException(string host)
        {
            // Arrange
            _mockConfiguration.Setup(x => x["USERS_MONGO_DATABASE_NAME"]).Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["USERS_MONGO_DATABASE_USER_DEVICE_COLLECTION"])
                .Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["USERS_MONGO_DATABASE_HOST"]).Returns(host);
            _mockConfiguration.Setup(x => x["USERS_MONGO_DATABASE_PORT"]).Returns($"{_fixture.Create<int>()}");
            _mockConfiguration.Setup(x => x["AZURE_NOTIFICATION_HUB_PATH"]).Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["AZURE_NOTIFICATION_HUB_CONNECTION_STRING"]).Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["AZURE_NOTIFICATION_HUB_SHARED_ACCESS_KEY"]).Returns(_fixture.Create<string>());

            // Act
            Action act = () => _fixture.Do<IServiceCollection>(x => _systemUnderTest.ConfigureServices(x));

            // Assert
            act.Should().Throw<ConfigurationNotFoundException>()
                .Which.Message.Should().Contain("USERS_MONGO_DATABASE_HOST");
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("sss")]
        public void ConfigureServices_MongoConfiguration_WhenPortIsInvalid_ThrowsException(string port)
        {
            // Arrange
            _mockConfiguration.Setup(x => x["USERS_MONGO_DATABASE_NAME"]).Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["USERS_MONGO_DATABASE_USER_DEVICE_COLLECTION"])
                .Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["USERS_MONGO_DATABASE_HOST"]).Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["USERS_MONGO_DATABASE_PORT"]).Returns(port);
            _mockConfiguration.Setup(x => x["AZURE_NOTIFICATION_HUB_PATH"]).Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["AZURE_NOTIFICATION_HUB_CONNECTION_STRING"]).Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["AZURE_NOTIFICATION_HUB_SHARED_ACCESS_KEY"]).Returns(_fixture.Create<string>());

            // Act
            Action act = () => _fixture.Do<IServiceCollection>(x => _systemUnderTest.ConfigureServices(x));

            // Assert
            act.Should().Throw<ConfigurationNotFoundException>()
                .Which.Message.Should().Contain("USERS_MONGO_DATABASE_PORT");
        }

        [TestMethod]
        public void ConfigureServices_MongoConfiguration_WhenAllValuesAreProvided_MapsToProperties()
        {
            // Arrange
            var databaseName = _fixture.Create<string>();
            var userDeviceCollection = _fixture.Create<string>();
            var host = _fixture.Create<string>();
            var port = _fixture.Create<int>();
            var username = _fixture.Create<string>();
            var password = _fixture.Create<string>();

            _mockConfiguration.Setup(x => x["USERS_MONGO_DATABASE_NAME"]).Returns(databaseName);
            _mockConfiguration.Setup(x => x["USERS_MONGO_DATABASE_USER_DEVICE_COLLECTION"])
                .Returns(userDeviceCollection);
            _mockConfiguration.Setup(x => x["USERS_MONGO_DATABASE_HOST"]).Returns(host);
            _mockConfiguration.Setup(x => x["USERS_MONGO_DATABASE_PORT"]).Returns($"{port}");
            _mockConfiguration.Setup(x => x["USERS_MONGO_DATABASE_USERNAME"]).Returns(username);
            _mockConfiguration.Setup(x => x["USERS_MONGO_DATABASE_PASSWORD"]).Returns(password);
            _mockConfiguration.Setup(x => x["AZURE_NOTIFICATION_HUB_PATH"]).Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["AZURE_NOTIFICATION_HUB_CONNECTION_STRING"]).Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["AZURE_NOTIFICATION_HUB_SHARED_ACCESS_KEY"]).Returns(_fixture.Create<string>());

            var serviceDescriptors = new List<ServiceDescriptor>();
            var mockServiceCollection = _fixture.Freeze<Mock<IServiceCollection>>();
            mockServiceCollection.Setup(x => x.Add(It.IsAny<ServiceDescriptor>()))
                .Callback<ServiceDescriptor>(x => serviceDescriptors.Add(x));

            // Act
            _systemUnderTest.ConfigureServices(mockServiceCollection.Object);

            // Assert
            serviceDescriptors.Should().NotBeEmpty();

            var mongoConfiguration =
                serviceDescriptors.FirstOrDefault(x => x.ImplementationInstance is IMongoConfiguration)
                    ?.ImplementationInstance as IMongoConfiguration;

            mongoConfiguration.Should().NotBeNull();
            mongoConfiguration.DatabaseName.Should().Be(databaseName);
            mongoConfiguration.UserDeviceCollectionName.Should().Be(userDeviceCollection);
            mongoConfiguration.Host.Should().Be(host);
            mongoConfiguration.Port.Should().Be(port);
            mongoConfiguration.Username.Should().Be(username);
            mongoConfiguration.Password.Should().Be(password);
        }
        
        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void ConfigureServices_AzureConfiguration_WhenHubPathIsInvalid_ThrowsException(string hubpath)
        {
            // Arrange
            _mockConfiguration.Setup(x => x["USERS_MONGO_DATABASE_NAME"]).Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["USERS_MONGO_DATABASE_USER_DEVICE_COLLECTION"])
                .Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["USERS_MONGO_DATABASE_HOST"]).Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["USERS_MONGO_DATABASE_PORT"]).Returns($"{_fixture.Create<int>()}");
            _mockConfiguration.Setup(x => x["AZURE_NOTIFICATION_HUB_PATH"]).Returns(hubpath);
            _mockConfiguration.Setup(x => x["AZURE_NOTIFICATION_HUB_CONNECTION_STRING"]).Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["AZURE_NOTIFICATION_HUB_SHARED_ACCESS_KEY"]).Returns(_fixture.Create<string>());

            // Act
            Action act = () => _fixture.Do<IServiceCollection>(x => _systemUnderTest.ConfigureServices(x));

            // Assert
            act.Should().Throw<ConfigurationNotFoundException>()
                .Which.Message.Should().Contain("AZURE_NOTIFICATION_HUB_PATH");
        }
        
        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void ConfigureServices_AzureConfiguration_WhenHubConnectionStringIsInvalid_ThrowsException(string hubConnectionString)
        {
            // Arrange
            _mockConfiguration.Setup(x => x["USERS_MONGO_DATABASE_NAME"]).Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["USERS_MONGO_DATABASE_USER_DEVICE_COLLECTION"])
                .Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["USERS_MONGO_DATABASE_HOST"]).Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["USERS_MONGO_DATABASE_PORT"]).Returns($"{_fixture.Create<int>()}");
            _mockConfiguration.Setup(x => x["AZURE_NOTIFICATION_HUB_PATH"]).Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["AZURE_NOTIFICATION_HUB_CONNECTION_STRING"]).Returns(hubConnectionString);
            _mockConfiguration.Setup(x => x["AZURE_NOTIFICATION_HUB_SHARED_ACCESS_KEY"]).Returns(_fixture.Create<string>());

            // Act
            Action act = () => _fixture.Do<IServiceCollection>(x => _systemUnderTest.ConfigureServices(x));

            // Assert
            act.Should().Throw<ConfigurationNotFoundException>()
                .Which.Message.Should().Contain("AZURE_NOTIFICATION_HUB_CONNECTION_STRING");
        }
        
        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void ConfigureServices_AzureConfiguration_WhenHubSharedKeyIsInvalid_ThrowsException(string sharedKey)
        {
            // Arrange
            _mockConfiguration.Setup(x => x["USERS_MONGO_DATABASE_NAME"]).Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["USERS_MONGO_DATABASE_USER_DEVICE_COLLECTION"])
                .Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["USERS_MONGO_DATABASE_HOST"]).Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["USERS_MONGO_DATABASE_PORT"]).Returns($"{_fixture.Create<int>()}");
            _mockConfiguration.Setup(x => x["AZURE_NOTIFICATION_HUB_PATH"]).Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["AZURE_NOTIFICATION_HUB_CONNECTION_STRING"]).Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["AZURE_NOTIFICATION_HUB_SHARED_ACCESS_KEY"]).Returns(sharedKey);

            // Act
            Action act = () => _fixture.Do<IServiceCollection>(x => _systemUnderTest.ConfigureServices(x));

            // Assert
            act.Should().Throw<ConfigurationNotFoundException>()
                .Which.Message.Should().Contain("AZURE_NOTIFICATION_HUB_SHARED_ACCESS_KEY");
        }
        
        [TestMethod]
        public void ConfigureServices_AzureConfiguration_WhenAllValuesAreProvided_MapsToProperties()
        {
            // Arrange
            var hubPath = _fixture.Create<string>();
            var hubConnectionString = _fixture.Create<string>();
            var sharedAccessKey = _fixture.Create<string>();

            _mockConfiguration.Setup(x => x["USERS_MONGO_DATABASE_NAME"]).Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["USERS_MONGO_DATABASE_USER_DEVICE_COLLECTION"])
                .Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["USERS_MONGO_DATABASE_HOST"]).Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["USERS_MONGO_DATABASE_PORT"]).Returns($"{_fixture.Create<int>()}");
            _mockConfiguration.Setup(x => x["USERS_MONGO_DATABASE_USERNAME"]).Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["USERS_MONGO_DATABASE_PASSWORD"]).Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["AZURE_NOTIFICATION_HUB_PATH"]).Returns(hubPath);
            _mockConfiguration.Setup(x => x["AZURE_NOTIFICATION_HUB_CONNECTION_STRING"]).Returns(hubConnectionString);
            _mockConfiguration.Setup(x => x["AZURE_NOTIFICATION_HUB_SHARED_ACCESS_KEY"]).Returns(sharedAccessKey);
            var serviceDescriptors = new List<ServiceDescriptor>();
            var mockServiceCollection = _fixture.Freeze<Mock<IServiceCollection>>();
            mockServiceCollection.Setup(x => x.Add(It.IsAny<ServiceDescriptor>()))
                .Callback<ServiceDescriptor>(x => serviceDescriptors.Add(x));

            // Act
            _systemUnderTest.ConfigureServices(mockServiceCollection.Object);

            // Assert
            serviceDescriptors.Should().NotBeEmpty();

            var azureConfiguration =
                serviceDescriptors.FirstOrDefault(x => x.ImplementationInstance is AzureNotificationConfiguration)
                    ?.ImplementationInstance as AzureNotificationConfiguration;

            azureConfiguration.Should().NotBeNull();
            azureConfiguration.NotificationHubPath.Should().Be(hubPath);
            azureConfiguration.ConnectionString.Should().Be(hubConnectionString);
            azureConfiguration.SharedAccessKey.Should().Be(sharedAccessKey);
        }
    }
}