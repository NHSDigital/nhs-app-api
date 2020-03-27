using System.IO;
using YamlDotNet.Core;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils
{
    public interface IParserBuilder<out T> where T : IParser
    {
        T GetParser<TReader>(TReader reader) where TReader : TextReader;
    }
}