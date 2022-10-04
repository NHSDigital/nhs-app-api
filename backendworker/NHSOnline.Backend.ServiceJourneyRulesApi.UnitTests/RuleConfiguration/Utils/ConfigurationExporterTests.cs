using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using NHSOnline.Backend.Support;
using UnitTestHelper;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.UnitTests.RuleConfiguration.Utils
{
    [TestClass]
    public class ConfigurationExporterTests
    {
        private const string OutputFolderPath = "c:/output";
        private const string CsvExportOutputFilePath = "c:/output/export.csv";

        private IConfigurationExporter _systemUnderTest;
        private IFixture _fixture;

        private Mock<IServiceJourneyRulesConfiguration> _mockConfiguration;
        private Mock<IYamlReaderFactory> _mockYamlReaderFactory;
        private Mock<IFileHandler> _mockFileHandler;
        private Mock<ILogger<ConfigurationExporter>> _mockLogger;
        private Mock<TextWriter> _mockTextWriter;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            _mockConfiguration = _fixture.Freeze<Mock<IServiceJourneyRulesConfiguration>>();
            _mockConfiguration.SetupGet(s => s.OutputFolderPath).Returns(OutputFolderPath);
            _mockConfiguration.SetupGet(x => x.CsvExportOutputFilePath).Returns(CsvExportOutputFilePath);

            _mockYamlReaderFactory = _fixture.Freeze<Mock<IYamlReaderFactory>>();

            _mockTextWriter = new Mock<TextWriter>();
            _mockFileHandler = _fixture.Freeze<Mock<IFileHandler>>();
            _mockFileHandler
                .Setup(x => x.GetTextWriter(CsvExportOutputFilePath))
                .Returns(_mockTextWriter.Object);

            _mockLogger = _fixture.Freeze<Mock<ILogger<ConfigurationExporter>>>();

            _systemUnderTest = _fixture.Create<ConfigurationExporter>();
        }

        [TestMethod]
        public async Task Export_WhenNoFilesPresent_LogsAndReturnsSuccess()
        {
            // Arrange
            _mockFileHandler
                .Setup(x => x.GetFiles(OutputFolderPath))
                .Returns(Array.Empty<string>());

            // Act
            var result = await _systemUnderTest.Export();

            // Assert
            result.Should().Be(0);
            _mockLogger.VerifyLogger(LogLevel.Information, $"No files to process at {OutputFolderPath}", Times.Once());
        }

        [TestMethod]
        public async Task Export_WhenFilesPresent_ReadsAndExportsFieldsCorrectlyToCsv()
        {
            // Arrange
            var targetConfiguration = GetTestTargetConfiguration();
            var files = new Dictionary<string, TargetConfiguration> { { "file1.yaml", targetConfiguration } };

            _mockFileHandler
                .Setup(x => x.GetFiles(OutputFolderPath))
                .Returns(files.Select(x => x.Key).ToArray);

            var targetSchema = new FileData(null, null);
            _mockFileHandler.Setup(x => x.ReadEmbeddedResourceFromFileName(Constants.FileNames.JourneyConfigurationSchema, out targetSchema))
                .Returns(true);

            foreach (var fileName in files)
            {
                SetupReader(fileName.Key, fileName.Value);
            }

            // Act
            var result = await _systemUnderTest.Export();

            // Assert
            _mockTextWriter.Verify(s => s.WriteLine("Target.All,Target.OdsCode,Target.OdsCodes,Target.CcgCode,Target.Supplier,Journeys.Appointments.InformaticaUrl,Journeys.CdssAdmin.ServiceDefinition,Journeys.CdssAdmin.KnownGeneralServiceDefinitions,Journeys.CdssAdvice.ServiceDefinition,Journeys.CdssAdvice.KnownGeneralServiceDefinitions,Journeys.CoronavirusInformation,Journeys.Documents,Journeys.HomeScreen.PublicHealthNotifications,Journeys.Im1Messaging.IsEnabled,Journeys.Im1Messaging.CanDeleteMessages,Journeys.Im1Messaging.CanUpdateReadStatus,Journeys.Im1Messaging.RequiresDetailsRequest,Journeys.Im1Messaging.SendMessageSubject,Journeys.MedicalRecord.Version,Journeys.Messaging,Journeys.Ndop,Journeys.NominatedPharmacy,Journeys.Notifications,Journeys.NotificationPrompt,Journeys.OneOneOne,Journeys.SilverIntegrations.AccountAdmin,Journeys.SilverIntegrations.AppointmentBookings,Journeys.SilverIntegrations.CarePlans,Journeys.SilverIntegrations.Consultations,Journeys.SilverIntegrations.ConsultationsAdmin,Journeys.SilverIntegrations.HealthTrackers,Journeys.SilverIntegrations.Libraries,Journeys.SilverIntegrations.Medicines,Journeys.SilverIntegrations.Messages,Journeys.SilverIntegrations.Participation,Journeys.SilverIntegrations.RecordSharing,Journeys.SilverIntegrations.SecondaryAppointments,Journeys.SilverIntegrations.TestResults,Journeys.SilverIntegrations.VaccineRecord,Journeys.Supplier,Journeys.SupportsLinkedProfiles,Journeys.UserInfo,Journeys.Wayfinder.IsEnabled"));
            _mockTextWriter.Verify(s => s.WriteLine(",TEST001,,,Emis,testInformaticaUrl,,,,,,,,True,,,,,,,,,,,,,,,,,,,,pkb; engage,,,,,,,,,"));

            result.Should().Be(0);
        }

        private static TargetConfiguration GetTestTargetConfiguration()
        {
            return new TargetConfiguration
            {
                Target = new Target
                {
                    OdsCode = "TEST001",
                    Supplier = Supplier.Emis,
                },
                Journeys = new Journeys
                {
                    Appointments = new Appointments
                    {
                        Provider = AppointmentsProvider.im1,
                        InformaticaUrl = "testInformaticaUrl",
                    },
                    Im1Messaging = new Im1Messaging
                    {
                        IsEnabled = true,
                    },
                    SilverIntegrations = new SilverIntegrations
                    {
                        Messages = new List<MessagesProvider>
                        {
                            MessagesProvider.pkb, MessagesProvider.engage
                        }
                    },
                },
            };
        }

        private void SetupReader<T>(string fileName, T result)
            where T : class, new()
        {
            var mockYamlReader = _fixture.Create<Mock<IYamlReader<T>>>();
            mockYamlReader.Setup(x => x.GetData()).ReturnsAsync(result);

            _mockYamlReaderFactory.Setup(x => x.GetReader<T>(fileName, It.IsAny<FileData>()))
                .Returns(mockYamlReader.Object);
        }
    }
}
