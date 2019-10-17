using System.Net.Http;

namespace NHSOnline.Backend.PfsApi.UserInfo
{
    public class UserInfoHttpClient
    {
        public HttpClient Client { get; }
        
        public UserInfoHttpClient(HttpClient client, IUserInfoApiConfig config)
        {   
            client.BaseAddress = config.UserInfoApiBaseUrl;
            Client = client;
        }
    }
}