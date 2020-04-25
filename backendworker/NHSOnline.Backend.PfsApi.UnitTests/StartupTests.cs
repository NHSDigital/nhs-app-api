using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.Support.Settings;
using UnitTestHelper;

namespace NHSOnline.Backend.PfsApi.UnitTests
{
    [TestClass]
    public class StartupTests
    {
        private IFixture _fixture;
        private Mock<IWebHostEnvironment> _mockIWebHostEnvironment;
        private Mock<ILoggerFactory> _mockILoggerFactory;
        private Mock<IServiceCollection> _mockIServiceCollection;
        private Startup _systemUnderTest;

        [TestMethod]
        public void Startup_Test_ConfigurationShouldThrowException_MissingConfigSetting()
        {
            // Arrange
            var isSuccess = false;

            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
            _mockIWebHostEnvironment = _fixture.Freeze<Mock<IWebHostEnvironment>>();
            _mockILoggerFactory = _fixture.Freeze<Mock<ILoggerFactory>>();
            _mockIServiceCollection = _fixture.Freeze<Mock<IServiceCollection>>();

            var configurationRoot = new ConfigurationBuilder()
                .AddJsonFile("appsettings.test.json")
                .Build();

            _systemUnderTest = new Startup(configurationRoot, _mockIWebHostEnvironment.Object, _mockILoggerFactory.Object);

            // Act
            try
            {
                _systemUnderTest.ConfigureServices(_mockIServiceCollection.Object);
            }
            catch (ConfigurationNotFoundException)
            {
                isSuccess = true;
            }
            catch (ArgumentNullException)
            {
                isSuccess = true;
            }

            // Assert
            isSuccess.Should().BeTrue();
        }
    }
}