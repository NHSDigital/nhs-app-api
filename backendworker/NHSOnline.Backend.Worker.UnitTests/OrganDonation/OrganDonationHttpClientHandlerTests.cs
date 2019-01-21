using System;
using System.Security.Cryptography.X509Certificates;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.OrganDonation;
using NHSOnline.Backend.Worker.Support.Certificate;

namespace NHSOnline.Backend.Worker.UnitTests.OrganDonation
{
    [TestClass]
    public sealed class OrganDonationHttpClientHandlerTests : IDisposable
    {
        private IFixture _fixture;
        private OrganDonationHttpClientHandler _systemUnderTest;
        private Mock<IConfiguration> _mockConfiguration;
        private ILogger<OrganDonationHttpClientHandler> _mockLogger;
        private Mock<ICertificateService> _certificateService;

        private const string Path = "OrganDonation/Resources/mycert.pfx";
        private const string Passphrase = "password1";

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _mockLogger = _fixture.Create<ILogger<OrganDonationHttpClientHandler>>();

            _mockConfiguration = new Mock<IConfiguration>();
            _certificateService = _fixture.Freeze<Mock<ICertificateService>>();
        }

        [TestMethod]
        public void Constructor_WhenProduction_DoesNotSetACustomValidationCallback()
        {
            _certificateService
                .Setup(x => x.GetCertificate(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new X509Certificate2(Path, Passphrase));

            _mockConfiguration.SetupGet(x => x["ASPNETCORE_ENVIRONMENT"]).Returns("Production");
            _systemUnderTest = CreateOrganDonationHttpClientHandler();
            _systemUnderTest.ServerCertificateCustomValidationCallback.Should().BeNull();
        }

        [TestMethod]
        public void Constructor_WhenNotProduction_SetsACustomValidationCallback()
        {
            _certificateService
                .Setup(x => x.GetCertificate(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new X509Certificate2(Path, Passphrase));

            _mockConfiguration.SetupGet(x => x["ASPNETCORE_ENVIRONMENT"]).Returns("Development");
            _systemUnderTest = CreateOrganDonationHttpClientHandler();
            _systemUnderTest.ServerCertificateCustomValidationCallback.Should().NotBeNull();
        }

        [TestMethod]
        public void Constructor_WhenCertifcatePathIsValid_AddsTheCertificate()
        {
            _certificateService
                .Setup(x => x.GetCertificate(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new X509Certificate2(Path, Passphrase));

            _mockConfiguration.SetupGet(x => x["ASPNETCORE_ENVIRONMENT"]).Returns("Production");
            _systemUnderTest = CreateOrganDonationHttpClientHandler();
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
            _systemUnderTest = CreateOrganDonationHttpClientHandler();
            _systemUnderTest.ClientCertificates.Should().BeEmpty();

        }

        private OrganDonationHttpClientHandler CreateOrganDonationHttpClientHandler()
        {
            return new OrganDonationHttpClientHandler(_mockConfiguration.Object, _mockLogger, _certificateService.Object);
        }

        public void Dispose()
        {
            _systemUnderTest.Dispose();
        }
    }
}