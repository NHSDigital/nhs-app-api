using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NHSOnline.Backend.Worker.Bridges.Emis.Models;

namespace NHSOnline.Backend.Worker.Bridges.Emis
{
    public class EmisClient : IEmisClient
    {
        public const string HeaderApplicationId = "X-API-ApplicationId";
        public const string HeaderEndUserSessionId = "X-API-EndUserSessionId";
        public const string HeaderSessionId = "X-API-SessionId";
        public const string HeaderVersion = "X-API-Version";

        private const string MeApplicationsPath = "me/applications";
        private const string SessionsEndUserSessionPath = "sessions/endusersession";
        private const string SessionsPath = "sessions";
        private const string DemographicsPath = "demographics?userPatientLinkToken={0}";

        private readonly HttpClient _httpClient;

        public EmisClient(IHttpClientFactory httpClientFactory, IEmisConfig config)
        {
            _httpClient = httpClientFactory.GetClient(HttpClientName.EmisApiClient);
            _httpClient.DefaultRequestHeaders.Add(HeaderApplicationId, config.ApplicationId);
            _httpClient.DefaultRequestHeaders.Add(HeaderVersion, config.Version);
            _httpClient.BaseAddress = config.BaseUrl;
        }

        public async Task<EmisApiObjectResponse<SessionsEndUserSessionPostResponse>> SessionsEndUserSessionPost()
        {
            var response = await Post<SessionsEndUserSessionPostResponse>(SessionsEndUserSessionPath);
            return response;
        }

        public async Task<EmisApiObjectResponse<SessionsPostResponse>> SessionsPost(string endUserSessionId, SessionsPostRequest model)
        {
            var response = await Post<SessionsPostRequest, SessionsPostResponse>(model, SessionsPath, endUserSessionId);
            return response;
        }

        public async Task<EmisApiObjectResponse<DemographicsGetResponse>> DemographicsGet(string userPatientLinkToken, string responseSessionId,
            string endUserSessionId)
        {
            var path = string.Format(DemographicsPath, userPatientLinkToken);

            var response = await Get<DemographicsGetResponse>(path, endUserSessionId, responseSessionId);
            return response;
        }

        public async Task<EmisApiObjectResponse<MeApplicationsPostResponse>> MeApplicationsPost(string endUserSessionId, MeApplicationsPostRequest model)
        {
            var response = await Post<MeApplicationsPostRequest, MeApplicationsPostResponse>(model, MeApplicationsPath, endUserSessionId);
            return response;
        }

        private async Task<EmisApiObjectResponse<TResponse>> Get<TResponse>(string path,
            string endUserSessionId = null, string sessionId = null)
        {
            var request = BuildEmisRequest(HttpMethod.Get, path, endUserSessionId, sessionId);
            var response = await SendRequestAndParseResponse<TResponse>(request);
            return response;
        }

        private async Task<EmisApiObjectResponse<TResponse>> Post<TRequest, TResponse>(TRequest model, string path,
            string endUserSessionId = null, string sessionId = null)
        {
            var request = BuildEmisRequest(HttpMethod.Post, path, endUserSessionId, sessionId);

            var body = JsonConvert.SerializeObject(model);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            var response = await SendRequestAndParseResponse<TResponse>(request);
            return response;
        }

        private async Task<EmisApiObjectResponse<TResponse>> Post<TResponse>(string path,
            string endUserSessionId = null, string sessionId = null)
        {
            var request = BuildEmisRequest(HttpMethod.Post, path, endUserSessionId, sessionId);

            var response = await SendRequestAndParseResponse<TResponse>(request);
            return response;
        }

        private static HttpRequestMessage BuildEmisRequest(HttpMethod httpMethod, string path, string endUserSessionId = null, string sessionId = null)
        {
            var request = new HttpRequestMessage(httpMethod, path);

            if (!string.IsNullOrEmpty(endUserSessionId))
            {
                request.Headers.Add(HeaderEndUserSessionId, new[] {endUserSessionId});
            }

            if (!string.IsNullOrEmpty(sessionId))
            {
                request.Headers.Add(HeaderSessionId, new[] {sessionId});
            }

            return request;
        }

        private async Task<EmisApiObjectResponse<TResponse>> SendRequestAndParseResponse<TResponse>(
            HttpRequestMessage request)
        {
            var responseMessage = await _httpClient.SendAsync(request);
            var response = new EmisApiObjectResponse<TResponse>(responseMessage.StatusCode);

            var stringResponse = await responseMessage.Content.ReadAsStringAsync();
            if (responseMessage.IsSuccessStatusCode)
            {
                response.Body = JsonConvert.DeserializeObject<TResponse>(stringResponse);
            }
            else
            {
                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    response.ErrorResponseBadRequest =
                        JsonConvert.DeserializeObject<BadRequestErrorResponse>(stringResponse);
                }
                else
                {
                    response.ErrorResponse = JsonConvert.DeserializeObject<ErrorResponse>(stringResponse);
                }
            }

            return response;
        }

        public class EmisApiResponse
        {
            public EmisApiResponse(HttpStatusCode statusCode)
            {
                StatusCode = statusCode;
            }

            public HttpStatusCode StatusCode { get; set; }
            public ErrorResponse ErrorResponse { get; set; }
            public BadRequestErrorResponse ErrorResponseBadRequest { get; set; }
            public bool HasSuccessStatusCode => (int)StatusCode >= 200 && (int)StatusCode <= 299;

            public bool HasExceptionWithMessage(string message)
            {
                return ErrorResponse?.Exceptions?.Any(x => x.Message == message) ?? false;
            }
        }

        public class EmisApiObjectResponse<TBody> : EmisApiResponse
        {
            public EmisApiObjectResponse(HttpStatusCode statusCode) : base(statusCode)
            {
            }

            public TBody Body { get; set; }
        }
    }
}
