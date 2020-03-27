using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils
{
    public sealed class YamlNodeDeserializer : INodeDeserializer
    {
        private readonly IDeserializer _deserializer;
        private readonly string _baseIncludePath;
        private readonly string _tag;
        private readonly IEnumerable<Type> _supportedTypes;
        private readonly ITextReaderBuilder<TextReader> _streamReaderBuilder;
        private readonly IParserBuilder<IParser> _parserBuilder;
        private readonly ILogger<YamlNodeDeserializer> _logger;

        public YamlNodeDeserializer(
            IDeserializer deserializer,
            string tag,
            string baseIncludePath,
            IEnumerable<Type> supportedTypes,
            ITextReaderBuilder<TextReader> streamReaderBuilder,
            IParserBuilder<IParser> parserBuilder,
            ILogger<YamlNodeDeserializer> logger)
        {
            _deserializer = deserializer;
            _tag = tag;
            _baseIncludePath = baseIncludePath;
            _supportedTypes = supportedTypes;
            _streamReaderBuilder = streamReaderBuilder;
            _parserBuilder = parserBuilder;
            _logger = logger;
        }

        public bool Deserialize(IParser reader, Type expectedType, Func<IParser, Type, object> nestedObjectDeserializer, out object value)
        {
            var scalar = reader.Peek<Scalar>();

            if (scalar == null || !_tag.Equals(scalar.Tag, StringComparison.Ordinal))
            {
                value = null;
                return false;
            }

            return DeserializeNode(reader, scalar, out value);
        }

        private bool DeserializeNode(IParser reader, Scalar scalar, out object value)
        {
            var typeName = Regex.Split(scalar.Value, "s/")[0];
            var filePath = Path.Join(_baseIncludePath, $"{scalar.Value}.yaml");

            try
            {
                var type = _supportedTypes.First(t => t.Name.Equals(typeName, StringComparison.Ordinal));

                using (var streamReader = _streamReaderBuilder.GetReader(filePath))
                {
                    var parser = _parserBuilder.GetParser(streamReader); 
                    value = _deserializer.Deserialize(parser, type);
                    reader.MoveNext();
                    return true;
                }
            }
            catch (FileNotFoundException e)
            {
                _logger.LogCritical(e, $"Error locating referenced file {filePath}.");
                throw;
            }
            catch (InvalidOperationException e)
            {
                _logger.LogCritical(e, $"Error retrieving type {typeName}.");
                _logger.LogInformation($"Ensure this model type is defined in the list of types provided" +
                                       " to the deserializer and is spelled correctly in the configuration files.");
                throw;
            }
        }
    }
}