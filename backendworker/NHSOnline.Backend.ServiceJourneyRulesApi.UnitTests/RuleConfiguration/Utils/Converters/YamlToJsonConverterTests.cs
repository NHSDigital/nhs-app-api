using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Models;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils.Converters;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.UnitTests.RuleConfiguration.Utils.Converters
{
    [TestClass]
    public class YamlToJsonConverterTests
    {
        private const string PlaceholderFileName = "test_file.yaml";
        private Mock<IErrorHandler> _mockErrorHandler;
        private readonly IDeserializer _deserializer = new DeserializerBuilder().WithNamingConvention(new CamelCaseNamingConvention()).Build();
        private readonly ISerializer _serializer = new SerializerBuilder().JsonCompatible().Build();
        private IYamlToJsonConverter _yamlToJsonConverter;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockErrorHandler = new Mock<IErrorHandler>();
            _yamlToJsonConverter = new YamlToJsonConverter(_mockErrorHandler.Object, _deserializer, _serializer);
        }

        [TestMethod]
        public void Convert_WhenCalledWithNullFileData_ReturnsNull()
        {
            var convertedFile = _yamlToJsonConverter.Convert(null);

            Assert.IsNull(convertedFile);
        }

        [TestMethod]
        public void Convert_WhenCalledWithNullData_ReturnsError()
        {
            var fileWithInvalidData = new FileData
            {
                Name = PlaceholderFileName,
                Data = null
            };
            var convertedFile = _yamlToJsonConverter.Convert(fileWithInvalidData);

            Assert.IsTrue(convertedFile.IsError);
            Assert.IsNotNull(convertedFile.Error);
            Assert.IsNull(convertedFile.Data);
        }

        [TestMethod]
        public void Convert_WhenCalledWithYamlData_ReturnsNoError()
        {
            var fileWithData = new FileData
            {
                Name = PlaceholderFileName,
                Data = ""
            };
            var convertedFile = _yamlToJsonConverter.Convert(fileWithData);

            Assert.IsFalse(convertedFile.IsError);
            Assert.IsNull(convertedFile.Error);
            Assert.IsNotNull(convertedFile.Data);
            Assert.AreEqual(fileWithData.Name, convertedFile.Name);
        }

        [TestMethod]
        public void ConvertAll_WhenCalledWithNullList_ReturnsEmptyList()
        {
            var convertedFiles = _yamlToJsonConverter.ConvertAll(null).ToList();

            Assert.IsNotNull(convertedFiles);
            Assert.AreEqual(0, convertedFiles.Count);
        }

        [TestMethod]
        public void ConvertAll_WhenCalledWithListOfValidFileData_ReturnsListOfConvertedFileData()
        {
            var fileWithValidData = new FileData
            {
                Name = PlaceholderFileName,
                Data = ""
            };
            var fileWithInvalidData = new FileData
            {
                Name = PlaceholderFileName,
                Data = null
            };

            var filesToConvert = new List<FileData>{ fileWithValidData, fileWithInvalidData, fileWithValidData };

            var convertedFiles = _yamlToJsonConverter.ConvertAll(filesToConvert).ToList();

            Assert.IsNotNull(convertedFiles);
            Assert.AreEqual(filesToConvert.Count, convertedFiles.Count);
            Assert.IsFalse(convertedFiles.ElementAt(0).IsError);
            Assert.IsTrue(convertedFiles.ElementAt(1).IsError);
            Assert.IsFalse(convertedFiles.ElementAt(2).IsError);
        }
    }
}