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
            // act
            Func<Task> act = async () => await _step.Execute(null);

            // assert
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
            // arrange
            var context = new ConfigurationContext();

            // act
            Func<Task> act = async () => await _step.Execute(context);

            // assert
            act.Should().Throw<ArgumentException>().And.ParamName.Should().Be("FolderOdsJourneys");
        }

        [TestMethod]
        public async Task Execute_WhenGivenAListOfFolderOdsJourneys_MergesJourneys()
        {
            // arrange
            var defaultFolderJourneys = CreateOdsJourneys(
                CreateJourneys(null, PrescriptionsJourneyType.im1Prescriptions,
                    MedicalRecordJourneyType.None),
                CreateJourneys(AppointmentsJourneyType.None, PrescriptionsJourneyType.None,
                    MedicalRecordJourneyType.im1MedicalRecord),
                CreateJourneys(AppointmentsJourneyType.im1Appointments, PrescriptionsJourneyType.None,
                    MedicalRecordJourneyType.disabled)
            );
            
            var anotherFolderJourneys = CreateOdsJourneys(
                CreateJourneys(null, PrescriptionsJourneyType.im1Prescriptions,
                    MedicalRecordJourneyType.disabled),
                CreateJourneys(AppointmentsJourneyType.disabled, PrescriptionsJourneyType.None,
                    MedicalRecordJourneyType.im1MedicalRecord),
                CreateJourneys(AppointmentsJourneyType.im1Appointments, PrescriptionsJourneyType.disabled,
                    MedicalRecordJourneyType.None)
            );
            
            var expectedMergedJourneys = CreateOdsJourneys(
                CreateJourneys(null, PrescriptionsJourneyType.im1Prescriptions,
                    MedicalRecordJourneyType.disabled),
                CreateJourneys(AppointmentsJourneyType.disabled, PrescriptionsJourneyType.None,
                    MedicalRecordJourneyType.im1MedicalRecord),
                CreateJourneys(AppointmentsJourneyType.im1Appointments, PrescriptionsJourneyType.disabled,
                    MedicalRecordJourneyType.disabled)
            );

            var context = new ConfigurationContext
            {
                FolderOdsJourneys = new Dictionary<string, IDictionary<string, Journeys>>
                {
                    { "c:/default", defaultFolderJourneys },
                    { "c:/another", anotherFolderJourneys }
                }
            };

            // act
            var result = await _step.Execute(context);

            // assert
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
            AppointmentsJourneyType? appointmentsJourneyType,
            PrescriptionsJourneyType? prescriptionsJourneyType,
            MedicalRecordJourneyType? medicalRecordJourneyType)
        {
            return new Journeys
            {
                Appointments = appointmentsJourneyType.HasValue
                    ? new Appointments { JourneyType = appointmentsJourneyType.Value }
                    : null,
                Prescriptions = prescriptionsJourneyType.HasValue
                    ? new Prescriptions { JourneyType = prescriptionsJourneyType.Value }
                    : null,
                MedicalRecord = medicalRecordJourneyType.HasValue
                    ? new MedicalRecord { JourneyType = medicalRecordJourneyType.Value }
                    : null
            };
        }
    }
}