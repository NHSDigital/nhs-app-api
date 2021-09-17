using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NHSOnline.App.Config;

namespace NHSOnline.App.NhsLogin
{
    internal sealed class NhsLoginService : INhsLoginService
    {
        private readonly ILogger _logger;
        private readonly INhsLoginConfiguration _loginConfig;
        private readonly INhsAppWebConfiguration _webConfig;

        public NhsLoginService(
            ILogger<NhsLoginService> logger,
            INhsLoginConfiguration loginConfig,
            INhsAppWebConfiguration webConfig)
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
            var authReturnUri = BuildLoginAuthReturnUri();

            var authoriseUri = NhsLoginUriBuilder.Create(_loginConfig)
                .ClientId("nhs-online")
                .Scopes(NhsLoginScope.P5NoGpSession)
                .VectorsOfTrust(NhsLoginVectorsOfTrust.P5BasicAndP9Sensitive)
                .RedirectUri(authReturnUri)
                .Challenge(codes.Challenge, codes.Method)
                .FidoAuthResponse(fidoAuthResponse)
                .Build();

            return new LoginState(_logger, authoriseUri, authReturnUri);
        }

        public CreateOnDemandGpSessionState CreateOnDemandGpSession(string assertedLoginIdentity, string redirectTo)
        {
            var authReturnUri = new UriBuilder
            {
                Scheme = _webConfig.Scheme,
                Host = _webConfig.Host,
                Path = "/on-demand-gp-return"
            }.Uri;

            var authoriseUri = NhsLoginUriBuilder.Create(_loginConfig)
                .ClientId("nhs-online")
                .Scopes(NhsLoginScope.P9WithGpSession)
                .VectorsOfTrust(NhsLoginVectorsOfTrust.P5BasicAndP9Sensitive)
                .RedirectUri(authReturnUri)
                .State(redirectTo)
                .AssertedLoginIdentity(assertedLoginIdentity)
                .Build();

            return new CreateOnDemandGpSessionState(authoriseUri, authReturnUri);
        }

        public LoginState CreateNhsLoginUpliftSession(ProofKeyCodeExchangeCodes codes, string assertedLoginIdentity)
        {
            var authReturnUri = BuildLoginAuthReturnUri();

            var authoriseUri = NhsLoginUriBuilder.Create(_loginConfig)
                .ClientId("nhs-online")
                .Scopes(NhsLoginScope.P5ToP9Uplift)
                .VectorsOfTrust(NhsLoginVectorsOfTrust.P9Sensitive)
                .RedirectUri(authReturnUri)
                .Challenge(codes.Challenge, codes.Method)
                .AssertedLoginIdentity(assertedLoginIdentity)
                .Build();

            return new LoginState(_logger, authoriseUri, authReturnUri);
        }

        private Uri BuildLoginAuthReturnUri()
        {
            return new UriBuilder
            {
                Scheme = _webConfig.Scheme,
                Host = _webConfig.Host,
                Path = "/auth-return"
            }.Uri;
        }
    }
}