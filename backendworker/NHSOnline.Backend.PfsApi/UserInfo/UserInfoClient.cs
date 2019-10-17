using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using CorrelationId;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.UserInfo
{
    public class UserInfoClient: IUserInfoClient
    {
        private readonly UserInfoHttpClient _httpClient;
        private readonly ICorrelationContextAccessor _correlationContext;
        
        public UserInfoClient(
            UserInfoHttpClient httpClient,
            ICorrelationContextAccessor correlationContext)
        {
            _httpClient = httpClient;
            _correlationContext = correlationContext;
        }

        public async Task<UserInfoResponse> Post(string accessToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, string.Empty);

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            
            request.Headers.Add(Constants.HttpHeaders.CorrelationIdentifier,
                _correlationContext.CorrelationContext?.CorrelationId ?? string.Empty);

            var responseMessage = await _httpClient.Client.SendAsync(request);
            return new UserInfoResponse(responseMessage.StatusCode);
        }
    }
}