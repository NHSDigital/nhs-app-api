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
    }
}