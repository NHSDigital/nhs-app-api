using AutoFixture;
using AutoFixture.AutoMoq;
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
            var result = _yamlToJsonConverter.Convert(null, out var convertedFile);

            Assert.IsFalse(result);
            Assert.IsNull(convertedFile);
        }

        [TestMethod]
        public void Convert_WhenCalledWithNullData_ReturnsError()
        {
            var fileWithInvalidData = new FileData(PlaceholderFileName, null);
            var result = _yamlToJsonConverter.Convert(fileWithInvalidData, out var convertedFile);

            Assert.IsFalse(result);
            Assert.IsNull(convertedFile);
        }

        [TestMethod]
        public void Convert_WhenCalledWithYamlData_ReturnsNoError()
        {
            var fileWithData = new FileData(PlaceholderFileName, string.Empty);
            var result = _yamlToJsonConverter.Convert(fileWithData, out var convertedFile);

            Assert.IsTrue(result);
            Assert.IsNotNull(convertedFile.Data);
            Assert.AreEqual(fileWithData.Name, convertedFile.Name);
        }
    }
}