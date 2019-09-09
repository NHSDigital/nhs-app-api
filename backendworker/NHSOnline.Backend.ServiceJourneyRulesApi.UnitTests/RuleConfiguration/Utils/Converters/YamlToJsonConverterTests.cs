using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Models;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils.Converters;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.UnitTests.RuleConfiguration.Utils.Converters
{
    [TestClass]
    public class YamlToJsonConverterTests
    {
        private const string PlaceholderFileName = "test_file.yaml";
        private IYamlToJsonConverter _yamlToJsonConverter;

        [TestInitialize]
        public void TestInitialize()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
            
            _yamlToJsonConverter = fixture.Create<YamlToJsonConverter>();
        }

        [TestMethod]
        public void Convert_WhenCalledWithNullFileData_ReturnsError()
        {
            // Act
            var result = _yamlToJsonConverter.Convert(null, out var convertedFile);

            // Assert
            result.Should().BeFalse();
            convertedFile.Should().BeNull();
        }

        [TestMethod]
        public void Convert_WhenCalledWithNullData_ReturnsError()
        {
            // Arrange
            var fileWithInvalidData = new FileData(PlaceholderFileName, null);
            
            // Act
            var result = _yamlToJsonConverter.Convert(fileWithInvalidData, out var convertedFile);

            // Assert
            result.Should().BeFalse();
            convertedFile.Should().BeNull();
        }

        [TestMethod]
        public void Convert_WhenCalledWithYamlData_ReturnsNoError()
        {
            // Arrange
            var fileWithData = new FileData(PlaceholderFileName, string.Empty);
            
            // Act
            var result = _yamlToJsonConverter.Convert(fileWithData, out var convertedFile);

            // Assert
            result.Should().BeTrue();
            convertedFile.Data.Should().NotBeNull();
            convertedFile.Name.Should().Be(fileWithData.Name);
        }
    }
}