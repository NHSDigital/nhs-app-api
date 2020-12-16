using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils
{
    internal class YamlSerializer : IYamlSerializer
    {
        private readonly ISerializer _serializer;

        public YamlSerializer()
        {
            _serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull)
                .Build();
        }

        public void Serialize<TModel>(TextWriter writer, TModel model)
            where TModel : class, new()
            => _serializer.Serialize(writer, model);

    }
}