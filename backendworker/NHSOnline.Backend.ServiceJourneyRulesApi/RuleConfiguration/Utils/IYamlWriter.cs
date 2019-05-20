namespace NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils
{
    internal interface IYamlWriter
    {
        void Write<TModel>(string filePath, TModel model) where TModel : class, new();
    }
}