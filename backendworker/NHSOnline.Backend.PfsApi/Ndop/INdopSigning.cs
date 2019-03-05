using Microsoft.IdentityModel.Tokens;

namespace NHSOnline.Backend.PfsApi.Ndop
{
    public interface INdopSigning
    {
        SigningCredentials GetSigningCredentials();
    }
}