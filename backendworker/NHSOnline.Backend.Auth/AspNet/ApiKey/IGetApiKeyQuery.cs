using System.Threading.Tasks;

namespace NHSOnline.Backend.Auth.AspNet.ApiKey
{
    public interface IGetApiKeyQuery
    {
        Task<SecureApiKey> Execute(string providedApiKey);
    }
}