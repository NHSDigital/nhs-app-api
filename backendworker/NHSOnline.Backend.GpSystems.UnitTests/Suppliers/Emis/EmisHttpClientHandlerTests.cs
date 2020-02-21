using System;
using System.Security.Cryptography.X509Certificates;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.Support.Certificate;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Emis
{
    [TestClass]
    public sealed class EmisHttpClientHandlerTests : IDisposable
    {
        private IFixture _fixture;
        private EmisHttpClientHandler _systemUnderTest;
        private EmisConfigurationSettings _config;
        private ILogger<EmisHttpClientHandler> _mockLogger;
        private Mock<ICertificateService> _certificateService;

        public const string DefaultEmisVersion = "2.1.0.0";
        public static readonly string DefaultEmisApplicationId = Guid.NewGuid().ToString();

        public static readonly Uri BaseUri = new Uri("http://emis_base_url/");

        private const string CertificatePath = "CertificatePath";
        private const string CertificatePassphrase = "CertificatePassphrase";       

        private const int CoursesMaxCoursesLimit = 100;
        private const int EmisExtendedHttpTimeoutSeconds = 6;
        private const int DefaultHttpTimeoutSeconds = 2;
        private const int PrescriptionsMaxCoursesSoftLimit = 100;
        private const string Path = "Suppliers/Vision/Resources/mycert.pfx";
        private const string Passphrase = "password1";
        private const string Environment = "testEnv";

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _mockLogger = _fixture.Create<ILogger<EmisHttpClientHandler>>();

            _config = new EmisConfigurationSettings(BaseUri, DefaultEmisApplicationId, DefaultEmisVersion, CertificatePath, 
                CertificatePassphrase, EmisExtendedHttpTimeoutSeconds, DefaultHttpTimeoutSeconds, CoursesMaxCoursesLimit, PrescriptionsMaxCoursesSoftLimit, Environment);
            
            _certificateService = _fixture.Freeze<Mock<ICertificateService>>();
        }

        [TestMethod]
        public void Constructor_WhenProduction_DoesNotSetACustomValidationCallback()
        {
            _certificateService
                .Setup(x => x.GetCertificate(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new X509Certificate2(Path, Passphrase));
            
            _systemUnderTest = CreateEmisHttpClientHandler();
            _systemUnderTest.ServerCertificateCustomValidationCallback.Should().NotBeNull();
        }

        [TestMethod]
        public void Constructor_WhenNotProduction_SetsACustomValidationCallback()
        {
            _certificateService
                .Setup(x => x.GetCertificate(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new X509Certificate2(Path, Passphrase));

            _systemUnderTest = CreateEmisHttpClientHandler();
            _systemUnderTest.ServerCertificateCustomValidationCallback.Should().NotBeNull();
        }

        [TestMethod]
        public void Constructor_WhenCertificatePathIsValid_AddsTheCertificate()
        {
            _certificateService
                .Setup(x => x.GetCertificate(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new X509Certificate2(Path, Passphrase));

            _systemUnderTest = CreateEmisHttpClientHandler();
            _systemUnderTest.ServerCertificateCustomValidationCallback.Should().NotBeNull();
            _systemUnderTest.ClientCertificates.Should().NotBeEmpty();
            _systemUnderTest.ClientCertificates.Should().HaveCount(1);
        }

        [TestMethod]
        public void NoCertificateFound()
        {
            _certificateService.Setup(x => x.GetCertificate(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((X509Certificate2)null);

            _systemUnderTest = CreateEmisHttpClientHandler();
            _systemUnderTest.ClientCertificates.Should().BeEmpty();
        }

        private EmisHttpClientHandler CreateEmisHttpClientHandler()
        {
            return new EmisHttpClientHandler( _config, _mockLogger, _certificateService.Object);
        }

        public void Dispose()
        {
            _systemUnderTest.Dispose();
        }
    }
}