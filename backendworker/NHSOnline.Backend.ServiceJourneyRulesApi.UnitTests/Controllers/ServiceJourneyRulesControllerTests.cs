using System;
using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.ServiceJourneyRulesApi.Controllers;
using NHSOnline.Backend.ServiceJourneyRulesApi.Models;
using NHSOnline.Backend.ServiceJourneyRulesApi.Service;
using ILoggerFactory = Microsoft.Extensions.Logging.ILoggerFactory;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.UnitTests.Controllers
{
    [TestClass]
    public class ServiceJourneyRulesControllerTests
    {
        private IFixture _fixture;
        
        private ServiceJourneyRulesController _systemUnderTest;
        private Mock<ILoggerFactory> _mockLogger;
        
        private Mock<IServiceJourneyRulesService> _mockServiceJourneyRulesService;

        private const string TestOdsCode = "A29928";

        private const string JourneyDisabled = "disabled";
        private const string AppointmentsJourney = "im1Appointments";
        
        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _mockLogger = _fixture.Freeze<Mock<ILoggerFactory>>();
            
            _mockServiceJourneyRulesService = new Mock<IServiceJourneyRulesService>();
            _systemUnderTest = new ServiceJourneyRulesController(_mockLogger.Object, _mockServiceJourneyRulesService.Object);
        }

        [TestMethod]
        public void CorrectResponseIsReturnedWhenAppointmentsJourneyIsDisabled()
        {
            _mockServiceJourneyRulesService.Setup(x => x.GetServiceJourneyRulesForOdsCode(TestOdsCode))
                .Returns(new ServiceJourneyRulesResponse()
                {
                    Appointments = new Appointments()
                    {
                        JourneyType = JourneyDisabled
                    }
                });

            var getResponse = _systemUnderTest.Get(TestOdsCode);
            Assert.IsTrue(getResponse.Value.Appointments.JourneyType.Equals(JourneyDisabled, StringComparison.Ordinal));
        }

        [TestMethod]
        public void CorrectResponseIsReturnedWhenJourneyIsIm1Appointments()
        {
            _mockServiceJourneyRulesService.Setup(x => x.GetServiceJourneyRulesForOdsCode(TestOdsCode))
                .Returns(new ServiceJourneyRulesResponse()
                {
                    Appointments = new Appointments()
                    {
                        JourneyType = AppointmentsJourney
                    }
                });

            var getResponse = _systemUnderTest.Get(TestOdsCode);
            Assert.IsTrue(getResponse.Value.Appointments.JourneyType.Equals(AppointmentsJourney, StringComparison.Ordinal));
        }
    }
}