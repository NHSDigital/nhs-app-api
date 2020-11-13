using System.Net.Http;
using NHSOnline.Backend.AspNet.HealthChecks;

namespace NHSOnline.Backend.PfsApi.UserInfo
{
    public class UserInfoHttpClient: INhsAppHealthCheckClient
    {
        public HttpClient Client { get; }

        public UserInfoHttpClient(HttpClient client, IUserInfoApiConfig config)
        {
            client.BaseAddress = config.UserInfoApiBaseUrl;
            Client = client;
        }
    }
}