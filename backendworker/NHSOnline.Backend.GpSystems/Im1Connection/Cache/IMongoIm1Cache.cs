using System.Threading.Tasks;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Im1Connection.Cache
{
    internal interface IMongoIm1Cache
    {
        Task Save(string key, string token);
        Task<Option<string>> Get(string key);
        Task<bool> Delete(string key);
    }
}