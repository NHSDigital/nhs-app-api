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
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Models;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils.Steps;
using UnitTestHelper;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.UnitTests.RuleConfiguration.Utils.Steps
{
    [TestClass]
    public class LoadConfigurationFilesTests
    {
        private readonly string _rulesConfigurationFolder = "c:/folder/rules";
        private readonly string _journeysConfigurationFolder = "c:/folder/journeys";

        private IValidatorStep _step;
        private ILoadStep _loadStep;
        private IFixture _fixture;
        private Mock<ILogger<LoadConfigurationFiles>> _mockLogger;
        private Mock<IFileHandler> _mockFileHandler;
        private Mock<IYamlReaderFactory> _mockYamlReaderFactory;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
            
            var mockServiceJourneyRulesConfiguration =
                _fixture.Freeze<Mock<IServiceJourneyRulesConfiguration>>();
            mockServiceJourneyRulesConfiguration.SetupGet(c => c.RulesFolderPath)
                .Returns(_rulesConfigurationFolder);
            mockServiceJourneyRulesConfiguration.SetupGet(c => c.JourneysFolderPath)
                .Returns(_journeysConfigurationFolder);

            _mockLogger = _fixture.Freeze<Mock<ILogger<LoadConfigurationFiles>>>();
            _mockFileHandler = _fixture.Freeze<Mock<IFileHandler>>();
            _mockYamlReaderFactory = _fixture.Freeze<Mock<IYamlReaderFactory>>();

            _step = _fixture.Create<LoadConfigurationFiles>();
            _loadStep = _fixture.Create<LoadConfigurationFiles>();
        }

        [TestMethod]
        public void Execute_WhenContextIsNotPresent_ThrowsAnException()
        {
            // Act
            Func<Task> act = async () => await _step.Execute(null);

            // Assert
            act.Should().Throw<AggregateException>()
                .And.InnerExceptions.Should().HaveCount(3)
                .And.AllBeOfType<ArgumentNullException>()
                .And.Contain(x => ((ArgumentNullException) x).ParamName.Equals("context", StringComparison.Ordinal))
                .And.Contain(x => ((ArgumentNullException) x).ParamName.Equals("RulesSchema", StringComparison.Ordinal))
                .And.Contain(x =>
                    ((ArgumentNullException) x).ParamName.Equals("TargetSchema", StringComparison.Ordinal));
        }

        [TestMethod]
        public void Execute_WhenContextHasNoValues_ThrowsAnException()
        {
            // Arrange
            var context = new ConfigurationContext();

            // Act
            Func<Task> act = async () => await _step.Execute(context);

            // Assert
            act.Should().Throw<AggregateException>()
                .And.InnerExceptions.Should().HaveCount(2)
                .And.AllBeOfType<ArgumentNullException>()
                .And.Contain(x => ((ArgumentNullException) x).ParamName.Equals("RulesSchema", StringComparison.Ordinal))
                .And.Contain(x =>
                    ((ArgumentNullException) x).ParamName.Equals("TargetSchema", StringComparison.Ordinal));
        }

        [TestMethod]
        public void Execute_WhenContextRulesSchemaIsNotPresent_ThrowsAnException()
        {
            // Arrange
            var context = new ConfigurationContext
            {
                TargetSchema = _fixture.Create<FileData>()
            };

            // Act
            Func<Task> act = async () => await _step.Execute(context);

            // Assert
            act.Should().Throw<ArgumentException>().And.ParamName.Should().Be("RulesSchema");
        }

        [TestMethod]
        public void Execute_WhenContextTargetSchemaIsNotPresent_ThrowsAnException()
        {
            // Arrange
            var context = new ConfigurationContext
            {
                RulesSchema = _fixture.Create<FileData>()
            };

            // Act
            Func<Task> act = async () => await _step.Execute(context);

            // Assert
            act.Should().Throw<ArgumentException>().And.ParamName.Should().Be("TargetSchema");
        }

        [TestMethod]
        public async Task Execute_WhenThereIsNoRulesConfigurationFile_ReturnsFalse()
        {
            // Arrange
            var context = new ConfigurationContext
            {
                TargetSchema = _fixture.Create<FileData>(),
                RulesSchema = _fixture.Create<FileData>()
            };

            SetupFileHandler(_rulesConfigurationFolder, Array.Empty<string>());

            // Act
            var result = await _step.Execute(context);

            // Assert
            _mockLogger.VerifyLogger(LogLevel.Error, "There must be exactly 1 rules configuration file in",
                Times.Once());
            _mockLogger.VerifyLogger(LogLevel.Critical, "Error reading rules config file", Times.Once());
            _mockLogger.VerifyNoOtherCalls();

            result.Should().BeFalse();
        }

        [TestMethod]
        public async Task Execute_WhenThereAreMultipleRulesConfigurationFiles_ReturnsFalse()
        {
            // Arrange
            var context = new ConfigurationContext
            {
                TargetSchema = _fixture.Create<FileData>(),
                RulesSchema = _fixture.Create<FileData>()
            };

            SetupFileHandler(_rulesConfigurationFolder, new[] { "rule1.yaml", "rule2.yaml" });

            // Act
            var result = await _step.Execute(context);

            // Assert
            _mockLogger.VerifyLogger(LogLevel.Error, "There must be exactly 1 rules configuration file in",
                Times.Once());
            _mockLogger.VerifyLogger(LogLevel.Critical, "Error reading rules config file", Times.Once());
            _mockLogger.VerifyNoOtherCalls();

            result.Should().BeFalse();
        }

        [TestMethod]
        public async Task Execute_WhenItFailsToReadTheRulesConfigurationFile_ReturnsFalse()
        {
            // Arrange
            var context = new ConfigurationContext
            {
                TargetSchema = _fixture.Create<FileData>(),
                RulesSchema = _fixture.Create<FileData>()
            };

            const string fileName = "rule1.yaml";
            SetupFileHandler(_rulesConfigurationFolder, new[] { fileName });
            SetupReader(fileName, (Rules) null);

            // Act
            var result = await _step.Execute(context);

            // Assert
            _mockLogger.VerifyLogger(LogLevel.Critical, "Error reading rules config file", Times.Once());
            _mockLogger.VerifyNoOtherCalls();

            result.Should().BeFalse();
        }

        [TestMethod]
        public async Task Execute_WhenThereAreNoTargetConfigurationFilesOnAllFolders_ReturnsFalse()
        {
            // Arrange
            var context = new ConfigurationContext
            {
                TargetSchema = _fixture.Create<FileData>(),
                RulesSchema = _fixture.Create<FileData>()
            };

            const string ruleFileName = "rule1.yaml";
            var rules = new Rules
            {
                FolderOrder = new List<string> { "folder1", "folder2" }
            };

            SetupFileHandler(_rulesConfigurationFolder, new[] { ruleFileName });
            SetupReader(ruleFileName, rules);

            var folder1Path = Path.Join(_journeysConfigurationFolder, rules.FolderOrder.First());
            var folder2Path = Path.Join(_journeysConfigurationFolder, rules.FolderOrder.Last());
            SetupFileHandler(new Dictionary<string, string[]>
            {
                { folder1Path, Array.Empty<string>() },
                { folder2Path, Array.Empty<string>() },
            });

            // Act
            var result = await _step.Execute(context);

            // Assert
            _mockLogger.VerifyLogger(LogLevel.Warning, $"found in directory {folder1Path}", Times.Once());
            _mockLogger.VerifyLogger(LogLevel.Warning, $"found in directory {folder2Path}", Times.Once());
            _mockLogger.VerifyLogger(LogLevel.Critical, "Error reading target configuration files", Times.Once());
            _mockLogger.VerifyNoOtherCalls();

            result.Should().BeFalse();
        }

        [TestMethod]
        public async Task Execute_WhenSomeTargetConfigurationFailToRead_ReturnsTrue()
        {
            // Arrange
            var context = new ConfigurationContext
            {
                TargetSchema = _fixture.Create<FileData>(),
                RulesSchema = _fixture.Create<FileData>()
            };

            const string ruleFileName = "rule1.yaml";
            var rules = new Rules
            {
                FolderOrder = new List<string> { "folder1", "folder2" }
            };

            SetupFileHandler(_rulesConfigurationFolder, new[] { ruleFileName });
            SetupReader(ruleFileName, rules);

            const string targetFileName = "target.yaml";
            const string target2FileName = "target2.yaml";
            const string target3FileName = "target3.yaml";
            const string target4FileName = "target4.yaml";
            var folder1Path = Path.Join(_journeysConfigurationFolder, rules.FolderOrder.First());
            var folder2Path = Path.Join(_journeysConfigurationFolder, rules.FolderOrder.Last());
            SetupFileHandler(new Dictionary<string, string[]>
            {
                { folder1Path, new[] { targetFileName, target2FileName } },
                { folder2Path, new[] { target3FileName, target4FileName } },
            });

            var targetConfiguration = _fixture.Create<TargetConfiguration>();
            var target3Configuration = _fixture.Create<TargetConfiguration>();
            var target4Configuration = _fixture.Create<TargetConfiguration>();
            
            SetupReader(targetFileName, targetConfiguration);
            SetupReader(target2FileName, (TargetConfiguration) null);
            SetupReader(target3FileName, target3Configuration);
            SetupReader(target4FileName, target4Configuration);

            // Act
            var result = await _step.Execute(context);

            // Assert
            _mockLogger.VerifyLogger(LogLevel.Critical, "Error reading target configuration files", Times.Once());
            _mockLogger.VerifyNoOtherCalls();

            result.Should().BeFalse();
        }

        [TestMethod]
        public async Task Execute_WhenThereAreNoTargetConfigurationFilesOnSomeFolders_ReturnsTrue()
        {
            // Arrange
            var context = new ConfigurationContext
            {
                TargetSchema = _fixture.Create<FileData>(),
                RulesSchema = _fixture.Create<FileData>()
            };

            const string ruleFileName = "rule1.yaml";
            var rules = new Rules
            {
                FolderOrder = new List<string> { "folder1", "folder2" }
            };

            SetupFileHandler(_rulesConfigurationFolder, new[] { ruleFileName });
            SetupReader(ruleFileName, rules);

            const string targetFileName = "target.yaml";
            var folder1Path = Path.Join(_journeysConfigurationFolder, rules.FolderOrder.First());
            var folder2Path = Path.Join(_journeysConfigurationFolder, rules.FolderOrder.Last());
            SetupFileHandler(new Dictionary<string, string[]>
            {
                { folder1Path, new[] { targetFileName } },
                { folder2Path, Array.Empty<string>() },
            });

            var targetConfiguration = _fixture.Create<TargetConfiguration>();
            SetupReader(targetFileName, targetConfiguration);

            // Act
            var result = await _step.Execute(context);

            // Assert
            _mockLogger.VerifyLogger(LogLevel.Warning, $"found in directory {folder2Path}", Times.Once());
            _mockLogger.VerifyNoOtherCalls();

            result.Should().BeTrue();
            context.FolderConfigurations.Should().NotBeNull()
                .And.HaveCount(1)
                .And.ContainKey(folder1Path)
                .WhichValue.Should().NotBeNull()
                .And.HaveCount(1)
                .And.Contain(targetConfiguration);
        }
        
        [TestMethod]
        public void ExecuteWithLoadContext_WhenTargetSchemaIsNotPresent_ThrowsAnException()
        {
            // Arrange
            var context = new LoadContext();

            // Act
            Func<Task> act = async () => await _loadStep.Execute(context);

            // Assert
            act.Should().Throw<ArgumentException>().And.ParamName.Should().Be("TargetSchema");
        }
        
        [TestMethod]
        public async Task ExecuteWithLoadContext_WhenItFailsToReadTheMergedConfigurationFile_ReturnsFalse()
        {
            // Arrange
            var context = new LoadContext
            {
                TargetSchema = _fixture.Create<FileData>()
            };

            const string fileName = "rule1.yaml";
            SetupFileHandler(_rulesConfigurationFolder, new[] { fileName });
            SetupReader(fileName, (Rules) null);

            // Act
            var result = await _loadStep.Execute(context);

            // Assert
            _mockLogger.VerifyLogger(LogLevel.Critical, "Error reading target configuration files.", Times.Once());

            result.Should().BeFalse();
        }
        
        [TestMethod]
        public async Task ExecuteWithLoadContext_WhenThereAreNoTargetConfigurationFiles_ReturnsFalse()
        {
            // Arrange
            var context = new LoadContext
            {
                TargetSchema = _fixture.Create<FileData>()
            };

            const string targetFileName = "target.yaml";
            SetupFileHandler(_rulesConfigurationFolder, Array.Empty<string>());

            var targetConfiguration = _fixture.Create<TargetConfiguration>();
            SetupReader(targetFileName, targetConfiguration);

            // Act
            var result = await _loadStep.Execute(context);

            // Assert

            result.Should().BeFalse();
        }

        private void SetupFileHandler(Dictionary<string, string[]> entries)
        {
            foreach (var (key, value) in entries)
            {
                SetupFileHandler(key, value);
            }
        }

        private void SetupFileHandler(string folderPath, string[] files)
        {
            _mockFileHandler.Setup(c => c.GetFiles(folderPath))
                .Returns(files);
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