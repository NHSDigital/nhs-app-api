using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Models;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils.Converters;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils.Json;
using YamlDotNet.Serialization;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils
{
    internal class YamlReaderFactory : IYamlReaderFactory
    {
        private readonly IDeserializer _deserializer;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IYamlToJsonConverter _yamlToJsonConverter;
        private readonly ISchemaValidator _schemaValidator;
        private readonly IFileHandler _fileHandler;
        
        private Dictionary<string, object> ReaderCache { get; } = new Dictionary<string, object>();

        public YamlReaderFactory(
            IDeserializer deserializer,
            ILoggerFactory loggerFactory,
            IYamlToJsonConverter yamlToJsonConverter,
            ISchemaValidator schemaValidator,
            IFileHandler fileHandler)
        {
            _deserializer = deserializer;
            _loggerFactory = loggerFactory;
            _yamlToJsonConverter = yamlToJsonConverter;
            _schemaValidator = schemaValidator;
            _fileHandler = fileHandler;
        }
        
        public IYamlReader<TModel> GetReader<TModel>(string filename, FileData schema)
            where TModel: class, new()
        {
            if (ReaderCache.TryGetValue(filename, out var reader) && reader is IYamlReader<TModel> yamlReader)
            {
                return yamlReader;
            }

            yamlReader = new YamlReader<TModel>(
                filename, 
                schema, 
                _deserializer, 
                _loggerFactory.CreateLogger<YamlReader<TModel>>(),
                _yamlToJsonConverter,
                _schemaValidator,
                _fileHandler);
            
            ReaderCache[filename] = yamlReader;

            return yamlReader;
        }
    }
}