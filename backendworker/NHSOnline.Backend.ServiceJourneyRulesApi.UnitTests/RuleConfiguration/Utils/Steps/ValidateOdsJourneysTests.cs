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
            var context = new ConfigurationContext
            {
                MergedOdsJourneys = new Dictionary<string, Journeys>
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
                            .MedicalRecord(null)
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
                    }
                }
            };

            // Act
            var result = await _step.Execute(context);

            // Assert;
            _mockLogger.VerifyLogger(LogLevel.Error, "Not all journey types have been defined for 'A2'.", Times.Once());
            _mockLogger.VerifyLogger(LogLevel.Error, "Not all journey types have been defined for 'A3'.", Times.Once());
            _mockLogger.VerifyLogger(LogLevel.Error, "Not all journey types have been defined for 'A4'.", Times.Once());
            _mockLogger.VerifyLogger(LogLevel.Error, "Not all journey types have been defined for 'A5'.", Times.Once());
            _mockLogger.VerifyLogger(LogLevel.Error, "Not all journey types have been defined for 'A6'.", Times.Once());
            _mockLogger.VerifyLogger(LogLevel.Error, "Not all journey types have been defined for 'A7'.", Times.Once());
            _mockLogger.VerifyLogger(LogLevel.Critical, "Error validating merged journeys.", Times.Once());

            result.Should().BeFalse();
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
                            .MedicalRecord(MedicalRecordProvider.gpAtHand)
                            .Prescriptions(PrescriptionsProvider.gpAtHand)
                            .NominatedPharmacyEnabled(true)
                            .NotificationsEnabled(true)
                            .Build()
                    },
                    {
                        "A2",
                        new JourneysBuilder()
                            .AppointmentProvider(AppointmentsProvider.im1)
                            .CdssAdviceProvider(CdssProvider.eConsult, "adviceDefinition")
                            .CdssAdminProvider(CdssProvider.eConsult, "adminDefinition")
                            .MedicalRecord(MedicalRecordProvider.gpAtHand)
                            .Prescriptions(PrescriptionsProvider.gpAtHand)
                            .NominatedPharmacyEnabled(true)
                            .NotificationsEnabled(true)
                            .Build()
                    },
                    {
                        "A3",
                        new JourneysBuilder()
                            .AppointmentProvider(AppointmentsProvider.informatica, "www.example.com")
                            .CdssAdviceProvider(CdssProvider.none)
                            .CdssAdminProvider(CdssProvider.eConsult, "adminDefinition")
                            .MedicalRecord(MedicalRecordProvider.gpAtHand)
                            .Prescriptions(PrescriptionsProvider.gpAtHand)
                            .NominatedPharmacyEnabled(true)
                            .NotificationsEnabled(true)
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
                            .MedicalRecord(MedicalRecordProvider.gpAtHand)
                            .Prescriptions(PrescriptionsProvider.gpAtHand)
                            .NominatedPharmacyEnabled(true)
                            .NotificationsEnabled(true)
                            .Build()
                    },
                    {
                        "A2",
                        new JourneysBuilder()
                            .AppointmentProvider(AppointmentsProvider.im1)
                            .CdssAdviceProvider(CdssProvider.eConsult, "adviceDefinition")
                            .CdssAdminProvider(CdssProvider.eConsult, "adminDefinition")
                            .MedicalRecord(MedicalRecordProvider.gpAtHand)
                            .Prescriptions(PrescriptionsProvider.gpAtHand)
                            .NominatedPharmacyEnabled(false)
                            .NotificationsEnabled(false)
                            .Build()
                    },
                    {
                        "A3",
                        new JourneysBuilder()
                            .AppointmentProvider(AppointmentsProvider.informatica, "www.example.com")
                            .CdssAdviceProvider(CdssProvider.none)
                            .CdssAdminProvider(CdssProvider.eConsult, "adminDefinition")
                            .MedicalRecord(MedicalRecordProvider.gpAtHand)
                            .Prescriptions(PrescriptionsProvider.gpAtHand)
                            .NominatedPharmacyEnabled(true)
                            .NotificationsEnabled(true)
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
            var context = new LoadContext
            {
                MergedOdsJourneys = new Dictionary<string, Journeys>
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
                            .MedicalRecord(null)
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
                    }
                }
            };

            // Act
            var result = await _loadStep.Execute(context);

            // Assert;
            _mockLogger.VerifyLogger(LogLevel.Error, "Not all journey types have been defined for 'A2'.", Times.Once());
            _mockLogger.VerifyLogger(LogLevel.Error, "Not all journey types have been defined for 'A3'.", Times.Once());
            _mockLogger.VerifyLogger(LogLevel.Error, "Not all journey types have been defined for 'A4'.", Times.Once());
            _mockLogger.VerifyLogger(LogLevel.Error, "Not all journey types have been defined for 'A5'.", Times.Once());
            _mockLogger.VerifyLogger(LogLevel.Error, "Not all journey types have been defined for 'A6'.", Times.Once());
            _mockLogger.VerifyLogger(LogLevel.Error, "Not all journey types have been defined for 'A7'.", Times.Once());
            _mockLogger.VerifyLogger(LogLevel.Error, "Not all journey types have been defined for 'A8'.", Times.Once());
            _mockLogger.VerifyLogger(LogLevel.Critical, "Error validating merged journeys.", Times.Once());

            result.Should().BeFalse();
        }

        private JourneysBuilder ValidJourneys()
        {
            return new JourneysBuilder()
                .AppointmentProvider(AppointmentsProvider.im1)
                .CdssAdviceProvider(CdssProvider.none)
                .CdssAdminProvider(CdssProvider.eConsult, "adminDefinition")
                .MedicalRecord(MedicalRecordProvider.gpAtHand)
                .Prescriptions(PrescriptionsProvider.gpAtHand)
                .NominatedPharmacyEnabled(true)
                .NotificationsEnabled(true);
        }
    }
}