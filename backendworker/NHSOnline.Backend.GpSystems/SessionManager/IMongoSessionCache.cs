using System.Threading.Tasks;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.SessionManager
{
    internal interface IMongoSessionCache
    {
        Task Create(string sessionId, string encodedUserSession);
        Task<Option<string>> Get(string sessionId);
        Task<bool> Delete(string sessionId);
        Task Update(string key, string encodedUserSession);
        Task<Option<string>> GetAndUpdate(string sessionId);
    }
}
