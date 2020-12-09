using System;
using System.Security.Cryptography.X509Certificates;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest;
using NHSOnline.Backend.Support.Certificate;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Microtest
{
    [TestClass]
    public sealed class MicrotestHttpClientHandlerTests : IDisposable
    {
        private MicrotestHttpClientHandler _systemUnderTest;
        private Mock<ILogger<MicrotestHttpClientHandler>> _mockLogger;
        private Mock<ICertificateService> _certificateService;

        private const string Path = "Suppliers/Vision/Resources/mycert.pfx";
        private const string Passphrase = "password1";

        [TestInitialize]
        public void TestInitialize()
        {
            _mockLogger = new Mock<ILogger<MicrotestHttpClientHandler>>();
            _certificateService = new Mock<ICertificateService>();
        }

        [TestMethod]
        public void Constructor_WhenCertificateEnabledTrueAndValidPathAndPassphrase_AddsTheCertificate()
        {
            _certificateService
                .Setup(x => x.GetCertificate(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new X509Certificate2(Path, Passphrase));

            var configuration = new MicrotestConfigurationSettings(
                new Uri("http://example.com"),
                true,
                Path,
                Passphrase,
                0,
                0);
            _systemUnderTest = CreateMicrotestHttpClientHandler(configuration);
            _systemUnderTest.ServerCertificateCustomValidationCallback.Should().NotBeNull();
            _systemUnderTest.ClientCertificates.Should().NotBeEmpty();
            _systemUnderTest.ClientCertificates.Should().HaveCount(1);
        }

        [TestMethod]
        public void Constructor_WhenCertificateEnabledTrueAndInvalidPathAndPassphrase_DoesNotAddTheCertificate()
        {
            _certificateService.Setup(x => x.GetCertificate(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((X509Certificate2)null);

            var configuration = new MicrotestConfigurationSettings(
                new Uri("http://example.com"),
                true,
                string.Empty,
                string.Empty,
                0,
                0);
            _systemUnderTest = CreateMicrotestHttpClientHandler(configuration);
            _systemUnderTest.ClientCertificates.Should().BeEmpty();
        }

        [TestMethod]
        public void Constructor_WhenCertificateEnabledFalse_DoesNotAddTheCertificate()
        {
            _certificateService.Setup(x => x.GetCertificate(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((X509Certificate2)null);

            var configuration = new MicrotestConfigurationSettings(
                new Uri("http://example.com"),
                true,
                string.Empty,
                string.Empty,
                0,
                0);
            _systemUnderTest = CreateMicrotestHttpClientHandler(configuration);
            _systemUnderTest.ClientCertificates.Should().BeEmpty();
        }

        private MicrotestHttpClientHandler CreateMicrotestHttpClientHandler(MicrotestConfigurationSettings configuration)
        {
            return new MicrotestHttpClientHandler(configuration, _mockLogger.Object, _certificateService.Object);
        }

        public void Dispose()
        {
            _systemUnderTest.Dispose();
        }
    }
}