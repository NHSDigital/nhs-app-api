using System.Threading.Tasks;

namespace NHSOnline.Backend.PfsApi.ServiceJourneyRules
{
    public interface IServiceJourneyRulesService
    {
        Task<bool> IsJourneyEnabled(string odsCode);
    }
}