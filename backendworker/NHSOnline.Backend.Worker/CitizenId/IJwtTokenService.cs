using Microsoft.IdentityModel.Tokens;
using NHSOnline.Backend.Worker.Support;

namespace NHSOnline.Backend.Worker.CitizenId
{
    public interface IJwtTokenService<T>
    {
        Option<T> ReadToken(string token,JsonWebKeySet signingKeys);
    }
}
