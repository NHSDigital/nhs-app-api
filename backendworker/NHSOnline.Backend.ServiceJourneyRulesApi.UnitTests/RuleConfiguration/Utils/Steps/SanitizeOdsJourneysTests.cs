using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.ServiceJourneyRulesApi.Models;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Models;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils.Steps;
using NHSOnline.Backend.Support.Sanitization;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.UnitTests.RuleConfiguration.Utils.Steps
{
    [TestClass]
    public class SanitizeOdsJourneysTests
    {
        private IFixture _fixture;
        private SanitizeOdsJourneys _sanitizeOdsJourneys;
        
        private Mock<IHtmlSanitizer> _mockHtmlSanitizer;
        private Mock<ILogger<SanitizeOdsJourneys>> _mockLogger;

        private const int PublicHealthNotificationCount = 3;
        private const string ExpectedSanitizedText = "Test text (sanitized)";
        private readonly List<string> _testJourneys = new List<string> { "A12345", "B67890" };

        private LoadContext _loadContext;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _mockHtmlSanitizer = _fixture.Create<Mock<IHtmlSanitizer>>();
            _mockLogger = _fixture.Create<Mock<ILogger<SanitizeOdsJourneys>>>();
            
            _sanitizeOdsJourneys = new SanitizeOdsJourneys(
                _mockHtmlSanitizer.Object,
                _mockLogger.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public async Task Execute_MergedOdsJourneysIsNull_ThrowsError()
        {
            await _sanitizeOdsJourneys.Execute(CreateLoadContext(true));

            _mockLogger.Verify(l => l
                .LogError(It.Is<string>(s =>
                    "The value for 'MergedOdsJourneys' has not been supplied".Equals(s, StringComparison.Ordinal))));
        }

        [TestMethod]
        public async Task Execute_MergedOdsJourneysHomeScreenNull_ReturnsTrue()
        {
            var result = await _sanitizeOdsJourneys.Execute(CreateLoadContext(homeScreenNull: true));
            
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task Execute_MergedOdsJourneysPublicHealthNotificationsNull_ReturnsTrue()
        {
            var result = await _sanitizeOdsJourneys.Execute(CreateLoadContext(publicHealthNotificationsNull: true));
            
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task Execute_MergedOdsJourneysHasPublicHealthNotifications_SanitizesEachAndReturnsTrue()
        {
            _loadContext = CreateLoadContext();
            MockSanitizeHtml();
            
            var result = await _sanitizeOdsJourneys.Execute(_loadContext);
            
            Assert.IsTrue(result);
            AssertPublicHealthNotificationsSanitized();
            _mockHtmlSanitizer.Verify();
        }

        private LoadContext CreateLoadContext(
            bool mergedOdsJourneysNull = false,
            bool homeScreenNull = false,
            bool publicHealthNotificationsNull = false)
        {
            return new LoadContext
            {
                MergedOdsJourneys = mergedOdsJourneysNull
                    ? null
                    : _testJourneys.ToDictionary(t => t, t => new Journeys
                    {
                        HomeScreen = homeScreenNull
                            ? null
                            : new HomeScreen
                            {
                                PublicHealthNotifications = publicHealthNotificationsNull
                                    ? null
                                    : _fixture
                                        .CreateMany<PublicHealthNotification>(PublicHealthNotificationCount)
                                        .ToList()
                            }
                    })
            };
        }

        private void MockSanitizeHtml()
        {
            foreach (var (_, journeys) in _loadContext.MergedOdsJourneys)
            {
                foreach (var notification in journeys.HomeScreen.PublicHealthNotifications)
                {
                    _mockHtmlSanitizer.Setup(h => h
                            .SanitizeHtml(
                                It.Is<string>(s => s.Equals(notification.Body, StringComparison.Ordinal))))
                        .Returns(ExpectedSanitizedText)
                        .Verifiable();
                }
            }
        }

        private void AssertPublicHealthNotificationsSanitized()
        {
            foreach (var (_, journeys) in _loadContext.MergedOdsJourneys)
            {
                foreach (var notification in journeys.HomeScreen.PublicHealthNotifications)
                {
                    Assert.AreEqual(ExpectedSanitizedText, notification.Body);
                }
            }
        }
    }
}