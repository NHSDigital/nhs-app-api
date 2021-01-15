using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace NHSOnline.Backend.Support.Certificate
{
    public interface ISigning
    {
        SigningCredentials GetSigningCredentials(string certPrefix);
        RSAParameters GetRsaParameters(IKeyConfig keyConfig);
    }
}