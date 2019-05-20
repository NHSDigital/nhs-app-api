using System.IO;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils
{
    internal interface IYamlSerializer
    {
        void Serialize<TModel>(TextWriter writer, TModel model)
            where TModel : class, new();
    }
}