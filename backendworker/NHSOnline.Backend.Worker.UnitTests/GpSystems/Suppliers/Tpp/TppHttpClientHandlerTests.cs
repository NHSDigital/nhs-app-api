using System;
using System.IO;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Tpp
{
    [TestClass]
    public class TppHttpClientHandlerTests
    {
        private TppHttpClientHandler _systemUnderTest;
        private Mock<IConfiguration> _mockConfiguration;
        private Mock<ILogger<TppHttpClientHandler>> _mockLogger;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _mockLogger = new Mock<ILogger<TppHttpClientHandler>>();
        }

        [TestMethod]
        public void Constructor_WhenProduction_DoesNotSetACustomValidationCallback()
        {
            _mockConfiguration.SetupGet(x => x["ASPNETCORE_ENVIRONMENT"]).Returns("Production");
            _systemUnderTest = new TppHttpClientHandler(_mockConfiguration.Object, _mockLogger.Object);
            _systemUnderTest.ServerCertificateCustomValidationCallback.Should().BeNull();
        }
        
        [TestMethod]
        public void Constructor_WhenNotProduction_SetsACustomValidationCallback()
        {
            _mockConfiguration.SetupGet(x => x["ASPNETCORE_ENVIRONMENT"]).Returns("Development");
            _systemUnderTest = new TppHttpClientHandler(_mockConfiguration.Object, _mockLogger.Object);
            _systemUnderTest.ServerCertificateCustomValidationCallback.Should().NotBeNull();
        }
        
        [DataTestMethod]
        [DataRow("", "boo")]      // Path empty
        [DataRow(null, "boo")]    // Path null
        [DataRow("./path", "")]   // Password empty
        [DataRow("./path", null)] // Password null
        [DataRow(null, null)]     // Both null
        [DataRow("", "")]         // Both empty
        public void Constructor_WhenCertifcatePathOrPasswordAreNullOrEmpty_DoesNotAddTheCertificateButLogsAWarning(string path, string password)
        {
            _mockConfiguration.SetupGet(x => x["TPP_CERTIFICATE_PATH"]).Returns(path);
            _mockConfiguration.SetupGet(x => x["TPP_CERTIFICATE_PASSWORD"]).Returns(password);
            
            _systemUnderTest = new TppHttpClientHandler(_mockConfiguration.Object, _mockLogger.Object);
            _systemUnderTest.ClientCertificates.Should().BeEmpty();
            _mockLogger.Verify(x => x.Log(
                LogLevel.Warning, (EventId) 0,
                It.Is<FormattedLogValues>(values => values.ToString() == "Could not add TPP client certificate due to missing certificate path or password."),
                null,
                It.IsAny<Func<object, Exception, string>>()
            ));
        }
    }
}