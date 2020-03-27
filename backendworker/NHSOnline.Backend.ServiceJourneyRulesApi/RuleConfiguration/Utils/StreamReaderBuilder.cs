using System.IO;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils
{
    public class StreamReaderBuilder : ITextReaderBuilder<StreamReader>
    {
        public StreamReader GetReader(string param)
        {
            return new StreamReader(param);
        }
    }
}