using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Models;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils.Steps;
using UnitTestHelper;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.UnitTests.RuleConfiguration.Utils.Steps
{
    [TestClass]
    public class ValidateOdsJourneysTests
    {
        private IValidatorStep _step;
        private Mock<ILogger<ValidateOdsJourneys>> _mockLogger;

        [TestInitialize]
        public void TestInitialize()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            _mockLogger = fixture.Freeze<Mock<ILogger<ValidateOdsJourneys>>>();

            _step = fixture.Create<ValidateOdsJourneys>();
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
                    ((ArgumentNullException) x).ParamName.Equals("MergedOdsJourneys", StringComparison.Ordinal));
            ;
        }

        [TestMethod]
        public void Execute_WhenMergedOdsJourneysIsNotPresent_ThrowsAnException()
        {
            // arrange
            var context = new ConfigurationContext();

            // act
            Func<Task> act = async () => await _step.Execute(context);

            // assert
            act.Should().Throw<ArgumentException>().And.ParamName.Should().Be("MergedOdsJourneys");
        }


        [TestMethod]
        public async Task Execute_WhenMissingJourneys_ReturnsFalse()
        {
            // arrange
            var context = new ConfigurationContext
            {
                MergedOdsJourneys = new Dictionary<string, Journeys>
                {
                    {
                        "A1",
                        CreateJourneys(AppointmentsJourneyType.Im1Appointments,
                            PrescriptionsJourneyType.Im1Prescriptions,
                            MedicalRecordJourneyType.Disabled)
                    },
                    {
                        "A2",
                        CreateJourneys(AppointmentsJourneyType.Disabled,
                            PrescriptionsJourneyType.None,
                            MedicalRecordJourneyType.Im1MedicalRecord)
                    },
                    {
                        "A3",
                        CreateJourneys(null,
                            PrescriptionsJourneyType.Im1Prescriptions,
                            MedicalRecordJourneyType.Im1MedicalRecord)
                    }
                }
            };

            // act
            var result = await _step.Execute(context);

            // assert;
            _mockLogger.VerifyLogger(LogLevel.Error, "Not all journey types have been defined for 'A2'.", Times.Once());
            _mockLogger.VerifyLogger(LogLevel.Error, "Not all journey types have been defined for 'A3'.", Times.Once());
            _mockLogger.VerifyLogger(LogLevel.Critical, "Error validating merged journeys.", Times.Once());

            result.Should().BeFalse();
        }
        
        [TestMethod]
        public async Task Execute_WhenAllJourneysAreProvided_ReturnsTrue()
        {
            // arrange
            var context = new ConfigurationContext
            {
                MergedOdsJourneys = new Dictionary<string, Journeys>
                {
                    {
                        "A1",
                        CreateJourneys(AppointmentsJourneyType.Im1Appointments,
                            PrescriptionsJourneyType.Im1Prescriptions,
                            MedicalRecordJourneyType.Disabled)
                    },
                    {
                        "A2",
                        CreateJourneys(AppointmentsJourneyType.Disabled,
                            PrescriptionsJourneyType.Disabled,
                            MedicalRecordJourneyType.Im1MedicalRecord)
                    },
                    {
                        "A3",
                        CreateJourneys(AppointmentsJourneyType.Im1Appointments,
                            PrescriptionsJourneyType.Im1Prescriptions,
                            MedicalRecordJourneyType.Im1MedicalRecord)
                    }
                }
            };

            // act
            var result = await _step.Execute(context);

            // assert;
            result.Should().BeTrue();
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