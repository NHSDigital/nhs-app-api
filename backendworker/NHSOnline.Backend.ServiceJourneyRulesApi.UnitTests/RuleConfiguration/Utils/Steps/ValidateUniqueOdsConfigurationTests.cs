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
            // Act
            Func<Task> act = async () => await _step.Execute(null);

            // Assert
            act.Should().Throw<AggregateException>()
                .And.InnerExceptions.Should().HaveCount(3)
                .And.AllBeOfType<ArgumentNullException>()
                .And.Contain(x => ((ArgumentNullException) x).ParamName.Equals("context", StringComparison.Ordinal))
                .And.Contain(x =>
                    ((ArgumentNullException) x).ParamName.Equals("FolderConfigurations", StringComparison.Ordinal))
                .And.Contain(x => ((ArgumentNullException) x).ParamName.Equals("GpInfos", StringComparison.Ordinal));
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
                .And.Contain(x =>
                    ((ArgumentNullException) x).ParamName.Equals("FolderConfigurations", StringComparison.Ordinal))
                .And.Contain(x => ((ArgumentNullException) x).ParamName.Equals("GpInfos", StringComparison.Ordinal));
        }

        [TestMethod]
        public void Execute_WhenContextGpInfosIsNotPresent_ThrowsAnException()
        {
            // Arrange
            var context = new ConfigurationContext
            {
                FolderConfigurations = _fixture.Create<Dictionary<string, IEnumerable<TargetConfiguration>>>()
            };

            // Act
            Func<Task> act = async () => await _step.Execute(context);

            // Assert
            act.Should().Throw<ArgumentException>().And.ParamName.Should().Be("GpInfos");
        }

        [TestMethod]
        public void Execute_WhenContextFolderConfigurationsIsNotPresent_ThrowsAnException()
        {
            // Arrange
            var context = new ConfigurationContext
            {
                GpInfos = _fixture.Create<Dictionary<string, GpInfo>>()
            };

            // Act
            Func<Task> act = async () => await _step.Execute(context);

            // Assert
            act.Should().Throw<ArgumentException>().And.ParamName.Should().Be("FolderConfigurations");
        }

        [TestMethod]
        public async Task Execute_WhenEmptyFolderConfigurations_ReturnsTrueWithEmptyList()
        {
            // Arrange
            var context = new ConfigurationContext
            {
                GpInfos = _fixture.Create<Dictionary<string, GpInfo>>(),
                FolderConfigurations = new Dictionary<string, IEnumerable<TargetConfiguration>>()
            };

            // Act
            var result = await _step.Execute(context);

            // Assert
            result.Should().BeTrue();
            context.FolderConfigurations.Should().NotBeNull().And.BeEmpty();
        }

        [TestMethod]
        public async Task Execute_WhenGpInfoIsEmptyAndTargetIsNotAnOdsCode_ReturnsTrueWithEmptyList()
        {
            // Arrange
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

            // Act
            var result = await _step.Execute(context);

            // Assert
            result.Should().BeTrue();
            context.FolderOdsJourneys.Should().NotBeNull().And.HaveCount(1);
            context.FolderOdsJourneys.Should().ContainKey(folderName)
                .WhichValue.Should().NotBeNull().And.BeEmpty();
        }

        [TestMethod]
        public async Task Execute_WhenThereIsAFolderOdsConflict_ReturnsFalse()
        {
            // Arrange
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
                            new TargetConfiguration
                                { Target = new Target { All = "*" }, Journeys = _fixture.Create<Journeys>() },
                            new TargetConfiguration
                                { Target = new Target { OdsCode = "3" }, Journeys = _fixture.Create<Journeys>() },
                        }
                    }
                }
            };

            // Act
            var result = await _step.Execute(context);

            // Assert
            _mockLogger.VerifyLogger(LogLevel.Error, "Error applying '3' ODS configuration to 'folder1' list",
                typeof(ArgumentException), Times.Once());

            _mockLogger.VerifyLogger(LogLevel.Critical, "Error validating unique ODS code per folder", Times.Once());
            _mockLogger.VerifyNoOtherCalls();

            result.Should().BeFalse();
            context.FolderOdsJourneys.Should().BeNull();
        }

        [TestMethod]
        public async Task Execute_WhenGpInfoHasMatchingTargets_SetsFolderOdsJourneys()
        {
            // Arrange
            const string folderName = "folder1";
            const string folderName2 = "folder2";
            const string ccgCode = "ccgCode1";
            const string ccgCode2 = "ccgCode2";

            var allConfiguration = CreateTargetConfiguration(all: "*");
            var ccgConfiguration = CreateTargetConfiguration(ccgCode: ccgCode2);
            var odsCodeConfiguration = CreateTargetConfiguration(odsCode: "9");
            var odsCodesConfiguration = CreateTargetConfiguration(odsCodes: new List<string> { "8", "10" });
            var supplierConfiguration = CreateTargetConfiguration(Supplier.Microtest);

            var context = new ConfigurationContext
            {
                GpInfos = new Dictionary<string, GpInfo>
                {
                    { "1", new GpInfo { Ods = "1", CcgCode = ccgCode, Supplier = GpInfoSupplier.Tpp } },
                    { "2", new GpInfo { Ods = "2", CcgCode = ccgCode2, Supplier = GpInfoSupplier.Emis } },
                    { "3", new GpInfo { Ods = "3", CcgCode = ccgCode2, Supplier = GpInfoSupplier.Vision } },
                    { "4", new GpInfo { Ods = "4", CcgCode = ccgCode, Supplier = GpInfoSupplier.Microtest} }
                },
                FolderConfigurations = new Dictionary<string, IEnumerable<TargetConfiguration>>
                {
                    { folderName, new[] { allConfiguration, odsCodeConfiguration } },
                    { folderName2, new[] { ccgConfiguration, supplierConfiguration, odsCodesConfiguration } },
                }
            };

            // Act
            var result = await _step.Execute(context);

            // Assert
            result.Should().BeTrue();
            context.FolderOdsJourneys.Should().NotBeNull().And.HaveCount(2);
            context.FolderOdsJourneys.Should().ContainKeys(folderName, folderName2);

            var firstFolderConfigurations = context.FolderOdsJourneys[folderName];
            firstFolderConfigurations.Should().NotBeNull().And.HaveCount(4);

            firstFolderConfigurations.Should().ContainKey("1").WhichValue.Should()
                .NotBe(allConfiguration.Journeys.AddSupplier(Supplier.Tpp)).And
                .BeEquivalentTo(allConfiguration.Journeys.AddSupplier(Supplier.Tpp));
            firstFolderConfigurations.Should().ContainKey("2").WhichValue.Should()
                .NotBe(allConfiguration.Journeys.AddSupplier(Supplier.Emis)).And
                .BeEquivalentTo(allConfiguration.Journeys.AddSupplier(Supplier.Emis));
            firstFolderConfigurations.Should().ContainKey("3").WhichValue.Should()
                .NotBe(allConfiguration.Journeys.AddSupplier(Supplier.Vision)).And
                .BeEquivalentTo(allConfiguration.Journeys.AddSupplier(Supplier.Vision));
            firstFolderConfigurations.Should().ContainKey("4").WhichValue.Should()
                .NotBe(odsCodeConfiguration.Journeys.AddSupplier(Supplier.Microtest)).And
                .BeEquivalentTo(allConfiguration.Journeys.AddSupplier(Supplier.Microtest));

            var secondFolderConfigurations = context.FolderOdsJourneys[folderName2];
            secondFolderConfigurations.Should().NotBeNull().And.HaveCount(3);

            secondFolderConfigurations.Should().ContainKey("2").WhichValue.Should()
                .NotBe(ccgConfiguration.Journeys.AddSupplier(Supplier.Emis)).And
                .BeEquivalentTo(ccgConfiguration.Journeys.AddSupplier(Supplier.Emis)    );
            secondFolderConfigurations.Should().ContainKey("3").WhichValue.Should()
                .NotBe(ccgConfiguration.Journeys.AddSupplier(Supplier.Vision)).And
                .BeEquivalentTo(ccgConfiguration.Journeys.AddSupplier(Supplier.Vision));
            secondFolderConfigurations.Should().ContainKey("4").WhichValue.Should()
                .NotBe(odsCodesConfiguration.Journeys.AddSupplier(Supplier.Microtest)).And
                .BeEquivalentTo(supplierConfiguration.Journeys.AddSupplier(Supplier.Microtest));
        }

        private TargetConfiguration CreateTargetConfiguration(Supplier supplier = Supplier.Unknown,
            string odsCode = null, List<string> odsCodes = null, string ccgCode = null, string all = null)
        {
            return new TargetConfiguration
            {
                Target = new Target
                    { Supplier = supplier, OdsCode = odsCode, OdsCodes = odsCodes, CcgCode = ccgCode, All = all },
                Journeys = _fixture.Create<Journeys>()
            };
        }
    }
}