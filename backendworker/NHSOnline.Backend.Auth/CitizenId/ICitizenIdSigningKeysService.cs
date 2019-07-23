using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.Auth.CitizenId
{
    public interface ICitizenIdSigningKeysService
    {
        Task<Option<JsonWebKeySet>> GetSigningKeys();
    }
}