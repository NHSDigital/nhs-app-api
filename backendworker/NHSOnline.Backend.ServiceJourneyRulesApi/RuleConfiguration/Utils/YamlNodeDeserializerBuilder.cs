using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

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
    }
}