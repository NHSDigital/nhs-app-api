using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Models;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils.Json;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.UnitTests.RuleConfiguration.Utils.Json
{
    [TestClass]
    public class SchemaValidatorTests
    {
        private const string ValidSchemaFileName = ".valid_schema.json";
        private const string ValidJson = @"{ ""target"": true }";
        private const string InvalidSchemaFileName = ".invalid_schema.json";
        private const string InvalidJson = @"{ ""target"": 1 }";
        private Mock<IErrorHandler> _mockErrorHandler;
        private ISchemaValidator _validator;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockErrorHandler = new Mock<IErrorHandler>();
            _validator = new SchemaValidator(_mockErrorHandler.Object);
        }

        [TestMethod]
        public async Task ValidateJsonAgainstSchema_WhenCalledWithInvalidSchema_ReturnsResultWithErrors()
        {
            var validationResult = await _validator.ValidateJsonAgainstSchema(
                GetEmbeddedResource(InvalidSchemaFileName),
                new FileData());

            _mockErrorHandler.Verify(errorHandler => errorHandler.LogError(It.IsAny<string>()), Times.Once);
            Assert.IsTrue(validationResult.IsErrors);
        }

        [TestMethod]
        public async Task ValidateJsonAgainstSchema_WillReuseExistingJsonSchema_IfAlreadyCached()
        {
            var validJsonSchema = GetEmbeddedResource(ValidSchemaFileName);
            var invalidJsonSchema = GetEmbeddedResource(InvalidSchemaFileName);
            // schema cached based on file name
            invalidJsonSchema.Name = validJsonSchema.Name;
            var validJson = new FileData
            {
                Name = "",
                Data = ValidJson
            };

            var firstValidationResult = await _validator.ValidateJsonAgainstSchema(validJsonSchema, validJson);
            var secondValidationResult = await _validator.ValidateJsonAgainstSchema(invalidJsonSchema, validJson);
            
            Assert.IsFalse(firstValidationResult.IsErrors);
            Assert.AreEqual(firstValidationResult.IsErrors, secondValidationResult.IsErrors);
            Assert.IsTrue(firstValidationResult.Errors.Count == 0);
            Assert.IsTrue(secondValidationResult.Errors.Count == 0);
        }

        [TestMethod]
        public async Task ValidateJsonAgainstSchema_WhenCalledWithValidSchemaInvalidJson_ReturnsResultWithErrors()
        {
            var validJsonSchema = GetEmbeddedResource(ValidSchemaFileName);
            var jsonFile = new FileData
            {
                Data = InvalidJson
            };

            var jsonValidationResult = await _validator.ValidateJsonAgainstSchema(validJsonSchema, jsonFile);

            Assert.IsTrue(jsonValidationResult.IsErrors);
            Assert.IsTrue(jsonValidationResult.Errors.Count > 0);
            Assert.IsNull(jsonValidationResult.Json);
        }

        [TestMethod]
        public async Task ValidateJsonAgainstSchema_WhenCalledWithValidSchemaValidJson_ReturnsResultWithoutErrors()
        {
            var validJsonSchema = GetEmbeddedResource(ValidSchemaFileName);
            var jsonFile = new FileData
            {
                Data = ValidJson
            };

            var jsonValidationResult = await _validator.ValidateJsonAgainstSchema(validJsonSchema, jsonFile);

            Assert.IsFalse(jsonValidationResult.IsErrors);
            Assert.AreEqual(jsonValidationResult.Errors.Count, 0);
            Assert.AreEqual(jsonFile.Data, jsonValidationResult.Json);
        }

        private static FileData GetEmbeddedResource(string fileName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith(fileName, StringComparison.Ordinal));
            
            using (var resourceStream = assembly.GetManifestResourceStream(resourceName))
            using (var reader = new StreamReader(resourceStream, Encoding.UTF8))
            {
                return new FileData
                {
                    Name = resourceName,
                    Data = reader.ReadToEnd()
                };
            }
        }
    }
}