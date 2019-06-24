using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.ServiceJourneyRulesApi.Models;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Models;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils.Steps;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.UnitTests.RuleConfiguration.Utils.Steps
{
    [TestClass]
    public class MergeOdsJourneysTests
    {
        private IValidatorStep _step;
        private Mock<ILogger<MergeOdsJourneys>> _mockLogger;

        [TestInitialize]
        public void TestInitialize()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
            
            _mockLogger = fixture.Freeze<Mock<ILogger<MergeOdsJourneys>>>();

            _step = fixture.Create<MergeOdsJourneys>();
        }

        [TestMethod]
        public void Execute_WhenContextIsNotPresent_ThrowsAnException()
        {
            // Act
            Func<Task> act = async () => await _step.Execute(null);

            // Assert
            act.Should().Throw<AggregateException>()
                .And.InnerExceptions.Should().HaveCount(2)
                .And.AllBeOfType<ArgumentNullException>()
                .And.Contain(x => ((ArgumentNullException) x).ParamName.Equals("context", StringComparison.Ordinal))
                .And.Contain(x =>
                    ((ArgumentNullException) x).ParamName.Equals("FolderOdsJourneys", StringComparison.Ordinal));
        }

        [TestMethod]
        public void Execute_WhenFolderOdsJourneysIsNotPresent_ThrowsAnException()
        {
            // Arrange
            var context = new ConfigurationContext();

            // Act
            Func<Task> act = async () => await _step.Execute(context);

            // Assert
            act.Should().Throw<ArgumentException>().And.ParamName.Should().Be("FolderOdsJourneys");
        }

        [TestMethod]
        public async Task Execute_WhenGivenAListOfFolderOdsJourneys_MergesJourneys()
        {
            // Arrange
            var defaultFolderJourneys = CreateOdsJourneys(
                CreateJourneys(null, PrescriptionsProvider.im1,
                    MedicalRecordProvider.Unknown),
                CreateJourneys(AppointmentsProvider.Unknown, PrescriptionsProvider.Unknown,
                    MedicalRecordProvider.im1),
                CreateJourneys(AppointmentsProvider.im1, PrescriptionsProvider.Unknown,
                    MedicalRecordProvider.none)
            );
            
            var anotherFolderJourneys = CreateOdsJourneys(
                CreateJourneys(null, PrescriptionsProvider.im1,
                    MedicalRecordProvider.none),
                CreateJourneys(AppointmentsProvider.none, PrescriptionsProvider.Unknown,
                    MedicalRecordProvider.im1),
                CreateJourneys(AppointmentsProvider.im1, PrescriptionsProvider.none,
                    MedicalRecordProvider.Unknown)
            );
            
            var expectedMergedJourneys = CreateOdsJourneys(
                CreateJourneys(null, PrescriptionsProvider.im1,
                    MedicalRecordProvider.none),
                CreateJourneys(AppointmentsProvider.none, PrescriptionsProvider.Unknown,
                    MedicalRecordProvider.im1),
                CreateJourneys(AppointmentsProvider.im1, PrescriptionsProvider.none,
                    MedicalRecordProvider.none)
            );

            var context = new ConfigurationContext
            {
                FolderOdsJourneys = new Dictionary<string, IDictionary<string, Journeys>>
                {
                    { "c:/default", defaultFolderJourneys },
                    { "c:/another", anotherFolderJourneys }
                }
            };

            // Act
            var result = await _step.Execute(context);

            // Assert
            result.Should().BeTrue();
            context.MergedOdsJourneys.Should().BeEquivalentTo(expectedMergedJourneys);
        }

        private static Dictionary<string, Journeys> CreateOdsJourneys(
            Journeys firstJourney,
            Journeys secondJourney,
            Journeys thirdJourney)
        {
            return new Dictionary<string, Journeys>
            {
                { "A1", firstJourney },
                { "A2", secondJourney },
                { "A3", thirdJourney }
            };
        }

        private static Journeys CreateJourneys(
            AppointmentsProvider? appointmentsProvider,
            PrescriptionsProvider? prescriptionsProvider,
            MedicalRecordProvider? medicalRecordProvider)
        {
            return new Journeys
            {
                Appointments = appointmentsProvider.HasValue
                    ? new Appointments { Provider = appointmentsProvider.Value }
                    : null,
                Prescriptions = prescriptionsProvider.HasValue
                    ? new Prescriptions { Provider = prescriptionsProvider.Value }
                    : null,
                MedicalRecord = medicalRecordProvider.HasValue
                    ? new MedicalRecord { Provider = medicalRecordProvider.Value }
                    : null
            };
        }
    }
}