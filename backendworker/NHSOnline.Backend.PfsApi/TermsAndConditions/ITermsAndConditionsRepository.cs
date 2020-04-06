using System.Threading.Tasks;

namespace NHSOnline.Backend.PfsApi.TermsAndConditions
{
    public interface ITermsAndConditionsRepository
    {
        Task Create(TermsAndConditionsRecord record);
        Task Update(TermsAndConditionsRecord record);
        Task<TermsAndConditionsRecord> Find(string nhsLoginId);
    }
}