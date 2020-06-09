using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.PfsApi.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.GpSystems;

namespace NHSOnline.Backend.PfsApi.UnitTests.DependencyInjection
{
    [TestClass]
    public class SupplierStartupTests
    {
        private IFixture _fixture;

        private SupplierStartup _systemUnderTest;
        private Mock<ILoggerFactory> _logger;
        private Mock<IConfiguration> _configuration;
        private Mock<IGpSystemRegistrationService> _gpSystemRegistrationService;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _logger = _fixture.Freeze<Mock<ILoggerFactory>>();
            _configuration = _fixture.Freeze<Mock<IConfiguration>>();
            _gpSystemRegistrationService = _fixture.Freeze<Mock<IGpSystemRegistrationService>>();

            _fixture.Inject(_logger);

            _systemUnderTest = _fixture.Create<SupplierStartup>();
        }

        [TestMethod]
        public void ConfigureServices_SetsUpCorrectServicesForDependencyInjection_WhenMicrotestIsEnabled()
        {
            // Arrange
            var serviceCollection = new ServiceCollection();

            _configuration.Setup(x => x["GP_PROVIDER_ENABLED_MICROTEST"]).Returns("True");

            _gpSystemRegistrationService
                .Setup(x => x.RegisterPfsServices(serviceCollection, It.Is<EnableGpSupplierConfiguration>(config => config.EnableMicrotest)))
                .Verifiable();

            // Act
            _systemUnderTest.ConfigureServices(serviceCollection);

            // Assert
            _gpSystemRegistrationService.Verify();
        }

        [DataTestMethod]
        [DataRow("False")]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("xyz")]
        public void ConfigureServices_DoesntSetupMicrotestServicesForDependencyInjection_WhenMicrotestIsDisabled(string value)
        {
            // Arrange
            var serviceCollection = new ServiceCollection();

            _configuration.Setup(x => x["GP_PROVIDER_ENABLED_MICROTEST"]).Returns(value);

            _gpSystemRegistrationService
                .Setup(x => x.RegisterPfsServices(serviceCollection, It.Is<EnableGpSupplierConfiguration>(config => config.EnableMicrotest == false)))
                .Verifiable();

            // Act
            _systemUnderTest.ConfigureServices(serviceCollection);

            // Assert
            _gpSystemRegistrationService.Verify();
        }

        [TestMethod]
        public void ConfigureServices_SetsUpCorrectServicesForDependencyInjection_WhenFakeIsEnabled()
        {
            // Arrange
            var serviceCollection = new ServiceCollection();

            _configuration.Setup(x => x["GP_PROVIDER_ENABLED_FAKE"]).Returns("True");

            _gpSystemRegistrationService
                .Setup(x => x.RegisterPfsServices(serviceCollection, It.Is<EnableGpSupplierConfiguration>(config => config.EnableFake)))
                .Verifiable();

            // Act
            _systemUnderTest.ConfigureServices(serviceCollection);

            // Assert
            _gpSystemRegistrationService.Verify();
        }

        [DataTestMethod]
        [DataRow("False")]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("xyz")]
        public void ConfigureServices_DoesntSetupFakeServicesForDependencyInjection_WhenFakeIsDisabled(string value)
        {
            // Arrange
            var serviceCollection = new ServiceCollection();

            _configuration.Setup(x => x["GP_PROVIDER_ENABLED_FAKE"]).Returns(value);

            _gpSystemRegistrationService
                .Setup(x => x.RegisterPfsServices(serviceCollection, It.Is<EnableGpSupplierConfiguration>(config => config.EnableFake == false)))
                .Verifiable();

            // Act
            _systemUnderTest.ConfigureServices(serviceCollection);

            // Assert
            _gpSystemRegistrationService.Verify();
        }
    }
}
