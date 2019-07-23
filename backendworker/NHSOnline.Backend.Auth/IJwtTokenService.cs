using Microsoft.IdentityModel.Tokens;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.Auth
{
    public interface IJwtTokenService<T>
    {
        Option<T> ReadToken(string token,JsonWebKeySet signingKeys);
    }
}
