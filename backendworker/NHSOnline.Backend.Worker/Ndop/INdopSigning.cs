using System.Security.Cryptography.X509Certificates;
using Microsoft.IdentityModel.Tokens;

namespace NHSOnline.Backend.Worker.Ndop
{
    public interface INdopSigning
    {
        SigningCredentials GetSigningCredentials();
    }
}