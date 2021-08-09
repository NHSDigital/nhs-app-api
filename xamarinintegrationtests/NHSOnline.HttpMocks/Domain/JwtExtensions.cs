using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;

namespace NHSOnline.HttpMocks.Domain
{
    public static class JwtExtensions
    {
        public static bool TryGetCode(this JwtPayload payload, [NotNullWhen(true)] out string? code)
        {
            if (payload.TryGetValue("code", out var codeObject) && codeObject is string codeString)
            {
                code = codeString;
                return true;
            }

            code = null;
            return false;
        }
    }
}