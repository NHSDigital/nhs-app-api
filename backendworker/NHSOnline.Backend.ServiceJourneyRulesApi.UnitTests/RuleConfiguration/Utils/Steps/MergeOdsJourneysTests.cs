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
                    .CoronavirusInformationEnabled(false)
                    .DocumentsEnabled(false)
                    .Im1MessagingEnabled(false, false, false, false,
                        false)
                    .MedicalRecord(MedicalRecordProvider.im1, "1")
                    .MessagingEnabled(false)
                    .NdopEnabled(false)
                    .NominatedPharmacyEnabled(true)
                    .NotificationsEnabled(true)
                    .NotificationPromptEnabled(false)
                    .OneOneOneEnabled(false)
                    .Prescriptions(PrescriptionsProvider.gpAtHand)
                    .SilverIntegrations(x => x
                        .AccountAdmin(AccountAdminProvider.gncr)
                        .ConsultationsAdmin(ConsultationsAdminProvider.engage)
                        .Participation(ParticipationProvider.substraktPatientPack)
                        .SecondaryAppointments(SecondaryAppointmentsProvider.ers)
                        .VaccineRecord(VaccineRecordProvider.nhsd)
                    )
                    .UserInfoEnabled(false)
                    .Build(),
                new JourneysBuilder()
                    .CdssAdminProvider(CdssProvider.none)
                    .HomeScreen(x => x.PublicHealthNotifications(CreatePublicHealthNotification("1")))
                    .MedicalRecord(MedicalRecordProvider.im1, "1")
                    .SilverIntegrations(x => x
                        .SecondaryAppointments(SecondaryAppointmentsProvider.ers)
                        .TestResults(TestResultsProvider.pkb))
                    .Build(),
                new JourneysBuilder()
                    .AppointmentProvider(AppointmentsProvider.informatica, "www.example.com")
                    .CdssAdviceProvider(CdssProvider.none)
                    .CoronavirusInformationEnabled(true)
                    .DocumentsEnabled(false)
                    .Im1MessagingEnabled(false, false, false, false,
                        false)
                    .MedicalRecord(MedicalRecordProvider.im1, "1")
                    .MessagingEnabled(false)
                    .NdopEnabled(true)
                    .NominatedPharmacyEnabled(false)
                    .NotificationsEnabled(false)
                    .NotificationPromptEnabled(true)
                    .OneOneOneEnabled(true)
                    .Prescriptions(PrescriptionsProvider.gpAtHand)
                    .UserInfoEnabled(false)
                    .Build(),
                new JourneysBuilder().Build()
            );

            var anotherFolderJourneys = CreateOdsJourneys(
                new JourneysBuilder()
                    .CdssAdminProvider(CdssProvider.eConsult, "adminDefinition")
                    .MedicalRecord(MedicalRecordProvider.im1, "2")
                    .Prescriptions(PrescriptionsProvider.gpAtHand)
                    .SilverIntegrations(x => x
                        .TestResults(TestResultsProvider.pkb))
                    .Build(),
                new JourneysBuilder()
                    .AppointmentProvider(AppointmentsProvider.gpAtHand)
                    .CoronavirusInformationEnabled(true)
                    .DocumentsEnabled(true)
                    .Im1MessagingEnabled(true, true, true,
                        true, true)
                    .MedicalRecord(MedicalRecordProvider.im1, "1")
                    .MessagingEnabled(true)
                    .NdopEnabled(true)
                    .NominatedPharmacyEnabled(true)
                    .NotificationsEnabled(true)
                    .NotificationPromptEnabled(true)
                    .OneOneOneEnabled(true)
                    .Prescriptions(PrescriptionsProvider.gpAtHand)
                    .SilverIntegrations(x => x
                        .CarePlans(CarePlansProvider.pkb)
                        .Consultations(ConsultationsProvider.pkb)
                        .ConsultationsAdmin(ConsultationsAdminProvider.engage)
                        .HealthTrackers(HealthTrackersProvider.pkb)
                        .Libraries(LibrariesProvider.pkb)
                        .Medicines(MedicinesProvider.pkb)
                        .Messages(MessagesProvider.pkb)
                        .Participation(ParticipationProvider.substraktPatientPack)
                        .RecordSharing(RecordSharingProvider.pkb)
                        .SecondaryAppointments(SecondaryAppointmentsProvider.pkb)
                        .VaccineRecord(VaccineRecordProvider.nhsd)
                    )
                    .UserInfoEnabled(true)
                    .Build(),
                new JourneysBuilder()
                    .AppointmentProvider(AppointmentsProvider.im1)
                    .CdssAdviceProvider(CdssProvider.eConsult, "adviceDefinition")
                    .CoronavirusInformationEnabled(false)
                    .DocumentsEnabled(true)
                    .HomeScreen(x => x.PublicHealthNotifications(CreatePublicHealthNotification("2")))
                    .Im1MessagingEnabled(true, false, false, false,
                        false)
                    .MedicalRecord(MedicalRecordProvider.gpAtHand, "1")
                    .MessagingEnabled(true)
                    .NdopEnabled(false)
                    .NotificationPromptEnabled(false)
                    .OneOneOneEnabled(false)
                    .Prescriptions(PrescriptionsProvider.im1)
                    .SilverIntegrations(x => x.SecondaryAppointments(SecondaryAppointmentsProvider.ers))
                    .UserInfoEnabled(true)
                    .Build(),
                new JourneysBuilder().Build()
            );

            var expectedMergedJourneys = CreateOdsJourneys(
                new JourneysBuilder()
                    .AppointmentProvider(AppointmentsProvider.informatica, "www.example.com")
                    .CdssAdminProvider(CdssProvider.eConsult, "adminDefinition")
                    .CdssAdviceProvider(CdssProvider.eConsult, "adviceDefinition")
                    .CoronavirusInformationEnabled(false)
                    .DocumentsEnabled(false)
                    .Im1MessagingEnabled(false, false, false, false,
                        false)
                    .MedicalRecord(MedicalRecordProvider.im1, "2")
                    .MessagingEnabled(false)
                    .NdopEnabled(false)
                    .NominatedPharmacyEnabled(true)
                    .NotificationsEnabled(true)
                    .NotificationPromptEnabled(false)
                    .OneOneOneEnabled(false)
                    .Prescriptions(PrescriptionsProvider.gpAtHand)
                    .SilverIntegrations(x => x
                        .AccountAdmin(AccountAdminProvider.gncr)
                        .ConsultationsAdmin(ConsultationsAdminProvider.engage)
                        .Participation(ParticipationProvider.substraktPatientPack)
                        .SecondaryAppointments(SecondaryAppointmentsProvider.ers)
                        .TestResults(TestResultsProvider.pkb)
                        .VaccineRecord(VaccineRecordProvider.nhsd)
                    )
                    .UserInfoEnabled(false)
                    .Build(),
                new JourneysBuilder()
                    .AppointmentProvider(AppointmentsProvider.gpAtHand)
                    .CdssAdminProvider(CdssProvider.none)
                    .CoronavirusInformationEnabled(true)
                    .DocumentsEnabled(true)
                    .HomeScreen(x => x.PublicHealthNotifications(new[] { CreatePublicHealthNotification("1") }))
                    .Im1MessagingEnabled(true, true, true,
                        true, true)
                    .MedicalRecord(MedicalRecordProvider.im1)
                    .MessagingEnabled(true)
                    .NdopEnabled(true)
                    .NominatedPharmacyEnabled(true)
                    .NotificationsEnabled(true)
                    .NotificationPromptEnabled(true)
                    .OneOneOneEnabled(true)
                    .Prescriptions(PrescriptionsProvider.gpAtHand)
                    .SilverIntegrations(x => x
                        .CarePlans(CarePlansProvider.pkb)
                        .Consultations(ConsultationsProvider.pkb)
                        .ConsultationsAdmin(ConsultationsAdminProvider.engage)
                        .HealthTrackers(HealthTrackersProvider.pkb)
                        .Libraries(LibrariesProvider.pkb)
                        .Medicines(MedicinesProvider.pkb)
                        .Messages(MessagesProvider.pkb)
                        .Participation(ParticipationProvider.substraktPatientPack)
                        .RecordSharing(RecordSharingProvider.pkb)
                        .SecondaryAppointments(SecondaryAppointmentsProvider.ers, SecondaryAppointmentsProvider.pkb)
                        .TestResults(TestResultsProvider.pkb)
                        .VaccineRecord(VaccineRecordProvider.nhsd)
                    )
                    .UserInfoEnabled(true)
                    .Build(),
                new JourneysBuilder()
                    .AppointmentProvider(AppointmentsProvider.im1)
                    .CdssAdviceProvider(CdssProvider.eConsult, "adviceDefinition")
                    .CoronavirusInformationEnabled(false)
                    .DocumentsEnabled(true)
                    .HomeScreen(x => x.PublicHealthNotifications(CreatePublicHealthNotification("2")))
                    .Im1MessagingEnabled(true, false, false,
                        false, false)
                    .MedicalRecord(MedicalRecordProvider.gpAtHand)
                    .MessagingEnabled(true)
                    .NdopEnabled(false)
                    .NominatedPharmacyEnabled(false)
                    .NotificationsEnabled(false)
                    .NotificationPromptEnabled(false)
                    .OneOneOneEnabled(false)
                    .Prescriptions(PrescriptionsProvider.im1)
                    .SilverIntegrations(x => x
                        .SecondaryAppointments(SecondaryAppointmentsProvider.ers))
                    .UserInfoEnabled(true)
                    .Build(),
                new JourneysBuilder()
                    .AppointmentProvider(null)
                    .CoronavirusInformationEnabled(null)
                    .CdssAdviceProvider(null)
                    .DocumentsEnabled(null)
                    .MedicalRecord(null)
                    .MessagingEnabled(null)
                    .NdopEnabled(null)
                    .NominatedPharmacyEnabled(null)
                    .NotificationsEnabled(null)
                    .NotificationPromptEnabled(null)
                    .OneOneOneEnabled(null)
                    .Prescriptions(null)
                    .UserInfoEnabled(null)
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

        private static PublicHealthNotification CreatePublicHealthNotification(string id) =>
            new PublicHealthNotification
            {
                Id = $"HealthNotification{id}",
                Type = NotificationType.callout,
                Urgency = NotificationUrgency.warning,
                Title = $"Health Notification {id} Title",
                Body = $"Health Notification {id} Body"
            };
    }
}
