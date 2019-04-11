using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.Support.Settings;
using UnitTestHelper;

namespace NHSOnline.Backend.PfsApi.UnitTests
{
    [TestClass]
    public class StartupTests
    {
        private IFixture _fixture;
        private Mock<IHostingEnvironment> _mockIHostingEnvironment;
        private Mock<ILoggerFactory> _mockILoggerFactory;
        private Mock<IServiceCollection> _mockIServiceCollection;
        private Startup _systemUnderTest;

        [TestMethod]
        public void Startup_Test_ConfigurationShouldThrowException_MissingConfigSetting()
        {
            var isSuccess = false;
            
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new ApiControllerAutoFixtureCustomization());
            _mockIHostingEnvironment = _fixture.Freeze<Mock<IHostingEnvironment>>();
            _mockILoggerFactory = _fixture.Freeze<Mock<ILoggerFactory>>();
            _mockIServiceCollection = _fixture.Freeze<Mock<IServiceCollection>>();

            _systemUnderTest = _fixture.Create<Startup>();
            
            var configurationRoot = new ConfigurationBuilder()
                .AddJsonFile("appsettings.test.json")
                .Build();
            
            _systemUnderTest = new Startup(configurationRoot, _mockIHostingEnvironment.Object, _mockILoggerFactory.Object);

            try
            {
                _systemUnderTest.ConfigureServices(_mockIServiceCollection.Object);
            }
            catch (ConfigurationNotFoundException)
            {
                isSuccess = true;
            }

            Assert.IsTrue(isSuccess);
        }
    }
}