using System;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Repository;
using NHSOnline.Backend.Support.Settings;

namespace NHSOnline.Backend.UserInfoApi.UnitTests
{
    [TestClass]
    public class StartupTests
    {
        private IFixture _fixture;
        private Mock<IConfiguration> _mockConfiguration;
        private Mock<IWebHostEnvironment> _mockHostingEnvironment;
        private Startup _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            _mockConfiguration = _fixture.Freeze<Mock<IConfiguration>>();
            _mockHostingEnvironment = _fixture.Freeze<Mock<IWebHostEnvironment>>();

            _systemUnderTest = _fixture.Create<Startup>();
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void ConfigureServices_MongoConfiguration_WhenConnectionStringIsInvalid_ThrowsException(string connectionString)
        {
            // Arrange
            _mockConfiguration.Setup(x => x["DEVICES_MONGO_CONNECTION_STRING"]).Returns(connectionString);
            _mockConfiguration.Setup(x => x["USERINFO_MONGO_DATABASE_NAME"]).Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["USERINFO_MONGO_DATABASE_COLLECTION"])
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
            _mockConfiguration.Setup(x => x["USERINFO_MONGO_DATABASE_NAME"]).Returns(databaseName);
            _mockConfiguration.Setup(x => x["USERINFO_MONGO_DATABASE_COLLECTION"])
                .Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["CITIZEN_ID_CLIENT_ID"]).Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["CITIZEN_ID_JWT_ISSUER"]).Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["CITIZEN_ID_BASE_URL"]).Returns(_fixture.Create<string>());

            // Act
            Action act = () => _fixture.Do<IServiceCollection>(x => _systemUnderTest.ConfigureServices(x));

            // Assert
            act.Should().Throw<ConfigurationNotValidException>()
                .Which.Message.Should().Contain("USERINFO_MONGO_DATABASE_NAME");
        }
        [TestMethod]
        public void ConfigureServices_WhenNhsAppApiKeyIsNotProvided_ThrowsException()
        {
            // Arrange
            _mockConfiguration.Setup(x => x["DEVICES_MONGO_CONNECTION_STRING"]).Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["USERINFO_MONGO_DATABASE_NAME"]).Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["USERINFO_MONGO_DATABASE_COLLECTION"])
                .Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["USERINFO_MONGO_DATABASE_NAME"])
                .Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["CITIZEN_ID_CLIENT_ID"]).Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["CITIZEN_ID_JWT_ISSUER"]).Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["CITIZEN_ID_BASE_URL"]).Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["ConfigurationSettings:DefaultHttpTimeoutSeconds"]).Returns("2");

            // Act
            Action act = () => _fixture.Do<IServiceCollection>(x => _systemUnderTest.ConfigureServices(x));

            // Assert
            act.Should().Throw<ConfigurationNotValidException>()
                .Which.Message.Should().Contain("NHSAPP_API_KEY");
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
            _mockConfiguration.Setup(x => x["USERINFO_MONGO_DATABASE_NAME"]).Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["USERINFO_MONGO_DATABASE_COLLECTION"])
                .Returns(messagesCollection);
            _mockConfiguration.Setup(x => x["CITIZEN_ID_CLIENT_ID"]).Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["CITIZEN_ID_JWT_ISSUER"]).Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["CITIZEN_ID_BASE_URL"]).Returns(_fixture.Create<string>());

            // Act
            Action act = () => _fixture.Do<IServiceCollection>(x => _systemUnderTest.ConfigureServices(x));

            // Assert
            act.Should().Throw<ConfigurationNotValidException>()
                .Which.Message.Should().Contain("USERINFO_MONGO_DATABASE_COLLECTION");
        }

        [TestMethod]
        public void ConfigureServices_MongoConfiguration_WhenAllValuesAreProvided_MapsToProperties()
        {
            // Arrange
            var connectionString = _fixture.Create<string>();
            var databaseName = _fixture.Create<string>();
            var messsagesCollection = _fixture.Create<string>();

            _mockConfiguration.Setup(x => x["DEVICES_MONGO_CONNECTION_STRING"]).Returns(connectionString);
            _mockConfiguration.Setup(x => x["USERINFO_MONGO_DATABASE_NAME"]).Returns(databaseName);
            _mockConfiguration.Setup(x => x["USERINFO_MONGO_DATABASE_COLLECTION"])
                .Returns(messsagesCollection);
            _mockConfiguration.Setup(x => x["CITIZEN_ID_CLIENT_ID"]).Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["CITIZEN_ID_JWT_ISSUER"]).Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["CITIZEN_ID_BASE_URL"]).Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["AUDIT_SINK_TYPE"]).Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["NHSAPP_API_KEY"]).Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["ConfigurationSettings:DefaultHttpTimeoutSeconds"]).Returns("2");

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddHttpClient();

            // Act
            _systemUnderTest.ConfigureServices(serviceCollection);

            // Assert
            var mongoConfiguration = serviceCollection.BuildServiceProvider().GetRequiredService<IMongoConfiguration>();

            using (new AssertionScope())
            {
                mongoConfiguration.Should().NotBeNull();
                mongoConfiguration.ConnectionString.Should().Be(connectionString);
                mongoConfiguration.DatabaseName.Should().Be(databaseName);
                mongoConfiguration.CollectionName.Should().Be(messsagesCollection);
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
            _mockConfiguration.Setup(x => x["DEVICES_MONGO_CONNECTION_STRING"]).Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["USERINFO_MONGO_DATABASE_NAME"]).Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["USERINFO_MONGO_DATABASE_COLLECTION"])
                .Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["CITIZEN_ID_CLIENT_ID"]).Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["CITIZEN_ID_JWT_ISSUER"]).Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["CITIZEN_ID_BASE_URL"]).Returns("https://authority.which.must.be.https.com/");
            _mockConfiguration.Setup(x => x["AUDIT_SINK_TYPE"]).Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["NHSAPP_API_KEY"]).Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["ConfigurationSettings:DefaultHttpTimeoutSeconds"]).Returns("2");
        }
    }
}