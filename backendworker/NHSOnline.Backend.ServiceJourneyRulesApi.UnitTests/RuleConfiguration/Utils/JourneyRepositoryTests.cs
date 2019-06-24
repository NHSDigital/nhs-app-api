using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Models;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.UnitTests.RuleConfiguration.Utils
{
    [TestClass]
    public class JourneyRepositoryTests
    {
        private IJourneyRepository _journeyRepository;

        [TestInitialize]
        public void TestInitialize()
        {
            var odsJourneys = new Dictionary<string, Journeys> { { "foo", new Journeys() } };

            _journeyRepository = new JourneyRepository(odsJourneys);
        }

        [TestMethod]
        public void GetJourneys_WhenCalledWithValidOdsCode_ReturnMatchingJourney()
        {
            // Act
            var result = _journeyRepository.GetJourneys("foo");
            
            // Assert
            result.Should().NotBeNull();
        }
    }
}