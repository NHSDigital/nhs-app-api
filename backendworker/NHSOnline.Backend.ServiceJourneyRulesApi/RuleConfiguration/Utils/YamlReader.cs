using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Models;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils.Converters;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils.Json;
using YamlDotNet.Serialization;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils
{
    internal class YamlReader<TModel> : IYamlReader<TModel>
        where TModel: class, new()
    {

        private readonly IDeserializer _deserializer;
        private readonly ILogger _logger;
        private readonly string _filePath;
        private readonly FileData _schemaData;
        private readonly IYamlToJsonConverter _yamlToJsonConverter;
        private readonly ISchemaValidator _schemaValidator;
        private readonly IFileHandler _fileHandler;

        private bool _isInitiliased;
        private TModel _model;
        
        public YamlReader(
            string filePath,
            FileData schemaData,
            IDeserializer deserializer,
            ILogger logger,
            IYamlToJsonConverter yamlToJsonConverter,
            ISchemaValidator schemaValidator,
            IFileHandler fileHandler)
        {
            _deserializer = deserializer;
            _logger = logger;
            _filePath = filePath;
            _schemaData = schemaData;
            _yamlToJsonConverter = yamlToJsonConverter;
            _schemaValidator = schemaValidator;
            _fileHandler = fileHandler;
        }

        public async Task<TModel> GetData()
        {
            if (_isInitiliased || _schemaData == null)
            {
                return _model;
            }

            _isInitiliased = true;
            var rawData = ReadToEnd();

            if (!await ValidateSchema(rawData))
            {
                return _model;
            }

            try
            {
                _model = _deserializer.Deserialize<TModel>(rawData);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error mapping {_filePath} content to {typeof(TModel)}");
            }

            return _model;
        }

        private async Task<bool> ValidateSchema(string rawData)
        {
            return _yamlToJsonConverter.Convert(new FileData(_filePath, rawData), out var jsonData)
                && await _schemaValidator.ValidateJsonAgainstSchema(_schemaData, jsonData);
        }
        
        private string ReadToEnd()
        {
            using (var stream = _fileHandler.GetTextReaderToReadFileContent(_filePath))
            {
                return stream.ReadToEnd();
            }
        }
    }
}