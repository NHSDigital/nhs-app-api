using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.ServiceJourneyRulesApi.Models;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.UnitTests.RuleConfiguration.Utils
{
    [TestClass]
    public class JourneyRepositoryTests
    {
        private const string TestOdsCode = "Foo";

        private IJourneyRepository _journeyRepository;

        [TestInitialize]
        public void TestInitialize()
        {
            var odsJourneys = new Dictionary<string, Journeys>
            {
                { TestOdsCode, new Journeys() }
            };

            _journeyRepository = new JourneyRepository(odsJourneys);
        }

        [TestMethod]
        public void GetJourneys_WhenCalledWithValidOdsCode_ReturnMatchingJourney()
        {
            // Act
            var result = _journeyRepository.GetJourneys(TestOdsCode);
            
            // Assert
            result.Should().NotBeNull();
        }

        [TestMethod]
        public void GetOdsCodes_WhenCalledWithNullJourneys_ReturnsEmptySet()
        {
            // Arrange
            _journeyRepository = new JourneyRepository(null);

            // Act
            var result = _journeyRepository.GetOdsCodes().OdsCodes.ToList();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [TestMethod]
        public void GetOdsCodes_WhenCalledWithEmptyJourneys_ReturnsEmptySet()
        {
            // Arrange
            _journeyRepository = new JourneyRepository(new Dictionary<string, Journeys>());

            // Act
            var result = _journeyRepository.GetOdsCodes().OdsCodes.ToList();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [TestMethod]
        public void GetOdsCodes_WhenCalledWithPopulatedJourneys_ReturnsCorrectOdsCodes()
        {
            // Act
            var result = _journeyRepository.GetOdsCodes().OdsCodes.ToList();

            // Assert
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
            result.Should().ContainSingle(x => x == TestOdsCode);
        }

        [TestMethod]
        public void GetOdsCodes_WhenCalledWithPopulatedJourneysWithNoOdsCodesPresent_ReturnsCorrectOdsCodesWithoutNoOdsCodes()
        {
            // Arrange
            _journeyRepository = new JourneyRepository(new Dictionary<string, Journeys>
            {
                { TestOdsCode, new Journeys() },
                { Constants.OdsCode.None, new Journeys() }
            });
            
            // Act
            var result = _journeyRepository.GetOdsCodes().OdsCodes.ToList();

            // Assert
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
            result.Should().ContainSingle(x => x == TestOdsCode);
        }
    }
}