using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Models;
using NJsonSchema;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils.Json
{
    public class SchemaValidator : ISchemaValidator
    {
        private readonly IErrorHandler _errorHandler;
        private readonly Dictionary<string, JsonSchema4> _schemas;

        public SchemaValidator(IErrorHandler errorHandler)
        {
            _errorHandler = errorHandler;
            _schemas = new Dictionary<string, JsonSchema4>();
        }

        private async Task<JsonSchema4> GetJsonSchema4(FileData schemaFile)
        {
            try
            {
                if (_schemas.ContainsKey(schemaFile.Name))
                {
                    _errorHandler.LogInformation($"Getting JSON schema from cache for {schemaFile.Name}");
                    return _schemas[schemaFile.Name];
                }

                var schema = await JsonSchema4.FromJsonAsync(schemaFile.Data);
                _schemas.Add(schemaFile.Name, schema);
                
                return schema;
            }
            catch (Exception e)
            {
                _errorHandler.LogError($"Error creating JSON schema from file {schemaFile.Name}. {e.Message}");
                return null;
            }
        }

        public async Task<JsonValidationResult> ValidateJsonAgainstSchema(FileData schemaFile, FileData jsonFile)
        {
            var result = new JsonValidationResult();

            var schema = await GetJsonSchema4(schemaFile);

            if (schema == null)
            {
                result.IsErrors = true;
                result.Errors.Add($"Validation unsuccessful for {jsonFile.Name}. Unable to create schema from {schemaFile.Name}.");
                return result;
            }

            try
            {
                _errorHandler.LogInformation($"Validating JSON from file {jsonFile.Name}");
                var errors = schema.Validate(jsonFile.Data);

                if (!errors.Any())
                {
                    _errorHandler.LogInformation($"Validation successful for {jsonFile.Name}");
                    result.Json = jsonFile.Data;
                }
                else
                {
                    _errorHandler.LogInformation($"Validation unsuccessful for {jsonFile.Name}");
                    result.IsErrors = true;
                    result.Errors = (from error in errors
                        select GetErrorText(jsonFile.Name, error.ToString())).ToList();
                }
            }
            catch (Exception e)
            {
                _errorHandler.LogInformation($"Validation unsuccessful for {jsonFile.Name}");
                result.IsErrors = true;
                result.Errors.Add(GetErrorText(jsonFile.Name, e.Message));
            }

            return result;
        }

        private static string GetErrorText(string jsonFileName, string error)
        {
            return $"{jsonFileName} | {error}";
        }
    }
}