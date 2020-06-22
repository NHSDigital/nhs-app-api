using System;
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

namespace NHSOnline.Backend.UserInfoApi.UnitTests
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
        public void ConfigureServices_WhenNhsAppApiKeyIsNotProvided_ThrowsException()
        {
            // Arrange
            _mockConfiguration.Setup(x => x["CITIZEN_ID_CLIENT_ID"]).Returns("Valid Value");
            _mockConfiguration.Setup(x => x["CITIZEN_ID_JWT_ISSUER"]).Returns("Valid Value");
            _mockConfiguration.Setup(x => x["CITIZEN_ID_BASE_URL"]).Returns("Valid Value");
            _mockConfiguration.Setup(x => x["ConfigurationSettings:DefaultHttpTimeoutSeconds"]).Returns("2");

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
            _mockConfiguration.Setup(x => x["CITIZEN_ID_CLIENT_ID"]).Returns("Valid Value");
           _mockConfiguration.Setup(x => x["CITIZEN_ID_JWT_ISSUER"]).Returns("Valid Value");
           _mockConfiguration.Setup(x => x["CITIZEN_ID_BASE_URL"]).Returns("https://authority.which.must.be.https.com/");
           _mockConfiguration.Setup(x => x["NHSAPP_API_KEY"]).Returns("Valid Value");
           _mockConfiguration.Setup(x => x["ConfigurationSettings:DefaultHttpTimeoutSeconds"]).Returns("2");
        }
    }
}