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
            
            _loadStep= fixture.Create<ValidateOdsJourneys>();
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
            ;
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
                        CreateJourneys(AppointmentsProvider.im1,
                            PrescriptionsProvider.im1,
                            MedicalRecordProvider.none)
                    },
                    {
                        "A2",
                        CreateJourneys(AppointmentsProvider.none,
                            PrescriptionsProvider.Unknown,
                            MedicalRecordProvider.im1)
                    },
                    {
                        "A3",
                        CreateJourneys(null,
                            PrescriptionsProvider.im1,
                            MedicalRecordProvider.im1)
                    }
                }
            };

            // Act
            var result = await _step.Execute(context);

            // Assert;
            _mockLogger.VerifyLogger(LogLevel.Error, "Not all journey types have been defined for 'A2'.", Times.Once());
            _mockLogger.VerifyLogger(LogLevel.Error, "Not all journey types have been defined for 'A3'.", Times.Once());
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
                        CreateJourneys(AppointmentsProvider.im1,
                            PrescriptionsProvider.im1,
                            MedicalRecordProvider.none)
                    },
                    {
                        "A2",
                        CreateJourneys(AppointmentsProvider.none,
                            PrescriptionsProvider.none,
                            MedicalRecordProvider.im1)
                    },
                    {
                        "A3",
                        CreateJourneys(AppointmentsProvider.im1,
                            PrescriptionsProvider.im1,
                            MedicalRecordProvider.im1)
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
                        CreateJourneys(AppointmentsProvider.im1,
                            PrescriptionsProvider.im1,
                            MedicalRecordProvider.none)
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
                        CreateJourneys(null,
                            PrescriptionsProvider.im1,
                            MedicalRecordProvider.none)
                    }
                }
            };

            // Act
            var result = await _loadStep.Execute(context);

            // Assert;
            _mockLogger.VerifyLogger(LogLevel.Error, "Not all journey types have been defined for 'A1'.", Times.Once());
            _mockLogger.VerifyLogger(LogLevel.Critical, "Error validating merged journeys.", Times.Once());

            result.Should().BeFalse();
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