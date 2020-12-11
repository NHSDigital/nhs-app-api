using System.Threading.Tasks;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Im1Connection.Cache
{
    public interface IIm1CacheService
    {
        string CacheKeyPropertyName { get; }

        Task SaveIm1ConnectionToken<T>(string key, T value);
        Task<Option<T>> GetIm1ConnectionToken<T>(string key);
        Task<bool> DeleteIm1ConnectionToken(string key);
    }
}