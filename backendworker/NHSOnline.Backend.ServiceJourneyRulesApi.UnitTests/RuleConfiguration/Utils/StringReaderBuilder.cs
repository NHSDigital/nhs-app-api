using System.IO;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.UnitTests.RuleConfiguration.Utils
{
    public class StringReaderBuilder : ITextReaderBuilder<StringReader>
    {
        public StringReader GetReader(string param)
        {
            return new StringReader(param);
        }
    }
}