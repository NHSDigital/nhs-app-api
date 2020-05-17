using System.Collections.Generic;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Support.Certificate;
using UnitTestHelper;

namespace NHSOnline.Backend.Support.UnitTests.Certificate
{
    [TestClass]
    public sealed class CertificateServiceTests
    {
        private IFixture _fixture;
        private Mock<ILogger<CertificateService>> _mockCertificateServiceLogger;
        private Mock<IConfiguration> _mockConfiguration;

        private Mock<object> _mockSender;
        private Mock<X509Certificate> _mockCertificate;
        private Mock<X509Chain> _mockChain;

        private const string Path = "Certificate/Certs/mycert.pfx";
        private const string Passphrase = "password1";

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _mockCertificateServiceLogger = _fixture.Freeze<Mock<ILogger<CertificateService>>>();
            _mockConfiguration = _fixture.Freeze<Mock<IConfiguration>>();

            _mockSender = _fixture.Freeze<Mock<object>>();
            _mockCertificate = _fixture.Freeze<Mock<X509Certificate>>();
            _mockChain = _fixture.Freeze<Mock<X509Chain>>();
        }

        [TestMethod]
        public void CreateCertificate()
        {
            var certificateService = new CertificateService(_mockCertificateServiceLogger.Object, _mockConfiguration.Object);
            var certificate = certificateService.GetCertificate(Path, Passphrase);
            certificate.Should().NotBeNull();
        }

        [DataTestMethod]
        [DataRow("", Passphrase, "Could not add client certificate due to missing certificate path.")]
        [DataRow(Path, "", "Could not add client certificate due to missing certificate passphrase.")]
        [DataRow("", "", "Could not add client certificate due to missing certificate path.")]
        public void InvalidCertificate_EmptyStringParameters(string path, string passPhase, string error)
        {
            AssertInvalidCertificateMissingPath(path, passPhase, error);
        }

        [DataTestMethod]
        [DataRow(null, Passphrase, "Could not add client certificate due to missing certificate path.")]
        [DataRow(Path, null, "Could not add client certificate due to missing certificate passphrase.")]
        [DataRow(null, null, "Could not add client certificate due to missing certificate path.")]
        // Cannot join empty string and null tests, as test report conflates null and empty
        // parameters into one test. We could miss failing tests if one fails and the other passes.
        public void InvalidCertificate_NullParameters(string path, string passPhrase, string error)
        {
            AssertInvalidCertificateMissingPath(path, passPhrase, error);
        }

        [TestMethod]
        public void InvalidCertificate_IncorrectPath()
        {
            var certificateService = new CertificateService(_mockCertificateServiceLogger.Object, _mockConfiguration.Object);
            var certificate = certificateService.GetCertificate("GpSystems/Suppliers/Vision/Resources/wrongFile.pfx", Passphrase);
            certificate.Should().BeNull();
            _mockCertificateServiceLogger.VerifyLogger(
                LogLevel.Error,
                "Could not add client certificate due to file GpSystems/Suppliers/Vision/Resources/wrongFile.pfx not existing in certificate path.",
                Times.Once());
        }

        [DataTestMethod]
        [DataRow(SslPolicyErrors.None, "Production", true)]
        [DataRow(SslPolicyErrors.RemoteCertificateNameMismatch, "Production", false)]
        [DataRow(SslPolicyErrors.RemoteCertificateNotAvailable, "Production", false)]
        [DataRow(SslPolicyErrors.RemoteCertificateChainErrors, "Production", false)]
        [DataRow(SslPolicyErrors.None, "Development", true)]
        [DataRow(SslPolicyErrors.RemoteCertificateNameMismatch, "Productionn", false)]
        [DataRow(SslPolicyErrors.RemoteCertificateNameMismatch, "Development", true)]
        [DataRow(SslPolicyErrors.RemoteCertificateNotAvailable, "Development", false)]
        [DataRow(SslPolicyErrors.RemoteCertificateChainErrors, "Development", false)]
        [DataRow(SslPolicyErrors.RemoteCertificateNameMismatch | SslPolicyErrors.RemoteCertificateChainErrors, "Develop", false)]
        public void ServerCertificateValidationHandlerWithError(SslPolicyErrors error, string environment, bool output)
        {
            IConfigurationBuilder configBuilder = new ConfigurationBuilder();
            configBuilder.AddInMemoryCollection(new[] { new KeyValuePair<string, string>("ASPNETCORE_ENVIRONMENT", environment) });
            var certificateService = new CertificateService(_mockCertificateServiceLogger.Object, configBuilder.Build());

            var outcome =
                certificateService.ServerCertificateValidationHandler(_mockSender, _mockCertificate.Object, _mockChain.Object, error);

            Assert.AreEqual(output, outcome);

        }

        private void AssertInvalidCertificateMissingPath(string path, string passphrase, string error)
        {
            var certificateService = new CertificateService(_mockCertificateServiceLogger.Object, _mockConfiguration.Object);
            var certificate = certificateService.GetCertificate(path, passphrase);
            certificate.Should().BeNull();
            _mockCertificateServiceLogger.VerifyLogger(LogLevel.Error, error, Times.Once());
        }
    }
}