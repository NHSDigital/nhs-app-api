using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NHSOnline.App.NhsLogin.Fido.Assertion;
using NHSOnline.App.NhsLogin.Fido.Models;
using NHSOnline.App.Threading;

namespace NHSOnline.App.NhsLogin.Fido
{
    internal sealed class FidoAuthorisationService
    {
        private const string AppId = "xarmain:nhs-app";

        private readonly ILogger<FidoAuthorisationService> _logger;
        private readonly IUafClient _uafClient;

        public FidoAuthorisationService(
            ILogger<FidoAuthorisationService> logger,
            IUafClient uafClient)
        {
            _logger = logger;
            _uafClient = uafClient;
        }

        public async Task<FidoAuthorisationResult> Authorise(IFidoKey fidoKey)
        {
            var authRequest = await GetAuthRequest().PreserveThreadContext();

            var header = authRequest.Header.DeepClone();
            header.AppId = AppId;

            var finalChallengeParams = GenerateFinalChallengeParams(authRequest.Challenge);

            var assertion = await GenerateAuthenticationAssertion(fidoKey, finalChallengeParams).PreserveThreadContext();

            var authResponse = new UafAuthenticationResponse
            {
                Header = header,
                FcParams = finalChallengeParams,
                Assertions =
                {
                    new UafAuthenticatorAuthenticationAssertion
                    {
                        Assertion = assertion
                    }
                }
            };

            var authResponseJson = JsonConvert.SerializeObject(new[] { authResponse });

            _logger.LogInformation("UAF Auth Response: {UAFAuthResponse}", authResponseJson);

            var fidoAuthResponse = Convert.ToBase64String(Encoding.UTF8.GetBytes(authResponseJson));
            return new FidoAuthorisationResult.Authorised(fidoAuthResponse);
        }

        private static string GenerateFinalChallengeParams(string challenge)
        {
            var uafFinalChallengeParams = new UafFinalChallengeParams
            {
                AppId = AppId,
                Challenge = challenge,
                FacetId = string.Empty
            };

            return Base64Url.Encode(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(uafFinalChallengeParams)));
        }

        private async Task<UafAuthenticationRequest> GetAuthRequest()
        {
            var result = await _uafClient.GetAuthorisationRequest().ResumeOnThreadPool();

            result.EnsureSuccessStatusCode();

            var resultString = await result.Content.ReadAsStringAsync().ResumeOnThreadPool();

            _logger.LogInformation("UAF Auth Request: {UAFAuthRequest}", resultString);

            var uafAuthenticationRequests = JsonConvert.DeserializeObject<UafAuthenticationRequest[]>(resultString);
            if (uafAuthenticationRequests is null)
            {
                _logger.LogError("Failed to deserialise auth request response: {Response}", resultString);
                throw new InvalidOperationException("Failed to deserialise auth request response");
            }

            return uafAuthenticationRequests[0];
        }

        private async Task<string> GenerateAuthenticationAssertion(IFidoKey key, string fcParams)
        {
            var builder = new AuthenticationAssertionBuilder(_logger)
                .FinalChallengeParams(fcParams)
                .KeyId(key.KeyId());

                builder = await builder.Sign(key.SignBytes).PreserveThreadContext();
                return await builder.Build().PreserveThreadContext();
        }
    }
}