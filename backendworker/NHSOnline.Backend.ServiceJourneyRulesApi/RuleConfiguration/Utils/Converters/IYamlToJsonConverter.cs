using System.Collections.Generic;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Models;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils.Converters
{
    public interface IYamlToJsonConverter
    {
        FileData Convert(FileData yamlFile);
        IEnumerable<FileData> ConvertAll(IEnumerable<FileData> yamlFiles);
    }
}