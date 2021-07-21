using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NHSOnline.App.Config;

namespace NHSOnline.App.NhsLogin
{
    internal sealed class NhsLoginService: INhsLoginService
    {
        private readonly ILogger _logger;
        private readonly INhsLoginConfiguration _loginConfig;
        private readonly INhsAppWebConfiguration _webConfig;

        public NhsLoginService(
            ILogger<NhsLoginService> logger,
            INhsLoginConfiguration loginConfig, INhsAppWebConfiguration webConfig)
        {
            _logger = logger;
            _loginConfig = loginConfig;
            _webConfig = webConfig;
        }

        public ProofKeyCodeExchangeCodes GeneratePkceCodes()
        {
            using var randomNumberGenerator = RandomNumberGenerator.Create();

            var randomSecret = new byte[32];
            randomNumberGenerator.GetBytes(randomSecret);

            var verifier = Base64UrlEncoder.Encode(randomSecret);

            using SHA256 hasher = SHA256.Create();
            var verifierHash = hasher.ComputeHash(Encoding.UTF8.GetBytes(verifier));

            var challenge = Base64UrlEncoder.Encode(verifierHash);

            return new ProofKeyCodeExchangeCodes(verifier, challenge, "S256");
        }

        public LoginState BeginLogin(ProofKeyCodeExchangeCodes codes, string? fidoAuthResponse)
        {
            var authReturnUri = new UriBuilder
            {
                Scheme = "nhsapp",
                Host = _webConfig.Host,
                Path = "/auth-return"
            }.Uri;

            var authoriseUri = NhsLoginUriBuilder.Create(_loginConfig)
                .Challenge(codes.Challenge, codes.Method)
                .ClientId("nhs-online")
#if GP_DECOUPLED
                .Scopes("openid", "profile", "profile_extended", "gp_registration_details")
#else
                .Scopes("openid", "profile", "profile_extended", "nhs_app_credentials", "gp_integration_credentials")
#endif
                .VectorsOfTrust("P5.Cp.Cd", "P5.Cp.Ck", "P5.Cm", "P9.Cp.Cd", "P9.Cp.Ck", "P9.Cm")
                .RedirectUri(authReturnUri)
                .FidoAuthResponse(fidoAuthResponse)
                .Uri;

            return new LoginState(_logger, authoriseUri, authReturnUri);
        }
    }
}
