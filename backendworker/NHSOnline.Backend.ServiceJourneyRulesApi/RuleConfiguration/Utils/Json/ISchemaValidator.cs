using System.Threading.Tasks;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Models;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils.Json
{
    public interface ISchemaValidator
    {
        Task<JsonValidationResult> ValidateJsonAgainstSchema(FileData schemaFile, FileData jsonFile);
    }
}