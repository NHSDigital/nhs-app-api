using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.ServiceJourneyRulesApi.Models;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Models;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils.Steps;
using UnitTestHelper;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.UnitTests.RuleConfiguration.Utils.Steps
{
    [TestClass]
    public class OutputOdsJourneysTests
    {
        private IValidatorStep _step;
        private IFixture _fixture;
        private Mock<IYamlWriter> _mockYamlWriter;
        private Mock<ILogger<OutputOdsJourneys>> _mockLogger;
        private Mock<IServiceJourneyRulesConfiguration> _mockConfiguration;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            _mockYamlWriter = _fixture.Freeze<Mock<IYamlWriter>>();
            _mockLogger = _fixture.Freeze<Mock<ILogger<OutputOdsJourneys>>>();
            _mockConfiguration = _fixture.Freeze<Mock<IServiceJourneyRulesConfiguration>>();
            _mockConfiguration.SetupGet(s => s.OutputFolderPath).Returns("c:/output");

            _step = _fixture.Create<OutputOdsJourneys>();
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
        public async Task Execute_WhenWritingFileFails_ReturnsFalse()
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
                            .Build()
                    }
                }
            };

            var exception = new IOException();

            _mockYamlWriter.Setup(s => s.Write(It.IsAny<string>(), It.IsAny<TargetConfiguration>()))
                .Throws(exception);

            // Act
            var result = await _step.Execute(context);

            // Assert
            _mockYamlWriter.Verify(s => s.Write(It.IsAny<string>(), It.IsAny<TargetConfiguration>()));
            _mockLogger.VerifyLogger(LogLevel.Critical, "Error outputting the merged files", exception, Times.Once());

            result.Should().BeFalse();
        }

        [TestMethod]
        public async Task Execute_WhenGivenAListOfOdsJourneys_WritesToFileAndReturnsTrue()
        {
            // Arrange
            var writeCalls = new Dictionary<string, TargetConfiguration>();
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
                            .Build()
                    },
                    {
                        "A2",
                        new JourneysBuilder()
                            .AppointmentProvider(AppointmentsProvider.im1)
                            .CdssAdviceProvider(CdssProvider.eConsult, "adviceDefinition")
                            .MedicalRecord(MedicalRecordProvider.gpAtHand)
                            .Prescriptions(PrescriptionsProvider.gpAtHand)
                            .NominatedPharmacyEnabled(false)
                            .CdssAdminProvider(CdssProvider.none)
                            .Build()
                    },
                    {
                        "A3",
                        new JourneysBuilder()
                            .AppointmentProvider(AppointmentsProvider.informatica,"www.example.com" )
                            .CdssAdviceProvider(CdssProvider.none)
                            .CdssAdminProvider(CdssProvider.none)
                            .MedicalRecord(MedicalRecordProvider.im1)
                            .Prescriptions(PrescriptionsProvider.gpAtHand)
                            .NominatedPharmacyEnabled(true)
                            .Build()
                    }
                }
            };

            _mockYamlWriter.Setup(s => s.Write(It.IsAny<string>(), It.IsAny<TargetConfiguration>()))
                .Callback((string path, TargetConfiguration configuration) => writeCalls.Add(path, configuration));

            // Act
            var result = await _step.Execute(context);

            // Assert
            result.Should().BeTrue();
            writeCalls.Should().HaveCount(3);
            writeCalls.Should().ContainKey("c:/output/A1.yaml")
                .WhichValue.Should().BeEquivalentTo(new TargetConfiguration
                {
                    Target = new Target { OdsCode = "A1" },
                    Journeys = context.MergedOdsJourneys["A1"]
                });

            writeCalls.Should().ContainKey("c:/output/A2.yaml")
                .WhichValue.Should().BeEquivalentTo(new TargetConfiguration
                {
                    Target = new Target { OdsCode = "A2" },
                    Journeys = context.MergedOdsJourneys["A2"]
                });

            writeCalls.Should().ContainKey("c:/output/A3.yaml")
                .WhichValue.Should().BeEquivalentTo(new TargetConfiguration
                {
                    Target = new Target { OdsCode = "A3" },
                    Journeys = context.MergedOdsJourneys["A3"]
                });
        }
    }
}