using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NHSOnline.App.NhsLogin.Fido.Models;
using NHSOnline.App.Threading;

namespace NHSOnline.App.NhsLogin.Fido
{
    internal sealed class UafClient : IUafClient
    {
        private const string RegistrationRequestEndpoint = "/regRequest";
        private const string RegistrationResponseEndpoint = "/regResponse";
        private const string DeregistrationRequestEndpoint = "/deregRequest";
        private const string AuthorisationRequestEndpoint = "/authRequest";

        private readonly UafHttpClient _uafHttpClient;
        private readonly ILogger<UafClient> _logger;

        public UafClient(UafHttpClient uafUafHttpClient, ILogger<UafClient> logger)
        {
            _uafHttpClient = uafUafHttpClient;
            _logger = logger;
        }

        public async Task<HttpResponseMessage> GetAuthorisationRequest()
            => await _uafHttpClient.Get(AuthorisationRequestEndpoint).ResumeOnThreadPool();

        public async Task<HttpResponseMessage> GetRegistrationRequest(string accessToken)
            => await _uafHttpClient.Get(RegistrationRequestEndpoint, accessToken).ResumeOnThreadPool();

        public async Task<HttpResponseMessage> PostRegistrationResponse(UafRegistrationResponse registrationResponse, string accessToken)
        {
            var requestJson = JsonConvert.SerializeObject(new[] { registrationResponse });

            _logger.LogInformation("UAF Registration Response Request: {UAFRegistrationResponseRequest}", requestJson);

            using var content = new StringContent(requestJson, Encoding.Default, "application/json");

            return await _uafHttpClient.Post(content, RegistrationResponseEndpoint, accessToken).ResumeOnThreadPool();
        }

        public async Task<HttpResponseMessage> PostDeregistrationRequest(string accessToken, UafDeregistrationRequest request)
        {
            var requestJson = JsonConvert.SerializeObject(new[] { request });

            _logger.LogInformation("UAF Deregistration Request: {UafDeregistrationRequest}", requestJson);

            using var content = new StringContent(requestJson, Encoding.Default, "application/json");

            return await _uafHttpClient.Post(content, DeregistrationRequestEndpoint, accessToken).ResumeOnThreadPool();
        }
    }
}