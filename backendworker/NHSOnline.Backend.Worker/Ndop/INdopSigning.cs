using Microsoft.IdentityModel.Tokens;

namespace NHSOnline.Backend.Worker.Ndop
{
    public interface INdopSigning
    {
        SigningCredentials GetSigningCredentials();
    }
}