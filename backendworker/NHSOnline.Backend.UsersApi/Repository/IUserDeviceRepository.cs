using System.Threading.Tasks;
using NHSOnline.Backend.Repository;

namespace NHSOnline.Backend.UsersApi.Repository
{
    public interface IUserDeviceRepository
    {
        Task<RepositoryCreateResult<UserDevice>> Create(UserDevice userDevice);
        Task<RepositoryFindResult<UserDevice>> Find(string nhsLoginId, string deviceId);
        Task<RepositoryFindResult<UserDevice>> FindRegistrations(int maxRecords);
        Task<RepositoryDeleteResult<UserDevice>> Delete(string nhsLoginId, string deviceId);
        Task<RepositoryUpdateResult<UserDevice>> UpdateOne(string nhsLoginId, string deviceId,
            UpdateRecordBuilder<UserDevice> updates);
    }
}