using System;
using System.Net.Http;
using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Suppliers.Vision;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Linkage;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models.Linkage;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Session;
using NHSOnline.Backend.Support.ResponseParsers;
using RichardSzalay.MockHttp;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Vision
{
    
    [TestClass]
    public class VisionLoggingExtensionsTests
    {
        private Mock<ILogger<VisionSessionService>> _logger;
        private IFixture _fixture;
        
        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _logger =  _fixture.Freeze<Mock<ILogger<VisionSessionService>>>();
        }

        [TestMethod]
        public void Vision_LoggingExtension_Validate_SuccessfulErrorLog()
        {
            var expectedResponse = _fixture.Create<PatientConfigurationResponse>();

            _logger.Object.LogVisionErrorResponse(expectedResponse);

            _logger.Verify(
                m => m.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<FormattedLogValues>(v => v.ToString().Contains("Name", StringComparison.InvariantCulture)),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<object, Exception, string>>()
                )
            );

        }
    }
}