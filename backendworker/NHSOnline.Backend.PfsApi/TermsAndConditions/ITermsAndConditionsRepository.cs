using System.Threading.Tasks;
using NHSOnline.Backend.Repository;

namespace NHSOnline.Backend.PfsApi.TermsAndConditions
{
    public interface ITermsAndConditionsRepository
    {
        Task<RepositoryCreateResult<TermsAndConditionsRecord>> Create(TermsAndConditionsRecord record);
        Task<RepositoryUpdateResult<TermsAndConditionsRecord>> Update(string nhsLoginId, UpdateRecordBuilder<TermsAndConditionsRecord> updates);
        Task<RepositoryFindResult<TermsAndConditionsRecord>> Find(string nhsLoginId);
    }
}