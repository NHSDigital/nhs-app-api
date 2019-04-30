using System.Threading.Tasks;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Models;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils.Json
{
    internal interface ISchemaValidator
    {
        Task<bool> ValidateJsonAgainstSchema(FileData schemaFile, FileData jsonFile);
    }
}