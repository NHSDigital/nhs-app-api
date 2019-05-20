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
using NHSOnline.Backend.Support;
using UnitTestHelper;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.UnitTests.RuleConfiguration.Utils.Steps
{
    [TestClass]
    public class ValidateUniqueOdsConfigurationTests
    {
        private IValidatorStep _step;
        private IFixture _fixture;
        private Mock<ILogger<ValidateUniqueOdsConfiguration>> _mockLogger;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            _mockLogger = _fixture.Freeze<Mock<ILogger<ValidateUniqueOdsConfiguration>>>();
            _step = _fixture.Create<ValidateUniqueOdsConfiguration>();
        }

        [TestMethod]
        public void Execute_WhenContextIsNotPresent_ThrowsAnException()
        {
            // act
            Func<Task> act = async () => await _step.Execute(null);

            // assert
            act.Should().Throw<AggregateException>()
                .And.InnerExceptions.Should().HaveCount(3)
                .And.AllBeOfType<ArgumentNullException>()
                .And.Contain(x => ((ArgumentNullException) x).ParamName.Equals("context", StringComparison.Ordinal))
                .And.Contain(x =>
                    ((ArgumentNullException) x).ParamName.Equals("FolderConfigurations", StringComparison.Ordinal))
                .And.Contain(x => ((ArgumentNullException) x).ParamName.Equals("GpInfos", StringComparison.Ordinal));
            ;
        }

        [TestMethod]
        public void Execute_WhenContextHasNoValues_ThrowsAnException()
        {
            // arrange
            var context = new ConfigurationContext();

            // act
            Func<Task> act = async () => await _step.Execute(context);

            // assert
            act.Should().Throw<AggregateException>()
                .And.InnerExceptions.Should().HaveCount(2)
                .And.AllBeOfType<ArgumentNullException>()
                .And.Contain(x =>
                    ((ArgumentNullException) x).ParamName.Equals("FolderConfigurations", StringComparison.Ordinal))
                .And.Contain(x => ((ArgumentNullException) x).ParamName.Equals("GpInfos", StringComparison.Ordinal));
        }

        [TestMethod]
        public void Execute_WhenContextGpInfosIsNotPresent_ThrowsAnException()
        {
            // arrange
            var context = new ConfigurationContext
            {
                FolderConfigurations = _fixture.Create<Dictionary<string, IEnumerable<TargetConfiguration>>>()
            };

            // act
            Func<Task> act = async () => await _step.Execute(context);

            // assert
            act.Should().Throw<ArgumentException>().And.ParamName.Should().Be("GpInfos");
        }

        [TestMethod]
        public void Execute_WhenContextFolderConfigurationsIsNotPresent_ThrowsAnException()
        {
            // arrange
            var context = new ConfigurationContext
            {
                GpInfos = _fixture.Create<Dictionary<string, GpInfo>>()
            };

            // act
            Func<Task> act = async () => await _step.Execute(context);

            // assert
            act.Should().Throw<ArgumentException>().And.ParamName.Should().Be("FolderConfigurations");
        }

        [TestMethod]
        public async Task Execute_WhenEmptyFolderConfigurations_ReturnsTrueWithEmptyList()
        {
            // arrange
            var context = new ConfigurationContext
            {
                GpInfos = _fixture.Create<Dictionary<string, GpInfo>>(),
                FolderConfigurations = new Dictionary<string, IEnumerable<TargetConfiguration>>()
            };

            // act
            var result = await _step.Execute(context);

            // assert
            result.Should().BeTrue();
            context.FolderConfigurations.Should().NotBeNull().And.BeEmpty();
        }

        [TestMethod]
        public async Task Execute_WhenGpInfoIsEmptyAndTargetIsNotAnOdsCode_ReturnsTrueWithEmptyList()
        {
            // arrange
            const string folderName = "folder1";
            var context = new ConfigurationContext
            {
                GpInfos = new Dictionary<string, GpInfo>(),
                FolderConfigurations = new Dictionary<string, IEnumerable<TargetConfiguration>>
                {
                    {
                        folderName,
                        new[]
                        {
                            new TargetConfiguration { Target = new Target { CcgCode = "ccgCode1" } },
                            new TargetConfiguration { Target = new Target { Supplier = Supplier.Emis } },
                            new TargetConfiguration { Target = new Target { All = "*" } }
                        }
                    }
                }
            };

            // act
            var result = await _step.Execute(context);

            // assert
            result.Should().BeTrue();
            context.FolderOdsJourneys.Should().NotBeNull().And.HaveCount(1);
            context.FolderOdsJourneys.Should().ContainKey(folderName)
                .WhichValue.Should().NotBeNull().And.BeEmpty();
        }

        [TestMethod]
        public async Task Execute_WhenThereIsAFolderConflict_ReturnsFalse()
        {
            // arrange
            var context = new ConfigurationContext
            {
                GpInfos = new Dictionary<string, GpInfo>
                {
                    { "1", new GpInfo { Ods = "1", CcgCode = "ccgCode1", Supplier = GpInfoSupplier.Tpp } },
                    { "2", new GpInfo { Ods = "2", CcgCode = "ccgCode2", Supplier = GpInfoSupplier.Emis } },
                    { "3", new GpInfo { Ods = "3", CcgCode = "ccgCode2", Supplier = GpInfoSupplier.Vision } }
                },
                FolderConfigurations = new Dictionary<string, IEnumerable<TargetConfiguration>>
                {
                    {
                        "folder1",
                        new[]
                        {
                            new TargetConfiguration { Target = new Target { All = "*" } },
                            new TargetConfiguration { Target = new Target { OdsCode = "3" } },
                        }
                    }
                }
            };

            // act
            var result = await _step.Execute(context);

            // assert
            _mockLogger.VerifyLogger(LogLevel.Error, "Error applying '3' ODS configuration to 'folder1' list",
                typeof(ArgumentException), Times.Once());

            _mockLogger.VerifyLogger(LogLevel.Critical, "Error validating unique ODS code per folder", Times.Once());
            _mockLogger.VerifyNoOtherCalls();

            result.Should().BeFalse();
            context.FolderOdsJourneys.Should().BeNull();
        }

        [TestMethod]
        public async Task Execute_WhenGpInfoHasMatchingTargets_SetsTheMatchingTargets()
        {
            // arrange
            const string folderName = "folder1";
            const string folderName2 = "folder2";
            const string folderName3 = "folder3";
            const string ccgCode = "ccgCode1";
            const string ccgCode2 = "ccgCode2";

            var context = new ConfigurationContext
            {
                GpInfos = new Dictionary<string, GpInfo>
                {
                    { "1", new GpInfo { Ods = "1", CcgCode = ccgCode, Supplier = GpInfoSupplier.Tpp } },
                    { "2", new GpInfo { Ods = "2", CcgCode = ccgCode2, Supplier = GpInfoSupplier.Emis } },
                    { "3", new GpInfo { Ods = "3", CcgCode = ccgCode2, Supplier = GpInfoSupplier.Vision } }
                },
                FolderConfigurations = new Dictionary<string, IEnumerable<TargetConfiguration>>
                {
                    {
                        folderName,
                        new[]
                        {
                            new TargetConfiguration { Target = new Target { All = "*" } }
                        }
                    },
                    {
                        folderName2,
                        new[]
                        {
                            new TargetConfiguration { Target = new Target { CcgCode = ccgCode } },
                            new TargetConfiguration { Target = new Target { OdsCode = "9" } },
                        }
                    },
                    {
                        folderName3,
                        new[]
                        {
                            new TargetConfiguration { Target = new Target { Supplier = Supplier.Tpp } },
                            new TargetConfiguration { Target = new Target { CcgCode = ccgCode2 } },
                        }
                    }
                }
            };

            // act
            var result = await _step.Execute(context);

            // assert
            result.Should().BeTrue();
            context.FolderOdsJourneys.Should().NotBeNull().And.HaveCount(3);
            context.FolderOdsJourneys.Should().ContainKey(folderName)
                .WhichValue.Should().NotBeNull().And.HaveCount(3);
            context.FolderOdsJourneys.Should().ContainKey(folderName2)
                .WhichValue.Should().NotBeNull().And.HaveCount(2);
            context.FolderOdsJourneys.Should().ContainKey(folderName3)
                .WhichValue.Should().NotBeNull().And.HaveCount(3);
        }
    }
}