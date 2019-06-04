using System;
using System.Security.Cryptography.X509Certificates;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp;
using NHSOnline.Backend.Support.Certificate;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp
{
    [TestClass]
    public sealed class TppHttpClientHandlerTests : IDisposable
    {
        private IFixture _fixture;
        private TppHttpClientHandler _systemUnderTest;
        private Mock<IConfiguration> _mockConfiguration;
        private Mock<ILogger<TppHttpClientHandler>> _mockLogger;
        private Mock<ICertificateService> _certificateService;

        private const string Path = "Suppliers/Vision/Resources/mycert.pfx";
        private const string Passphrase = "password1";

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _mockConfiguration = new Mock<IConfiguration>();
            _mockLogger = new Mock<ILogger<TppHttpClientHandler>>();
            _certificateService = _fixture.Freeze<Mock<ICertificateService>>();
        }

        [TestMethod]
        public void Constructor_WhenProduction_DoesNotSetACustomValidationCallback()
        {
            _certificateService
                .Setup(x => x.GetCertificate(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new X509Certificate2(Path, Passphrase));

            _mockConfiguration.SetupGet(x => x["ASPNETCORE_ENVIRONMENT"]).Returns("Production");
            _systemUnderTest = CreateTppHttpClientHandler();
            _systemUnderTest.ServerCertificateCustomValidationCallback.Should().BeNull();
            _systemUnderTest.ClientCertificates.Should().NotBeEmpty();
        }

        [TestMethod]
        public void Constructor_WhenNotProduction_SetsACustomValidationCallback()
        {
            _certificateService
                .Setup(x => x.GetCertificate(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new X509Certificate2(Path, Passphrase));

            _mockConfiguration.SetupGet(x => x["ASPNETCORE_ENVIRONMENT"]).Returns("Development");
            _systemUnderTest = CreateTppHttpClientHandler();
            _systemUnderTest.ServerCertificateCustomValidationCallback.Should().NotBeNull();
        }

        [TestMethod]
        public void Constructor_WhenCertifcatePathIsValid_AddsTheCertificate()
        {
            _certificateService
                .Setup(x => x.GetCertificate(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new X509Certificate2(Path, Passphrase));

            _mockConfiguration.SetupGet(x => x["ASPNETCORE_ENVIRONMENT"]).Returns("Production");
            _systemUnderTest = CreateTppHttpClientHandler();
            _systemUnderTest.ServerCertificateCustomValidationCallback.Should().BeNull();
            _systemUnderTest.ClientCertificates.Should().NotBeEmpty();
            _systemUnderTest.ClientCertificates.Should().HaveCount(1);
        }

        [TestMethod]
        public void NoCertificateFound()
        {
            _certificateService.Setup(x => x.GetCertificate(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((X509Certificate2)null);
            _mockConfiguration.SetupGet(x => x["ASPNETCORE_ENVIRONMENT"]).Returns("Production");
            _systemUnderTest = CreateTppHttpClientHandler();
            _systemUnderTest.ClientCertificates.Should().BeEmpty();
        }

        private TppHttpClientHandler CreateTppHttpClientHandler()
        {
            return new TppHttpClientHandler(_mockConfiguration.Object,
                new TppConfigurationSettings 
                {
                    CertificatePath = Path,
                    CertificatePassphrase = Passphrase
                }, 
                _mockLogger.Object,
                _certificateService.Object);
        }

        public void Dispose()
        {
            _systemUnderTest.Dispose();
        }
    }
}
