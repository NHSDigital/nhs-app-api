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
        
        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _mockLogger = _fixture.Freeze<Mock<ILoggerFactory>>();
            
            _mockServiceJourneyRulesService = new Mock<IServiceJourneyRulesService>();
            _systemUnderTest = new ServiceJourneyRulesController(_mockLogger.Object, _mockServiceJourneyRulesService.Object);
        }

        [TestMethod]
        public void CorrectResponseIsReturnedWhenThereIsNoProvider()
        {
            _mockServiceJourneyRulesService.Setup(x => x.GetServiceJourneyRulesForOdsCode(TestOdsCode))
                .Returns(new ServiceJourneyRulesResponse()
                {
                    Appointments = new Appointments
                    {
                        Provider = AppointmentsProvider.none
                    }
                });

            var getResponse = _systemUnderTest.Get(TestOdsCode);
            Assert.AreEqual(getResponse.Value.Appointments.Provider, AppointmentsProvider.none);
        }

        [TestMethod]
        public void CorrectResponseIsReturnedWhenProviderIsIm1()
        {
            _mockServiceJourneyRulesService.Setup(x => x.GetServiceJourneyRulesForOdsCode(TestOdsCode))
                .Returns(new ServiceJourneyRulesResponse()
                {
                    Appointments = new Appointments
                    {
                        Provider = AppointmentsProvider.im1
                    }
                });

            var getResponse = _systemUnderTest.Get(TestOdsCode);
            Assert.AreEqual(getResponse.Value.Appointments.Provider, AppointmentsProvider.im1);
        }
    }
}