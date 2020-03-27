using System.IO;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils
{
    public interface ITextReaderBuilder<out T> where T : TextReader
    {
        T GetReader(string param);
    }
}