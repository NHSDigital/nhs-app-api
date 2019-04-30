using System.Threading.Tasks;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils
{
    internal interface IYamlReader<TModel>
        where TModel: class, new()
    {
        Task<TModel> GetData();
    }
}