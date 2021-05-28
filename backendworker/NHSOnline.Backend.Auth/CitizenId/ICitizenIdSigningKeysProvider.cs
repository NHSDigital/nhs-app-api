using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.Auth.CitizenId
{
    public interface ICitizenIdSigningKeysProvider
    {
        Task<Option<JsonWebKeySet>> GetSigningKeys(string keyId);
    }
}
