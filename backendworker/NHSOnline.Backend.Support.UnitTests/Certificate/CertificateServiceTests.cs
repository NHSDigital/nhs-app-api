using System;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Support.Certificate;

namespace NHSOnline.Backend.Support.UnitTests.Certificate
{
    [TestClass]
    public sealed class CertificateServiceTests
    {
        private IFixture _fixture;
        private Mock<ILogger<CertificateService>> _mockCertificateServiceLogger;

        private const string Path = "Certificate/Certs/mycert.pfx";
        private const string Passphrase = "password1";

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _mockCertificateServiceLogger = _fixture.Freeze<Mock<ILogger<CertificateService>>>();
        }

        [TestMethod]
        public void CreateCertificate()
        {
            var certificateService = new CertificateService(_mockCertificateServiceLogger.Object);
            var certificate = certificateService.GetCertificate(Path, Passphrase);
            certificate.Should().NotBeNull();
        }

        [DataTestMethod]
        [DataRow("", Passphrase, "Could not add client certificate due to missing certificate path.")]
        [DataRow(Path, "", "Could not add client certificate due to missing certificate passphrase.")]
        [DataRow("", "", "Could not add client certificate due to missing certificate path.")]
        public void InvalidCertificate_EmptyStringParameters(string path, string passphase, string error)
        {
            AssertInvalidCertificateMissingPath(path, passphase, error);
        }

        [DataTestMethod]
        [DataRow(null, Passphrase, "Could not add client certificate due to missing certificate path.")]
        [DataRow(Path, null, "Could not add client certificate due to missing certificate passphrase.")]
        [DataRow(null, null, "Could not add client certificate due to missing certificate path.")]
        // Cannot join empty string and null tests, as test report conflates null and empty
        // parameters into one test. We could miss failing tests if one fails and the other passes.
        public void InvalidCertificate_NullParameters(string path, string passphrase, string error)
        {
            AssertInvalidCertificateMissingPath(path, passphrase, error);
        }

        [TestMethod]
        public void InvalidCertificate_IncorrectPath()
        {
            AssertInvalidCertificateMissingPath(
                "GpSystems/Suppliers/Vision/Resources/wrongFile.pfx", 
                Passphrase,
                "Could not add client certificate due to file not existing in certificate path.");
        }

        private void AssertInvalidCertificateMissingPath(string path, string passphrase, string error)
        {
            var certificateService = new CertificateService(_mockCertificateServiceLogger.Object);
            var certificate = certificateService.GetCertificate(path, passphrase);
            certificate.Should().BeNull();
            _mockCertificateServiceLogger.Verify(x => x.Log(
                LogLevel.Error, 0,
                It.Is<FormattedLogValues>(values => values.ToString().Equals(error, StringComparison.Ordinal)),
                null,
                It.IsAny<Func<object, Exception, string>>()
            ));
        }
    }
}