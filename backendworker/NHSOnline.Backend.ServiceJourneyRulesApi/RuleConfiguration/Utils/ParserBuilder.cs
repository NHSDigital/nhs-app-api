using System.IO;
using YamlDotNet.Core;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils
{
    public class ParserBuilder : IParserBuilder<Parser>
    {
        public Parser GetParser<T>(T reader) where T : TextReader
        {
            return new Parser(reader);
        }
    }
}