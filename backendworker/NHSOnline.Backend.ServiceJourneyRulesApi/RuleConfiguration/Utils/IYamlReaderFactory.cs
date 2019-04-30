using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Models;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils
{
    internal interface IYamlReaderFactory
    {
        IYamlReader<TModel> GetReader<TModel>(string filename, FileData schema) where TModel: class, new();
    }
}