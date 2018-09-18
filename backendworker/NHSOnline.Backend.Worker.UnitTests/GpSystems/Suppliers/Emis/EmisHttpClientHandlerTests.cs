using System;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Emis
{
    [TestClass]
    public sealed class EmisHttpClientHandlerTests : IDisposable
    {
        private EmisHttpClientHandler _systemUnderTest;
        private Mock<IConfiguration> _mockConfiguration;
        private Mock<ILogger<EmisHttpClientHandler>> _mockLogger;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _mockLogger = new Mock<ILogger<EmisHttpClientHandler>>();
        }

        [TestMethod]
        public void Constructor_WhenProduction_DoesNotSetACustomValidationCallback()
        {
            _mockConfiguration.SetupGet(x => x["ASPNETCORE_ENVIRONMENT"]).Returns("Production");
            _systemUnderTest = new EmisHttpClientHandler(_mockConfiguration.Object, _mockLogger.Object);
            _systemUnderTest.ServerCertificateCustomValidationCallback.Should().BeNull();
        }
        
        [TestMethod]
        public void Constructor_WhenNotProduction_SetsACustomValidationCallback()
        {
            _mockConfiguration.SetupGet(x => x["ASPNETCORE_ENVIRONMENT"]).Returns("Development");
            _systemUnderTest = new EmisHttpClientHandler(_mockConfiguration.Object, _mockLogger.Object);
            _systemUnderTest.ServerCertificateCustomValidationCallback.Should().NotBeNull();
        }
        
        [DataTestMethod]
        [DataRow("")]      // Path empty
        [DataRow(null)]    // Path null
        public void Constructor_WhenCertifcatePathIsNullOrEmpty_DoesNotAddTheCertificateButLogsAWarning(string path)
        {
            _mockConfiguration.SetupGet(x => x["EMIS_CERTIFICATE_PATH"]).Returns(path);
            
            _systemUnderTest = new EmisHttpClientHandler(_mockConfiguration.Object, _mockLogger.Object);
            _systemUnderTest.ClientCertificates.Should().BeEmpty();
            _mockLogger.Verify(x => x.Log(
                LogLevel.Warning, 0,
                It.Is<FormattedLogValues>(values => values.ToString()
                    .Equals("Could not add EMIS client certificate due to missing certificate path.",
                        StringComparison.Ordinal)),
                null,
                It.IsAny<Func<object, Exception, string>>()
            ));
        }

        [TestMethod]
        public void Constructor_WhenCertifcatePathIsValid_AddsTheCertificate()
        {
            const string path = "GpSystems/Suppliers/Emis/Resources/EMISTestCert.cer";
            _mockConfiguration.SetupGet(x => x["EMIS_CERTIFICATE_PATH"]).Returns(path);
            
            _systemUnderTest = new EmisHttpClientHandler(_mockConfiguration.Object, _mockLogger.Object);
            _systemUnderTest.ClientCertificates.Should().HaveCount(1);
        }

        public void Dispose()
        {
            _systemUnderTest.Dispose();
        }
    }
}