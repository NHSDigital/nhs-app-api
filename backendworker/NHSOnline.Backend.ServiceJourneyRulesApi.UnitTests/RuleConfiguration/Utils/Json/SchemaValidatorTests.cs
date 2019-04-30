using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Models;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils.Json;
using UnitTestHelper;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.UnitTests.RuleConfiguration.Utils.Json
{
    [TestClass]
    public class SchemaValidatorTests
    {
        private const string ValidSchemaFileName = ".valid_schema.json";
        private const string ValidJson = @"{ ""target"": true }";
        private const string InvalidSchemaFileName = ".invalid_schema.json";
        private const string InvalidJson = @"{ ""target"": 1 }";
        private Mock<ILogger<SchemaValidator>> _mockLogger;
        private ISchemaValidator _validator;

        [TestInitialize]
        public void TestInitialize()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
            
            _mockLogger = fixture.Freeze<Mock<ILogger<SchemaValidator>>>();
            
            _validator = fixture.Create<SchemaValidator>();
        }

        [TestMethod]
        public async Task ValidateJsonAgainstSchema_WhenCalledWithInvalidSchema_ReturnsFalse()
        {
            // act
            var result = await _validator.ValidateJsonAgainstSchema(
                GetEmbeddedResource(InvalidSchemaFileName),
                new FileData(null, null));
            
            // assert
            _mockLogger.VerifyLogger(LogLevel.Error, typeof(JsonReaderException), Times.Once());

            result.Should().BeFalse();
        }

        [TestMethod]
        public async Task ValidateJsonAgainstSchema_WhenCalledWithValidSchemaInvalidJson_ReturnsFalse()
        {
            // arrange
            var validJsonSchema = GetEmbeddedResource(ValidSchemaFileName);
            var jsonFile = new FileData(string.Empty, InvalidJson);

            // act
            var result = await _validator.ValidateJsonAgainstSchema(validJsonSchema, jsonFile);

            // assert
            _mockLogger.VerifyLogger(LogLevel.Error, Times.Once());

            result.Should().BeFalse();
        }

        [TestMethod]
        public async Task ValidateJsonAgainstSchema_WhenCalledWithValidSchemaValidJson_ReturnsTrue()
        {
            // arrange
            var validJsonSchema = GetEmbeddedResource(ValidSchemaFileName);
            var jsonFile = new FileData(string.Empty, ValidJson);

            // act
            var result = await _validator.ValidateJsonAgainstSchema(validJsonSchema, jsonFile);

            // assert
            result.Should().BeTrue();
        }

        private static FileData GetEmbeddedResource(string fileName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith(fileName, StringComparison.Ordinal));
            
            using (var resourceStream = assembly.GetManifestResourceStream(resourceName))
            using (var reader = new StreamReader(resourceStream, Encoding.UTF8))
            {
                return new FileData(resourceName, reader.ReadToEnd());
            }
        }
    }
}