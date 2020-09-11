using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using CorrelationId;
using Microsoft.AspNetCore.Http;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.UserInfo
{
    public class UserInfoClient : IUserInfoClient
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

        public async Task<UserInfoResponse> Post(string accessToken, HttpContext httpContext)
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, string.Empty);

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            request.Headers.Add(Constants.HttpHeaders.CorrelationIdentifier,
                _correlationContext.CorrelationContext?.CorrelationId ?? string.Empty);

            var webAppVersion = httpContext.Request.Headers[Constants.HttpHeaders.WebAppVersion];
            var nativeAppVersion = httpContext.Request.Headers[Constants.HttpHeaders.NativeAppVersion];
            if (webAppVersion.Any())
            {
                request.Headers.Add(Constants.HttpHeaders.WebAppVersion, webAppVersion.ToArray());
            }

            if (nativeAppVersion.Any())
            {
                request.Headers.Add(Constants.HttpHeaders.NativeAppVersion, nativeAppVersion.ToArray());
            }

            var responseMessage = await _httpClient.Client.SendAsync(request);
            return new UserInfoResponse(responseMessage.StatusCode);
        }
    }
}