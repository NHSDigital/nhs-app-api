using System;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Support.Repository;
using NHSOnline.Backend.Support.Settings;
using UnitTestHelper;

namespace NHSOnline.Backend.MessagesApi.UnitTests
{
    [TestClass]
    public class StartupTests
    {
        private IFixture _fixture;
        private Mock<IConfiguration> _mockConfiguration;
        private Mock<IHostingEnvironment> _mockHostingEnvironment;
        private Startup _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            _mockConfiguration = _fixture.Freeze<Mock<IConfiguration>>();
            _mockHostingEnvironment = _fixture.Freeze<Mock<IHostingEnvironment>>();

            _systemUnderTest = _fixture.Create<Startup>();
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void ConfigureServices_MongoConfiguration_WhenConnectionStringIsInvalid_ThrowsException(string connectionString)
        {
            // Arrange
            _mockConfiguration.Setup(x => x["DEVICES_MONGO_CONNECTION_STRING"]).Returns(connectionString);
            _mockConfiguration.Setup(x => x["MESSAGES_MONGO_DATABASE_NAME"]).Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["MESSAGES_MONGO_DATABASE_MESSAGES_COLLECTION"])
                .Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["CITIZEN_ID_CLIENT_ID"]).Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["CITIZEN_ID_JWT_ISSUER"]).Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["CITIZEN_ID_BASE_URL"]).Returns(_fixture.Create<string>());

            // Act
            Action act = () => _fixture.Do<IServiceCollection>(x => _systemUnderTest.ConfigureServices(x));

            // Assert
            act.Should().Throw<ConfigurationNotValidException>()
                .Which.Message.Should().Contain("DEVICES_MONGO_CONNECTION_STRING");
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void ConfigureServices_MongoConfiguration_WhenDatabaseNameIsInvalid_ThrowsException(string databaseName)
        {
            // Arrange
            _mockConfiguration.Setup(x => x["DEVICES_MONGO_CONNECTION_STRING"]).Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["MESSAGES_MONGO_DATABASE_NAME"]).Returns(databaseName);
            _mockConfiguration.Setup(x => x["MESSAGES_MONGO_DATABASE_MESSAGES_COLLECTION"])
                .Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["CITIZEN_ID_CLIENT_ID"]).Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["CITIZEN_ID_JWT_ISSUER"]).Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["CITIZEN_ID_BASE_URL"]).Returns(_fixture.Create<string>());

            // Act
            Action act = () => _fixture.Do<IServiceCollection>(x => _systemUnderTest.ConfigureServices(x));

            // Assert
            act.Should().Throw<ConfigurationNotValidException>()
                .Which.Message.Should().Contain("MESSAGES_MONGO_DATABASE_NAME");
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void ConfigureServices_MongoConfiguration_WhenMessagesCollectionIsInvalid_ThrowsException
        (
            string messagesCollection
        )
        {
            // Arrange
            _mockConfiguration.Setup(x => x["DEVICES_MONGO_CONNECTION_STRING"]).Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["MESSAGES_MONGO_DATABASE_NAME"]).Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["MESSAGES_MONGO_DATABASE_MESSAGES_COLLECTION"])
                .Returns(messagesCollection);
            _mockConfiguration.Setup(x => x["CITIZEN_ID_CLIENT_ID"]).Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["CITIZEN_ID_JWT_ISSUER"]).Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["CITIZEN_ID_BASE_URL"]).Returns(_fixture.Create<string>());

            // Act
            Action act = () => _fixture.Do<IServiceCollection>(x => _systemUnderTest.ConfigureServices(x));

            // Assert
            act.Should().Throw<ConfigurationNotValidException>()
                .Which.Message.Should().Contain("MESSAGES_MONGO_DATABASE_MESSAGES_COLLECTION");
        }

        [TestMethod]
        public void ConfigureServices_MongoConfiguration_WhenAllValuesAreProvided_MapsToProperties()
        {
            // Arrange
            var connectionString = _fixture.Create<string>();
            var databaseName = _fixture.Create<string>();
            var messagesCollection = _fixture.Create<string>();

            _mockConfiguration.Setup(x => x["DEVICES_MONGO_CONNECTION_STRING"]).Returns(connectionString);
            _mockConfiguration.Setup(x => x["MESSAGES_MONGO_DATABASE_NAME"]).Returns(databaseName);
            _mockConfiguration.Setup(x => x["MESSAGES_MONGO_DATABASE_MESSAGES_COLLECTION"])
                .Returns(messagesCollection);
            _mockConfiguration.Setup(x => x["CITIZEN_ID_CLIENT_ID"]).Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["CITIZEN_ID_JWT_ISSUER"]).Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["CITIZEN_ID_BASE_URL"]).Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["AUDIT_SINK_TYPE"]).Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["NHSAPP_API_KEY"]).Returns(_fixture.Create<string>());

            var mockServiceCollection = _fixture.Create<Mock<IServiceCollection>>();
            var serviceDescriptors = ServiceCollectionHelper.SetupServiceDescriptor(mockServiceCollection);

            // Act
            _systemUnderTest.ConfigureServices(mockServiceCollection.Object);

            // Assert
            serviceDescriptors.Should().NotBeEmpty();

            var mongoConfiguration =
                serviceDescriptors.FirstOrDefault(x => x.ImplementationInstance is IMongoConfiguration)
                    ?.ImplementationInstance as IMongoConfiguration;

            using (new AssertionScope())
            {
                mongoConfiguration.Should().NotBeNull();
                mongoConfiguration.ConnectionString.Should().Be(connectionString);
                mongoConfiguration.DatabaseName.Should().Be(databaseName);
                mongoConfiguration.CollectionName.Should().Be(messagesCollection);
            }
        }

        [TestMethod]
        public void ConfigureServices_WhenNhsAppApiKeyIsNotProvided_ThrowsException()
        {
            // Arrange
            _mockConfiguration.Setup(x => x["DEVICES_MONGO_CONNECTION_STRING"]).Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["MESSAGES_MONGO_DATABASE_NAME"]).Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["MESSAGES_MONGO_DATABASE_MESSAGES_COLLECTION"])
                .Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["CITIZEN_ID_CLIENT_ID"]).Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["CITIZEN_ID_JWT_ISSUER"]).Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["CITIZEN_ID_BASE_URL"]).Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["AUDIT_SINK_TYPE"]).Returns(_fixture.Create<string>());

            // Act
            Action act = () => _fixture.Do<IServiceCollection>(x => _systemUnderTest.ConfigureServices(x));

            // Assert
            act.Should().Throw<ConfigurationNotValidException>()
                .Which.Message.Should().Contain("NHSAPP_API_KEY");
        }


        [TestMethod]
        public void ConfigureServices_ConfigureAuth_WhenInProduction_ShouldRequireHttpsMetadata()
        {
            // Arrange
            SetupAllConfiguration();

            _mockHostingEnvironment.SetupGet(x => x.EnvironmentName).Returns(EnvironmentName.Production);

            var mockServiceCollection = _fixture.Create<Mock<IServiceCollection>>();
            var serviceDescriptors = ServiceCollectionHelper.SetupServiceDescriptor(mockServiceCollection);

            // Act
            _systemUnderTest.ConfigureServices(mockServiceCollection.Object);

            // Assert
            serviceDescriptors.Should().NotBeEmpty();

            var configureJwtBearerOptions =
                serviceDescriptors
                    .FirstOrDefault(x => x.ImplementationInstance is IConfigureNamedOptions<JwtBearerOptions>)
                    ?.ImplementationInstance as IConfigureNamedOptions<JwtBearerOptions>;

            configureJwtBearerOptions.Should().NotBeNull();
            var jwtBearerOptions = new JwtBearerOptions();

            configureJwtBearerOptions.Configure("Bearer", jwtBearerOptions);

            jwtBearerOptions.RequireHttpsMetadata.Should().BeTrue();
        }

        [TestMethod]
        public void ConfigureServices_ConfigureAuth_WhenInDevelopment_ShouldNotRequireHttpsMetadata()
        {
            // Arrange
            SetupAllConfiguration();

            _mockHostingEnvironment.SetupGet(x => x.EnvironmentName).Returns(EnvironmentName.Development);

            var mockServiceCollection = _fixture.Create<Mock<IServiceCollection>>();
            var serviceDescriptors = ServiceCollectionHelper.SetupServiceDescriptor(mockServiceCollection);

            // Act
            _systemUnderTest.ConfigureServices(mockServiceCollection.Object);

            // Assert
            serviceDescriptors.Should().NotBeEmpty();

            var configureJwtBearerOptions =
                serviceDescriptors
                    .FirstOrDefault(x => x.ImplementationInstance is IConfigureNamedOptions<JwtBearerOptions>)
                    ?.ImplementationInstance as IConfigureNamedOptions<JwtBearerOptions>;

            configureJwtBearerOptions.Should().NotBeNull();
            var jwtBearerOptions = new JwtBearerOptions();

            configureJwtBearerOptions.Configure("Bearer", jwtBearerOptions);

            jwtBearerOptions.RequireHttpsMetadata.Should().BeFalse();
        }

        private void SetupAllConfiguration()
        {
            _mockConfiguration.Setup(x => x["DEVICES_MONGO_CONNECTION_STRING"]).Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["MESSAGES_MONGO_DATABASE_NAME"]).Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["MESSAGES_MONGO_DATABASE_MESSAGES_COLLECTION"])
                .Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["CITIZEN_ID_CLIENT_ID"]).Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["CITIZEN_ID_JWT_ISSUER"]).Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["CITIZEN_ID_BASE_URL"]).Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["AUDIT_SINK_TYPE"]).Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["NHSAPP_API_KEY"]).Returns(_fixture.Create<string>());
        }
    }
}