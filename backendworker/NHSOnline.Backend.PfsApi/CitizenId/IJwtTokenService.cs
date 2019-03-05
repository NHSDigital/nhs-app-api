using Microsoft.IdentityModel.Tokens;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.CitizenId
{
    public interface IJwtTokenService<T>
    {
        Option<T> ReadToken(string token,JsonWebKeySet signingKeys);
    }
}
