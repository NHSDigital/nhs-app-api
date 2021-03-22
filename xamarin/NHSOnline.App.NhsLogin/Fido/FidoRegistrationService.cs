using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NHSOnline.App.Config;
using NHSOnline.App.NhsLogin.Fido.Assertion;
using NHSOnline.App.NhsLogin.Fido.Models;
using NHSOnline.App.Threading;

namespace NHSOnline.App.NhsLogin.Fido
{
    internal sealed class FidoRegistrationService
    {
        private const string AppId = "xarmain:nhs-app";

        private readonly ILogger _logger;
        private readonly INhsLoginConfiguration _config;

        public FidoRegistrationService(
            ILogger<FidoService> logger,
            INhsLoginConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        internal async Task<FidoRegisterResult> Register(IFidoKey key, string accessToken)
        {
            var registrationRequest = await GetRegistrationRequest(accessToken).ResumeOnThreadPool();

            var registrationResponse = await CreateAndSignRegistrationResponse(registrationRequest, key).ResumeOnThreadPool();

            return await PostRegistrationResponse(registrationResponse, accessToken).ResumeOnThreadPool();
        }

        private async Task<UafRegistrationRequest> GetRegistrationRequest(string accessToken)
        {
            using var client = new HttpClient
            {
                BaseAddress = _config.UafBaseAddress
            };
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

            var result = await client.GetAsync("/regRequest").ResumeOnThreadPool();
            var resultString = await result.Content.ReadAsStringAsync().ResumeOnThreadPool();

            _logger.LogInformation("UAF Registration Request: {UAFRegistrationRequest}", resultString);

            return JsonConvert.DeserializeObject<UafRegistrationRequest[]>(resultString)[0];
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
            using var client = new HttpClient
            {
                BaseAddress = _config.UafBaseAddress
            };
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

            var requestJson = JsonConvert.SerializeObject(new[] { registrationResponse });

            _logger.LogInformation("UAF Registration Response Request: {UAFRegistrationResponseRequest}", requestJson);

            using var content = new StringContent(requestJson, Encoding.Default, "application/json");
            var result = await client.PostAsync("/regResponse", content).ResumeOnThreadPool();
            var resultString = await result.Content.ReadAsStringAsync().ResumeOnThreadPool();

            _logger.LogInformation("UAF Registration Response Response: {UAFRegistrationResponseResponse}", resultString);

            if (result.IsSuccessStatusCode)
            {
                return new FidoRegisterResult.Registered();
            }

            _logger.LogError("Fido post registration response returned {HttpStatusCode}", result.StatusCode);
            return new FidoRegisterResult.Failed();
        }
    }
}