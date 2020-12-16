using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NodeDeserializers;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils
{
    public class YamlNodeDeserializerBuilder
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly DeserializerBuilder _deserializerBuilder;

        private string _tag;
        private IEnumerable<Type> _supportedTypes;

        public YamlNodeDeserializerBuilder(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            _deserializerBuilder = new DeserializerBuilder();
        }

        public YamlNodeDeserializerBuilder WithTag(string tag)
        {
            _tag = tag;
            return this;
        }

        public YamlNodeDeserializerBuilder WithNamingConvention(INamingConvention namingConvention)
        {
            _deserializerBuilder.WithNamingConvention(namingConvention);
            return this;
        }

        public YamlNodeDeserializerBuilder WithSupportedTypes(IEnumerable<Type> supportedTypes)
        {
            _supportedTypes = supportedTypes;
            return this;
        }
        
        public IDeserializer Build()
        {
            return _deserializerBuilder
                .WithTagMapping(_tag, typeof(object))
                .WithNodeDeserializer(new NullableEnumScalarNodeDeserializer(), x => x.InsteadOf<ScalarNodeDeserializer>())
                .WithNodeDeserializer(
                    new YamlNodeDeserializer(
                        _deserializerBuilder.Build(),
                        _tag,
                        _serviceProvider.GetService<IServiceJourneyRulesConfiguration>().GlobalsFolderPath,
                        _supportedTypes,
                        _serviceProvider.GetService<ITextReaderBuilder<TextReader>>(),
                        _serviceProvider.GetService<IParserBuilder<Parser>>(),
                        _serviceProvider.GetService<ILogger<YamlNodeDeserializer>>()),
                    x => x.OnTop())
                .Build();
        }

        private sealed class NullableEnumScalarNodeDeserializer : INodeDeserializer
        {
            private readonly INodeDeserializer _scalarNodeDeserializer = new ScalarNodeDeserializer();

            public bool Deserialize(IParser parser, Type expectedType, Func<IParser, Type, object> nestedObjectDeserializer, out object value)
            {
                // Accept looks at the current value but does not consume it so it is still available for the ScalarNodeDeserializer
                if (!parser.Accept<Scalar>(out var scalar))
                {
                    value = null;
                    return false;
                }

                // YamlDotNet 9.1.1 does not correctly handle deserialization of nullable enums
                // https://github.com/aaubry/YamlDotNet/issues/544
                var underlyingType = Nullable.GetUnderlyingType(expectedType) ?? expectedType;
                if (underlyingType.IsEnum)
                {
                    // Consume the current token
                    parser.MoveNext();

                    value = Enum.Parse(underlyingType, scalar.Value, true);
                    return true;
                }

                return _scalarNodeDeserializer.Deserialize(parser, expectedType, nestedObjectDeserializer, out value);
            }
        }
    }
}