using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Models;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils.Converters
{
    internal interface IYamlToJsonConverter
    {
        bool Convert(FileData yamlFile, out FileData jsonFile);
    }
}