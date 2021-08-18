using System.Threading.Tasks;

namespace Nhs.App.Api.Integration.Tests.Services.AccessTokenService
{
    public interface IAccessTokenCacheService
    {
        Task<string> FetchToken();
    }
}
