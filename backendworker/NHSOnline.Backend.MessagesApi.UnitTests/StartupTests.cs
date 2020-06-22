using System;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Support.Settings;
using UnitTestHelper;

namespace NHSOnline.Backend.MessagesApi.UnitTests
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
        public void ConfigureServices_WhenNhsAppApiKeyIsNotProvided_ThrowsException()
        {
            // Arrange
            _mockConfiguration.Setup(x => x["CITIZEN_ID_CLIENT_ID"]).Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["CITIZEN_ID_JWT_ISSUER"]).Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["CITIZEN_ID_BASE_URL"]).Returns(_fixture.Create<string>());

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

            _mockHostingEnvironment.SetupGet(x => x.EnvironmentName).Returns(Environments.Production);

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

            _mockHostingEnvironment.SetupGet(x => x.EnvironmentName).Returns(Environments.Development);

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
            _mockConfiguration.Setup(x => x["CITIZEN_ID_CLIENT_ID"]).Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["CITIZEN_ID_JWT_ISSUER"]).Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["CITIZEN_ID_BASE_URL"]).Returns(_fixture.Create<string>());
            _mockConfiguration.Setup(x => x["NHSAPP_API_KEY"]).Returns(_fixture.Create<string>());
        }
    }
}