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
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils.Steps;
 using UnitTestHelper;

 namespace NHSOnline.Backend.ServiceJourneyRulesApi.UnitTests.RuleConfiguration.Utils.Steps
{
    [TestClass]
    public class LoadRequiredFilesTests
    {

        private LoadRequiredFiles _step;
        private Mock<IGpInfoReader> _mockGpInfoReader;
        private Mock<IFileHandler> _mockFileHandler;
        private Mock<ILogger<LoadRequiredFiles>> _mockLogger;
        
        [TestInitialize]
        public void TestInitialize()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
            
            _mockGpInfoReader = fixture.Freeze<Mock<IGpInfoReader>>();
            _mockFileHandler = fixture.Freeze<Mock<IFileHandler>>();
            _mockLogger = fixture.Freeze<Mock<ILogger<LoadRequiredFiles>>>();
            _step = fixture.Create<LoadRequiredFiles>();
        }

        [TestMethod]
        public void Execute_WhenContextIsNotPresent_ThrowsAnException()
        {
            // act
            Func<Task> act = async () => await _step.Execute(null);
            
            // assert
            act.Should().Throw<ArgumentException>().And.ParamName.Should().Be("context");
        }
        
        [TestMethod]
        public async Task Execute_WhenGpInfoDoesNotExist_ReturnsFalse()
        {
            // arrange
            var context = new ConfigurationContext();
            _mockGpInfoReader.Setup(s => s.GetGpInfo(It.IsAny<string>()))
                .Returns((IDictionary<string, GpInfo>)null);
            
            // act
            var result = await _step.Execute(context);
            
            // assert
            _mockLogger.VerifyLogger(LogLevel.Critical, Times.Once());
            
            result.Should().BeFalse();
            context.GpInfos.Should().BeNull();
        }

        [TestMethod]
        public async Task Execute_WhenRulesSchemaDoesNotExist_ReturnsFalse()
        {
            // arrange
            var context = new ConfigurationContext();
            FileData fileData;
            _mockFileHandler.Setup(x => x.ReadEmbeddedResourceFromFileName(Constants.FileNames.RulesSchema, out fileData))
                .Returns(false);
            
            
            // act
            var result = await _step.Execute(context);
            
            // assert
            _mockLogger.VerifyLogger(LogLevel.Critical, Times.Once());
            
            result.Should().BeFalse();
            context.RulesSchema.Should().BeNull();
        }

        [TestMethod]
        public async Task Execute_WhenJourneysSchemaDoesNotExist_ReturnsFalse()
        {
            // arrange
            var context = new ConfigurationContext();
            FileData fileData;
            _mockFileHandler.Setup(x => x.ReadEmbeddedResourceFromFileName(Constants.FileNames.JourneyConfigurationSchema, out fileData))
                .Returns(false);
            
            // act
            var result = await _step.Execute(context);
            
            // assert
            _mockLogger.VerifyLogger(LogLevel.Critical, Times.Once());
            
            result.Should().BeFalse();
            context.TargetSchema.Should().BeNull();
        }

        [TestMethod]
        public async Task Execute_WhenAllFilesExist_ReturnsTrue()
        {
            // arrange
            var context = new ConfigurationContext();
            var targetSchema = new FileData(null, null);
            var rulesSchema = new FileData(null, null);
            
            _mockFileHandler.Setup(x => x.ReadEmbeddedResourceFromFileName(Constants.FileNames.JourneyConfigurationSchema, out targetSchema))
                .Returns(true);
            _mockFileHandler.Setup(x => x.ReadEmbeddedResourceFromFileName(Constants.FileNames.RulesSchema, out rulesSchema))
                .Returns(true);
            _mockGpInfoReader.Setup(s => s.GetGpInfo(It.IsAny<string>()))
                .Returns(new Dictionary<string, GpInfo>());
            
            // act
            var result = await _step.Execute(context);
            
            // assert
            _mockLogger.VerifyNoOtherCalls();
            
            result.Should().BeTrue();
            context.TargetSchema.Should().BeSameAs(targetSchema);
            context.RulesSchema.Should().BeSameAs(rulesSchema);
            context.GpInfos.Should().NotBeNull();
        }
    }
}