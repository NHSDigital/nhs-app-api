using System.Threading.Tasks;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.Auth
{
    public interface IJwtTokenService<T>
    {
        Task<Option<IdToken>> ReadToken(string token);
    }
}
