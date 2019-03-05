using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.CidApi.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.GpSystems;

namespace NHSOnline.Backend.CidApi.UnitTests.DependencyInjection
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
        public void TestInitialise()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _logger = _fixture.Freeze<Mock<ILoggerFactory>>();
            _configuration = _fixture.Freeze<Mock<IConfiguration>>();
            _gpSystemRegistrationService = _fixture.Freeze<Mock<IGpSystemRegistrationService>>();

            _fixture.Inject(_logger);

            _systemUnderTest = _fixture.Create<SupplierStartup>();
        }

        [TestMethod]
        public void ConfigureServices_SetsUpCorrectServicesForDependencyInjection_WhenEmisIsEnabled()
        {
            // Arrange
            var serviceCollection = new ServiceCollection();

            _configuration.Setup(x => x["GP_PROVIDER_ENABLED_EMIS"]).Returns("True");

            _gpSystemRegistrationService
                .Setup(x => x.RegisterCidServices(serviceCollection, It.Is<EnableGpSupplierConfiguration>(config => config.EnableEmis == true)))
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
        public void ConfigureServices_DoesntSetupEmisServicesForDependencyInjection_WhenEmisIsDisabled(string value)
        {
            // Arrange
            var serviceCollection = new ServiceCollection();

            _configuration.Setup(x => x["GP_PROVIDER_ENABLED_EMIS"]).Returns(value);

            _gpSystemRegistrationService
                .Setup(x => x.RegisterCidServices(serviceCollection, It.Is<EnableGpSupplierConfiguration>(config => config.EnableEmis == false)))
                .Verifiable();

            // Act
            _systemUnderTest.ConfigureServices(serviceCollection);

            // Assert
            _gpSystemRegistrationService.Verify();
        }

        [TestMethod]
        public void ConfigureServices_SetsUpCorrectServicesForDependencyInjection_WhenVisinoIsEnabled()
        {
            // Arrange
            var serviceCollection = new ServiceCollection();

            _configuration.Setup(x => x["GP_PROVIDER_ENABLED_VISION"]).Returns("True");

            _gpSystemRegistrationService
                .Setup(x => x.RegisterCidServices(serviceCollection, It.Is<EnableGpSupplierConfiguration>(config => config.EnableVision == true)))
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
        public void ConfigureServices_DoesntSetupVisionServicesForDependencyInjection_WhenVisionIsDisabled(string value)
        {
            // Arrange
            var serviceCollection = new ServiceCollection();

            _configuration.Setup(x => x["GP_PROVIDER_ENABLED_VISION"]).Returns(value);

            _gpSystemRegistrationService
                .Setup(x => x.RegisterCidServices(serviceCollection, It.Is<EnableGpSupplierConfiguration>(config => config.EnableVision == false)))
                .Verifiable();

            // Act
            _systemUnderTest.ConfigureServices(serviceCollection);

            // Assert
            _gpSystemRegistrationService.Verify();
        }

        [TestMethod]
        public void ConfigureServices_SetsUpCorrectServicesForDependencyInjection_WhenTppIsEnabled()
        {
            // Arrange
            var serviceCollection = new ServiceCollection();

            _configuration.Setup(x => x["GP_PROVIDER_ENABLED_TPP"]).Returns("True");

            _gpSystemRegistrationService
                .Setup(x => x.RegisterCidServices(serviceCollection, It.Is<EnableGpSupplierConfiguration>(config => config.EnableTpp == true)))
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
        public void ConfigureServices_DoesntSetupEmisServicesForDependencyInjection_WhenTppIsDisabled(string value)
        {
            // Arrange
            var serviceCollection = new ServiceCollection();

            _configuration.Setup(x => x["GP_PROVIDER_ENABLED_TPP"]).Returns(value);

            _gpSystemRegistrationService
                .Setup(x => x.RegisterCidServices(serviceCollection, It.Is<EnableGpSupplierConfiguration>(config => config.EnableTpp == false)))
                .Verifiable();

            // Act
            _systemUnderTest.ConfigureServices(serviceCollection);

            // Assert
            _gpSystemRegistrationService.Verify();
        }

        [TestMethod]
        public void ConfigureServices_SetsUpCorrectServicesForDependencyInjection_WhenMicrotestIsEnabled()
        {
            // Arrange
            var serviceCollection = new ServiceCollection();

            _configuration.Setup(x => x["GP_PROVIDER_ENABLED_MICROTEST"]).Returns("True");

            _gpSystemRegistrationService
                .Setup(x => x.RegisterCidServices(serviceCollection, It.Is<EnableGpSupplierConfiguration>(config => config.EnableMicrotest == true)))
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
                .Setup(x => x.RegisterCidServices(serviceCollection, It.Is<EnableGpSupplierConfiguration>(config => config.EnableMicrotest == false)))
                .Verifiable();

            // Act
            _systemUnderTest.ConfigureServices(serviceCollection);

            // Assert
            _gpSystemRegistrationService.Verify();
        }
    }
}
