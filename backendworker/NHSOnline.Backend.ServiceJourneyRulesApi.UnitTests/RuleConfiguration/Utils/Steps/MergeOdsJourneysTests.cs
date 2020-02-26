using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.ServiceJourneyRulesApi.Models;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Models;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils.Steps;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.UnitTests.RuleConfiguration.Utils.Steps
{
    [TestClass]
    public class MergeOdsJourneysTests
    {
        private IValidatorStep _step;

        [TestInitialize]
        public void TestInitialize()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

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
                new JourneysBuilder()
                    .AppointmentProvider(AppointmentsProvider.informatica, "www.example.com")
                    .CdssAdviceProvider(CdssProvider.eConsult, "adviceDefinition")
                    .MedicalRecord(MedicalRecordProvider.im1, 1)
                    .Prescriptions(PrescriptionsProvider.gpAtHand)
                    .NominatedPharmacyEnabled(true)
                    .NotificationsEnabled(true)
                    .MessagingEnabled(false)
                    .UserInfoEnabled(false)
                    .SilverIntegrations(_ => _.SecondaryAppointments(new[] { SecondaryAppointmentProvider.ers }))
                    .Build(),
                new JourneysBuilder()
                    .CdssAdminProvider(CdssProvider.none)
                    .MedicalRecord(MedicalRecordProvider.im1, 1)
                    .SilverIntegrations(_ => _.SecondaryAppointments(new[] { SecondaryAppointmentProvider.ers }))
                    .Build(),
                new JourneysBuilder()
                    .AppointmentProvider(AppointmentsProvider.informatica, "www.example.com")
                    .CdssAdviceProvider(CdssProvider.none)
                    .MedicalRecord(MedicalRecordProvider.im1, 1)
                    .Prescriptions(PrescriptionsProvider.gpAtHand)
                    .NominatedPharmacyEnabled(false)
                    .NotificationsEnabled(false)
                    .MessagingEnabled(false)
                    .UserInfoEnabled(false)
                    .SilverIntegrations(_ => _.SecondaryAppointments(new[] { SecondaryAppointmentProvider.pkb }))
                    .Build(),
                new JourneysBuilder().Build()
            );

            var anotherFolderJourneys = CreateOdsJourneys(
                new JourneysBuilder()
                    .CdssAdminProvider(CdssProvider.eConsult, "adminDefinition")
                    .MedicalRecord(MedicalRecordProvider.im1, 2)
                    .Prescriptions(PrescriptionsProvider.gpAtHand)
                    .Build(),
                new JourneysBuilder()
                    .AppointmentProvider(AppointmentsProvider.gpAtHand)
                    .MedicalRecord(MedicalRecordProvider.im1, 1)
                    .Prescriptions(PrescriptionsProvider.gpAtHand)
                    .NominatedPharmacyEnabled(true)
                    .NotificationsEnabled(true)
                    .MessagingEnabled(true)
                    .UserInfoEnabled(true)
                    .SilverIntegrations(_ => _
                        .SecondaryAppointments(SecondaryAppointmentProvider.pkb)
                        .Messages(MessagesProvider.pkb)
                        .Consultations(ConsultationsProvider.pkb))
                    .Build(),
                new JourneysBuilder()
                    .AppointmentProvider(AppointmentsProvider.im1)
                    .CdssAdviceProvider(CdssProvider.eConsult, "adviceDefinition")
                    .MedicalRecord(MedicalRecordProvider.gpAtHand, 1)
                    .Prescriptions(PrescriptionsProvider.im1)
                    .MessagingEnabled(true)
                    .UserInfoEnabled(true)
                    .SilverIntegrations(_ => _.SecondaryAppointments())
                    .Build(),
                new JourneysBuilder().Build()
            );

            var expectedMergedJourneys = CreateOdsJourneys(
                new JourneysBuilder()
                    .AppointmentProvider(AppointmentsProvider.informatica, "www.example.com")
                    .CdssAdviceProvider(CdssProvider.eConsult, "adviceDefinition")
                    .CdssAdminProvider(CdssProvider.eConsult, "adminDefinition")
                    .MedicalRecord(MedicalRecordProvider.im1, 2)
                    .Prescriptions(PrescriptionsProvider.gpAtHand)
                    .NominatedPharmacyEnabled(true)
                    .NotificationsEnabled(true)
                    .MessagingEnabled(false)
                    .UserInfoEnabled(false)
                    .SilverIntegrations(_ => _
                        .SecondaryAppointments(SecondaryAppointmentProvider.ers)
                        .Messages()
                        .Consultations())
                    .Build(),
                new JourneysBuilder()
                    .AppointmentProvider(AppointmentsProvider.gpAtHand)
                    .CdssAdminProvider(CdssProvider.none)
                    .MedicalRecord(MedicalRecordProvider.im1)
                    .Prescriptions(PrescriptionsProvider.gpAtHand)
                    .NominatedPharmacyEnabled(true)
                    .NotificationsEnabled(false)
                    .NotificationsEnabled(true)
                    .MessagingEnabled(true)
                    .UserInfoEnabled(true)
                    .SilverIntegrations(_ => _
                        .SecondaryAppointments(SecondaryAppointmentProvider.pkb)
                        .Messages(MessagesProvider.pkb)
                        .Consultations(ConsultationsProvider.pkb))
                    .Build(),
                new JourneysBuilder()
                    .AppointmentProvider(AppointmentsProvider.im1)
                    .CdssAdviceProvider(CdssProvider.eConsult, "adviceDefinition")
                    .MedicalRecord(MedicalRecordProvider.gpAtHand)
                    .Prescriptions(PrescriptionsProvider.im1)
                    .NominatedPharmacyEnabled(false)
                    .NotificationsEnabled(false)
                    .MessagingEnabled(true)
                    .UserInfoEnabled(true)
                    .SilverIntegrations(_ => _
                        .SecondaryAppointments()
                        .Messages()
                        .Consultations())
                    .Build(),
                new JourneysBuilder()
                    .AppointmentProvider(null)
                    .CdssAdviceProvider(null)
                    .MedicalRecord(null)
                    .Prescriptions(null)
                    .NominatedPharmacyEnabled(null)
                    .NotificationsEnabled(null)
                    .MessagingEnabled(null)
                    .UserInfoEnabled(null)
                    .SilverIntegrations(_ => _
                        .SecondaryAppointments()
                        .Messages()
                        .Consultations())
                    .Build()
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

        private static Dictionary<string, Journeys> CreateOdsJourneys(Journeys firstJourney,
            Journeys secondJourney,
            Journeys thirdJourney,
            Journeys fourthJourney
        )
        {
            return new Dictionary<string, Journeys>
            {
                { "A1", firstJourney },
                { "A2", secondJourney },
                { "A3", thirdJourney },
                { "A4", fourthJourney }
            };
        }
    }
}