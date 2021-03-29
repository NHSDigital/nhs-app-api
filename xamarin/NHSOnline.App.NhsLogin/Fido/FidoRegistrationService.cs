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
    internal sealed class FidoRegistrationService
    {
        private const string AppId = "xarmain:nhs-app";

        private readonly ILogger _logger;
        private readonly IUafClient _uafClient;

        public FidoRegistrationService(
            ILogger<FidoService> logger,
            IUafClient uafClient)
        {
            _logger = logger;
            _uafClient = uafClient;
        }

        internal async Task<FidoRegisterResult> Register(IFidoKey key, string accessToken)
        {
            var registrationRequest = await GetRegistrationRequest(accessToken).ResumeOnThreadPool();

            var registrationResponse = await CreateAndSignRegistrationResponse(registrationRequest, key).ResumeOnThreadPool();

            return await PostRegistrationResponse(registrationResponse, accessToken).ResumeOnThreadPool();
        }

        internal async Task Deregister(string accessToken, string keyId)
        {
            var deregistrationRequest = CreateDeregistrationRequest(keyId);

            await PostDeregistrationRequest(accessToken, deregistrationRequest).ResumeOnThreadPool();
        }

        private async Task<UafRegistrationRequest> GetRegistrationRequest(string accessToken)
        {
            var result = await _uafClient.GetRegistrationRequest(accessToken).ResumeOnThreadPool();
            var resultString = await result.Content.ReadAsStringAsync().ResumeOnThreadPool();

            _logger.LogInformation("UAF Registration Request: {UAFRegistrationRequest}", resultString);

            var uafRegistrationRequests = JsonConvert.DeserializeObject<UafRegistrationRequest[]>(resultString);
            if (uafRegistrationRequests is null)
            {
                _logger.LogError("Failed to deserialise reg request response: {Response}", resultString);
                throw new InvalidOperationException("Failed to deserialise reg request response");
            }
            return uafRegistrationRequests[0];
        }

        private async Task<UafRegistrationResponse> CreateAndSignRegistrationResponse(
            UafRegistrationRequest registrationRequest,
            IFidoKey key)
        {
            var header = registrationRequest.Header.DeepClone();
            header.AppId = AppId;

            var finalChallengeParams = GenerateFinalChallengeParams(registrationRequest.Challenge);
            var assertion = await GenerateRegistrationAssertion(key, finalChallengeParams).ResumeOnThreadPool();

            var registrationResponse = new UafRegistrationResponse
            {
                Header = header,
                FcParams = finalChallengeParams,
                Assertions =
                {
                    new UafAuthenticatorRegistrationAssertion
                    {
                        Assertion = assertion
                    }
                }
            };
            return registrationResponse;
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

        private async Task<string> GenerateRegistrationAssertion(IFidoKey key, string fcParams)
        {
            var builder = new RegistrationAssertionBuilder(_logger)
                .FinalChallengeParams(fcParams)
                .KeyId(key.KeyId())
                .PublicKeyEccX962Raw(key.PublicKeyEccX962Raw());

            builder = await builder.Sign(key.SignBytes).ResumeOnThreadPool();

            return await builder.Build().ResumeOnThreadPool();
        }

        private async Task<FidoRegisterResult> PostRegistrationResponse(UafRegistrationResponse registrationResponse, string accessToken)
        {
            var result = await _uafClient.PostRegistrationResponse(registrationResponse, accessToken).ResumeOnThreadPool();
            var resultString = await result.Content.ReadAsStringAsync().ResumeOnThreadPool();

            _logger.LogInformation("UAF Registration Response Response: {UAFRegistrationResponseResponse}", resultString);

            if (result.IsSuccessStatusCode)
            {
                return new FidoRegisterResult.Registered();
            }

            _logger.LogError("Fido post registration response returned {HttpStatusCode}", result.StatusCode);
            return new FidoRegisterResult.Failed();
        }

        private static UafDeregistrationRequest CreateDeregistrationRequest(string keyId)
        {
            var request = new UafDeregistrationRequest
            {
                Header = new UafOperationHeader
                {
                    Upv = new UafProtocolVersion { Major = 1, Minor = 0},
                    AppId = AppId,
                    Op = UafOperation.Dereg
                }
            };
            request.Authenticators.Add(new UafDeregisterAuthenticator { KeyId = keyId });
            return request;
        }

        private async Task PostDeregistrationRequest(string accessToken, UafDeregistrationRequest request)
        {
            var result = await _uafClient.PostDeregistrationRequest(accessToken, request).ResumeOnThreadPool();
            var resultString = await result.Content.ReadAsStringAsync().ResumeOnThreadPool();

            _logger.LogInformation("UAF Deregistration Response: {UafDeregistrationResponse}", resultString);

            result.EnsureSuccessStatusCode();
        }
    }
}