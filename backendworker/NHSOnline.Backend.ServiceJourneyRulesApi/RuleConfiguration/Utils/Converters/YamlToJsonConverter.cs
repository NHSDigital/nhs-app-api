using System;
using System.Collections.Generic;
using System.IO;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Models;
using YamlDotNet.Serialization;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils.Converters
{
    public class YamlToJsonConverter : IYamlToJsonConverter
    {
        private readonly IErrorHandler _errorHandler;
        private readonly IDeserializer _deserializer;
        private readonly ISerializer _serializer;

        public YamlToJsonConverter(IErrorHandler errorHandler, IDeserializer deserializer, ISerializer serializer)
        {
            _errorHandler = errorHandler;
            _deserializer = deserializer;
            _serializer = serializer;
        }

        public FileData Convert(FileData yamlFile)
        {
            if (yamlFile == null)
            {
                return null;
            }

            var result = new FileData
            {
                Name = yamlFile.Name
            };

            try
            {
                _errorHandler.LogInformation($"Deserializing {yamlFile.Name}");

                using (var sr = new StringReader(yamlFile.Data))
                {
                    var yaml = _deserializer.Deserialize(sr);

                    using (var json = new StringWriter())
                    {
                        _errorHandler.LogInformation($"Successfully deserialized {yamlFile.Name}");
                        _serializer.Serialize(json, yaml);
                        result.Data = json.ToString();
                    }
                }
            }
            catch (Exception e)
            {
                _errorHandler.LogError($"Error deserializing {yamlFile.Name}");

                result.IsError = true;
                result.Error = e.Message;
            }

            return result;
        }

        public IEnumerable<FileData> ConvertAll(IEnumerable<FileData> yamlFiles)
        {
            var result = new List<FileData>();

            if (yamlFiles == null)
            {
                return result;
            }

            foreach (var yamlFile in yamlFiles)
            {
                result.Add(Convert(yamlFile));
            }

            return result;
        }
    }
}