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
using NHSOnline.Backend.Support;
using UnitTestHelper;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.UnitTests.RuleConfiguration.Utils.Steps
{
    [TestClass]
    public class ValidateOdsJourneysTests
    {
        private IValidatorStep _step;
        private ILoadStep _loadStep;
        private Mock<ILogger<ValidateOdsJourneys>> _mockLogger;

        [TestInitialize]
        public void TestInitialize()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            _mockLogger = fixture.Freeze<Mock<ILogger<ValidateOdsJourneys>>>();

            _step = fixture.Create<ValidateOdsJourneys>();

            _loadStep = fixture.Create<ValidateOdsJourneys>();
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
                    ((ArgumentNullException) x).ParamName.Equals("MergedOdsJourneys", StringComparison.Ordinal));
        }

        [TestMethod]
        public void Execute_WhenMergedOdsJourneysIsNotPresent_ThrowsAnException()
        {
            // Arrange
            var context = new ConfigurationContext();

            // Act
            Func<Task> act = async () => await _step.Execute(context);

            // Assert
            act.Should().Throw<ArgumentException>().And.ParamName.Should().Be("MergedOdsJourneys");
        }

        [TestMethod]
        public async Task Execute_WhenMissingJourneys_ReturnsFalse()
        {
            // Arrange
            var context = new ConfigurationContext();
            await TestInvalidJourneys(context);
        }

        [TestMethod]
        public async Task Execute_WhenAllJourneysAreProvided_ReturnsTrue()
        {
            // Arrange
            var context = new ConfigurationContext
            {
                MergedOdsJourneys = new Dictionary<string, Journeys>
                {
                    {
                        "A1",
                        new JourneysBuilder()
                            .AppointmentProvider(AppointmentsProvider.im1)
                            .CdssAdviceProvider(CdssProvider.none)
                            .CdssAdminProvider(CdssProvider.eConsult, "adminDefinition")
                            .MedicalRecord(MedicalRecordProvider.gpAtHand, "1")
                            .Prescriptions(PrescriptionsProvider.gpAtHand)
                            .NominatedPharmacyEnabled(true)
                            .NotificationsEnabled(true)
                            .MessagingEnabled(true)
                            .UserInfoEnabled(true)
                            .SilverIntegrations(_ => _
                                .SecondaryAppointments( SecondaryAppointmentProvider.pkb )
                                .Messages( MessagesProvider.pkb )
                                .Consultations(ConsultationsProvider.pkb))
                            .HomeScreen(_ => _.PublicHealthNotifications(CreatePublicHealthNotification()))
                            .DocumentsEnabled(true)
                            .Im1MessagingEnabled(true, true)
                            .WithSupplier(Supplier.Emis)
                            .Build()
                    },
                    {
                        "A2",
                        new JourneysBuilder()
                            .AppointmentProvider(AppointmentsProvider.im1)
                            .CdssAdviceProvider(CdssProvider.eConsult, "adviceDefinition")
                            .CdssAdminProvider(CdssProvider.eConsult, "adminDefinition")
                            .MedicalRecord(MedicalRecordProvider.gpAtHand, "1")
                            .Prescriptions(PrescriptionsProvider.gpAtHand)
                            .NominatedPharmacyEnabled(false)
                            .NotificationsEnabled(false)
                            .MessagingEnabled(false)
                            .UserInfoEnabled(false)
                            .SilverIntegrations(_ => _
                                .SecondaryAppointments()
                                .Messages()
                                .Consultations())
                            .DocumentsEnabled(false)
                            .Im1MessagingEnabled(false, false)
                            .WithSupplier(Supplier.Tpp)
                            .Build()
                    },
                    {
                        "A3",
                        new JourneysBuilder()
                            .AppointmentProvider(AppointmentsProvider.informatica, "www.example.com")
                            .CdssAdviceProvider(CdssProvider.none)
                            .CdssAdminProvider(CdssProvider.eConsult, "adminDefinition")
                            .MedicalRecord(MedicalRecordProvider.gpAtHand, "1")
                            .Prescriptions(PrescriptionsProvider.gpAtHand)
                            .NominatedPharmacyEnabled(true)
                            .NotificationsEnabled(true)
                            .MessagingEnabled(true)
                            .UserInfoEnabled(true)
                            .SilverIntegrations(_ => _
                                .SecondaryAppointments()
                                .Messages()
                                .Consultations())
                            .DocumentsEnabled(true)
                            .Im1MessagingEnabled(true, false)
                            .WithSupplier(Supplier.Vision)
                            .Build()
                    }
                }
            };

            // Act
            var result = await _step.Execute(context);

            // Assert;
            result.Should().BeTrue();
        }

        [TestMethod]
        public async Task Execute_WhenMergedConfigFilesContainsAllJourneys_ReturnsTrue()
        {
            // Arrange
            var context = new LoadContext
            {
                MergedOdsJourneys = new Dictionary<string, Journeys>
                {
                    {
                        "A1",
                        new JourneysBuilder()
                            .AppointmentProvider(AppointmentsProvider.im1)
                            .CdssAdviceProvider(CdssProvider.none)
                            .CdssAdminProvider(CdssProvider.eConsult, "adminDefinition")
                            .MedicalRecord(MedicalRecordProvider.gpAtHand, "1")
                            .Prescriptions(PrescriptionsProvider.gpAtHand)
                            .NominatedPharmacyEnabled(true)
                            .NotificationsEnabled(true)
                            .MessagingEnabled(true)
                            .UserInfoEnabled(true)
                            .SilverIntegrations(_ => _.SecondaryAppointments()
                                .Messages()
                                .Consultations())
                            .DocumentsEnabled(true)
                            .Im1MessagingEnabled(true, false)
                            .WithSupplier(Supplier.Microtest)
                            .Build()
                    },
                    {
                        "A2",
                        new JourneysBuilder()
                            .AppointmentProvider(AppointmentsProvider.gpAtHand)
                            .CdssAdviceProvider(CdssProvider.eConsult, "adviceDefinition")
                            .CdssAdminProvider(CdssProvider.eConsult, "adminDefinition")
                            .MedicalRecord(MedicalRecordProvider.gpAtHand, "1")
                            .Prescriptions(PrescriptionsProvider.gpAtHand)
                            .NominatedPharmacyEnabled(false)
                            .NotificationsEnabled(false)
                            .MessagingEnabled(false)
                            .UserInfoEnabled(false)
                            .SilverIntegrations(_ => _
                                .SecondaryAppointments()
                                .Messages()
                                .Consultations())
                            .DocumentsEnabled(false)
                            .Im1MessagingEnabled(false, false)
                            .WithSupplier(Supplier.Emis)
                            .Build()
                    },
                    {
                        "A3",
                        new JourneysBuilder()
                            .AppointmentProvider(AppointmentsProvider.informatica, "www.example.com")
                            .CdssAdviceProvider(CdssProvider.none)
                            .CdssAdminProvider(CdssProvider.eConsult, "adminDefinition")
                            .MedicalRecord(MedicalRecordProvider.gpAtHand, "1")
                            .Prescriptions(PrescriptionsProvider.gpAtHand)
                            .NominatedPharmacyEnabled(true)
                            .NotificationsEnabled(true)
                            .MessagingEnabled(true)
                            .UserInfoEnabled(true)
                            .SilverIntegrations(_ => _
                                .SecondaryAppointments()
                                .Messages()
                                .Consultations())
                            .HomeScreen(_ => _.PublicHealthNotifications(CreatePublicHealthNotification()))
                            .DocumentsEnabled(true)
                            .Im1MessagingEnabled(true, true)
                            .WithSupplier(Supplier.Tpp)
                            .Build()
                    }
                }
            };

            // Act
            var result = await _loadStep.Execute(context);

            // Assert;
            result.Should().BeTrue();
        }

        [TestMethod]
        public async Task Execute_WhenMergedConfigFilesMissingJourneys_ReturnsFalse()
        {
            // Arrange
            var context = new LoadContext();
            await TestInvalidJourneys(context);
        }

        private async Task TestInvalidJourneys(LoadContext context)
        {

            // Arrange
            context.MergedOdsJourneys = new Dictionary<string, Journeys>
            {
                {
                    "A1",
                    ValidJourneys().Build()
                },
                {
                    "A2",
                    ValidJourneys()
                        .AppointmentProvider(null)
                        .Build()
                },
                {
                    "A3",
                    ValidJourneys()
                        .CdssAdviceProvider(null)
                        .Build()
                },
                {
                    "A4",
                    ValidJourneys()
                        .CdssAdminProvider(null)
                        .Build()
                },
                {
                    "A5",
                    ValidJourneys()
                        .MedicalRecord(null, "1")
                        .Build()
                },
                {
                    "A6",
                    ValidJourneys()
                        .Prescriptions(null)
                        .Build()
                },
                {
                    "A7",
                    ValidJourneys()
                        .NominatedPharmacyEnabled(null)
                        .Build()
                },
                {
                    "A8",
                    ValidJourneys()
                        .NotificationsEnabled(null)
                        .Build()
                },
                {
                    "A9",
                    ValidJourneys()
                        .MessagingEnabled(null)
                        .Build()
                },
                {
                    "A10",
                    ValidJourneys()
                        .UserInfoEnabled(null)
                        .Build()
                },
                {
                    "A11",
                    ValidJourneys()
                        .SilverIntegrations(_ => _.Consultations().Messages())
                        .Build()
                },
                {
                    "A12",
                    ValidJourneys()
                        .SilverIntegrations(_ => _.Consultations().SecondaryAppointments())
                        .Build()
                },
                {
                    "A13",
                    ValidJourneys()
                        .SilverIntegrations(_ => _.SecondaryAppointments().Messages())
                        .Build()
                },
                {
                    "A14",
                    ValidJourneys()
                        .HomeScreen(_ => _.PublicHealthNotifications())
                        .Build()
                },
                {
                    "A15",
                    ValidJourneys()
                        .WithSupplier(Supplier.Unknown)
                        .Build()
                }
            };

            // Act
            var result = await _loadStep.Execute(context);

            // Assert;
            _mockLogger.VerifyLogger(LogLevel.Debug, "Validation successful for 'A1'", Times.Once());
            AssertError("A2", "journeys.Appointments.Provider");
            AssertError("A3", "journeys.CdssAdvice.Provider");
            AssertError("A4", "journeys.CdssAdmin.Provider");
            AssertError("A5", "journeys.MedicalRecord.Provider");
            AssertError("A6", "journeys.Prescriptions.Provider");
            AssertError("A7", "journeys.NominatedPharmacy");
            AssertError("A8", "journeys.Notifications");
            AssertError("A9", "journeys.Messaging");
            AssertError("A10", "journeys.UserInfo");
            AssertError("A11", "journeys.SilverIntegrations.SecondaryAppointments");
            AssertError("A12", "journeys.SilverIntegrations.Messages");
            AssertError("A13", "journeys.SilverIntegrations.Consultations");
            AssertError("A14", "journeys.HomeScreen.PublicHealthNotifications");
            AssertError("A15", "journeys.Supplier");
            _mockLogger.VerifyLogger(LogLevel.Critical, "Error validating merged journeys.", Times.Once());

            result.Should().BeFalse();
        }

        private void AssertError(string odsCode, string value)
        {
            _mockLogger.VerifyLogger(LogLevel.Error,
                $"Not all journey types have been defined for '{odsCode}'. Missing Values:\n{value}", Times.Once());
        }

        private static JourneysBuilder ValidJourneys()
        {
            return new JourneysBuilder()
                .AppointmentProvider(AppointmentsProvider.im1)
                .CdssAdviceProvider(CdssProvider.none)
                .CdssAdminProvider(CdssProvider.eConsult, "adminDefinition")
                .MedicalRecord(MedicalRecordProvider.gpAtHand, "1")
                .Prescriptions(PrescriptionsProvider.gpAtHand)
                .NominatedPharmacyEnabled(true)
                .NotificationsEnabled(true)
                .MessagingEnabled(true)
                .UserInfoEnabled(true)
                .HomeScreen(_ => _.PublicHealthNotifications(CreatePublicHealthNotification()))
                .SilverIntegrations(_ => _
                    .SecondaryAppointments()
                    .Messages()
                    .Consultations())
                .DocumentsEnabled(true)
                .Im1MessagingEnabled(true, true)
                .WithSupplier(Supplier.Emis);
        }

        private static PublicHealthNotification CreatePublicHealthNotification() =>
            new PublicHealthNotification
            {
                Id = "HealthNotification1",
                Type = NotificationType.callout,
                Urgency = NotificationUrgency.warning,
                Title = "Health Notification 1 Title",
                Body = "Health Notification 1 Body"
            };
    }
}