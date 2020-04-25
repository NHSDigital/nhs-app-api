using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Models;
using NJsonSchema;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils.Json
{
    internal class SchemaValidator : ISchemaValidator
    {
        private readonly ILogger _logger;
        private Dictionary<string, JsonSchema> Schemas { get; } = new Dictionary<string, JsonSchema>();

        public SchemaValidator(ILogger<SchemaValidator> logger)
        {
            _logger = logger;
        }

        private async Task<JsonSchema> GetJsonSchema(FileData schemaFile)
        {
            try
            {
                if (Schemas.TryGetValue(schemaFile.Name, out var schema))
                {
                    _logger.LogDebug($"Getting JSON schema from cache for {schemaFile.Name}");
                    return schema;
                }

                schema = await JsonSchema.FromJsonAsync(schemaFile.Data);
                Schemas.Add(schemaFile.Name, schema);
                
                return schema;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error creating JSON schema from file {schemaFile.Name}");
                return null;
            }
        }

        public async Task<bool> ValidateJsonAgainstSchema(FileData schemaFile, FileData jsonFile)
        {
            var schema = await GetJsonSchema(schemaFile);
            
            if (schema == null)
            {
                _logger.LogError($"Validation unsuccessful for {jsonFile.Name}. Unable to create schema from {schemaFile.Name}.");
                return false;
            }

            try
            {
                _logger.LogDebug($"Validating JSON from file {jsonFile.Name}");
                var errors = schema.Validate(jsonFile.Data);

                if (!errors.Any())
                {
                    _logger.LogDebug($"Validation successful for {jsonFile.Name}");
                    return true;
                }
                
                _logger.LogWarning($"Validation unsuccessful for {jsonFile.Name}");
                foreach (var error in errors)
                {
                    _logger.LogError(error.ToString());
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Validation unsuccessful for {jsonFile.Name}");
            }

            return false;
        }
    }
}