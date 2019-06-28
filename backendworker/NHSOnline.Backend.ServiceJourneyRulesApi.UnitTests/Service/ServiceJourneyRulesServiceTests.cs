using System.Collections.Generic;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.ServiceJourneyRulesApi.Models;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils;
using NHSOnline.Backend.ServiceJourneyRulesApi.Service;
using ILoggerFactory = Microsoft.Extensions.Logging.ILoggerFactory;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.UnitTests.Service
{
    [TestClass]
    public class ServiceJourneyRulesServiceTests
    {
        private IFixture _fixture;
        
        private ServiceJourneyRulesService _systemUnderTest;
        private Journeys _expectedJourneys;

        private const string TestOdsCode = "A29928";
        
        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            var loggerFactory = _fixture.Freeze<Mock<ILoggerFactory>>();

            _expectedJourneys = _fixture.Create<Journeys>();

            var journeyRepository = new JourneyRepository(new Dictionary<string, Journeys>
            {
                {TestOdsCode, _expectedJourneys }
            });
            _systemUnderTest = new ServiceJourneyRulesService(loggerFactory.Object, journeyRepository);
        }
        
        [TestMethod]
        public void GetServiceJourneyRulesForOdsCode_Success()
        {
            var getResponse = _systemUnderTest.GetServiceJourneyRulesForOdsCode(TestOdsCode);
            getResponse.Journeys.Should().BeEquivalentTo(_expectedJourneys);
        }

        [TestMethod]
        public void GetServiceJourneyRulesForOdsCode_NoJourneys()
        {
            var getResponse = _systemUnderTest.GetServiceJourneyRulesForOdsCode("123");
            getResponse.Journeys.Should().BeNull();
        }
    }
}