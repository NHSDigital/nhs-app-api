using System;
using System.IO;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Models;
using YamlDotNet.Serialization;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils.Converters
{
    internal class YamlToJsonConverter : IYamlToJsonConverter
    {
        private readonly ILogger _logger;
        private readonly IDeserializer _deserializer;
        private readonly ISerializer _serializer;

        public YamlToJsonConverter(
            ILogger<YamlToJsonConverter> logger,
            IDeserializer deserializer,
            ISerializer serializer)
        {
            _logger = logger;
            _deserializer = deserializer;
            _serializer = serializer;
        }

        public bool Convert(FileData yamlFile, out FileData jsonFile)
        {
            if (yamlFile != null)
            {
                try
                {
                    _logger.LogDebug($"Deserializing {yamlFile.Name}");

                    using (var sr = new StringReader(yamlFile.Data))
                    {
                        var yaml = _deserializer.Deserialize(sr);

                        using (var json = new StringWriter())
                        {
                            _logger.LogDebug($"Successfully deserialized {yamlFile.Name}");
                            _serializer.Serialize(json, yaml);

                            jsonFile = new FileData(yamlFile.Name, json.ToString());
                            return true;
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Error deserializing {yamlFile.Name}");
                }
            }

            jsonFile = null;
            return false;
        }
    }
}