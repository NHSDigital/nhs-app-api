using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NHSOnline.Backend.Worker.Suppliers.Emis.Models;

namespace NHSOnline.Backend.Worker.Suppliers.Emis
{
    public class EmisClient : IEmisClient
    {
        public const string HeaderApplicationId = "X-API-ApplicationId";
        public const string HeaderEndUserSessionId = "X-API-EndUserSessionId";
        public const string HeaderSessionId = "X-API-SessionId";
        public const string HeaderUserPatientLinkToken = "userPatientLinkToken";
        public const string HeaderVersion = "X-API-Version";

        private const string SessionsEndUserSessionPath = "sessions/endusersession";
        private const string SessionsPath = "sessions";
        private const string DemographicsPath = "demographics";
        private readonly HttpClient _httpClient;

        public EmisClient(HttpClient httpClient, IEmisConfig config)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Add(HeaderApplicationId, config.ApplicationId);
            _httpClient.DefaultRequestHeaders.Add(HeaderVersion, config.Version);
            _httpClient.BaseAddress = config.BaseUrl;
        }


        public async Task<CreateEndUserSessionResponseModel> EndUserSessionAsync()
        {
            var response = _httpClient.PostAsync(SessionsEndUserSessionPath, null);
            var stringResponse = await response.Result.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<CreateEndUserSessionResponseModel>(stringResponse);
        }

        public async Task<CreateSessionResponseModel> SessionsAsync(string endUserSessionId, string connectionToken,
            string odsCode)
        {
            var sessionRequest = new CreateSessionRequestModel
            {
                AccessIdentityGuid = connectionToken,
                NationalPracticeCode = odsCode
            };

            var request = new HttpRequestMessage(HttpMethod.Post, SessionsPath);
            request.Headers.Add(HeaderEndUserSessionId, new[] {endUserSessionId});
            request.Content = new StringContent(JsonConvert.SerializeObject(sessionRequest));
            var response = _httpClient.SendAsync(request);

            var stringResponse = await response.Result.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<CreateSessionResponseModel>(stringResponse);
        }

        public async Task<DemographicsResponse> DemographicsAsync(string userPatientLinkToken, string responseSessionId,
            string endUserSessionId)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, DemographicsPath);
            request.Headers.Add(HeaderEndUserSessionId, new[] {endUserSessionId});
            request.Headers.Add(HeaderSessionId, new[] {responseSessionId});
            request.Headers.Add(HeaderUserPatientLinkToken, new[] {userPatientLinkToken});

            var response = _httpClient.SendAsync(request);

            var stringResponse = await response.Result.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<DemographicsResponse>(stringResponse);
        }
    }
}