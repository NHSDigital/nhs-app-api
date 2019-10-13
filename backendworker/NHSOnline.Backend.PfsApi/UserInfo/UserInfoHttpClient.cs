using System.Net.Http;
using NHSOnline.Backend.PfsApi.Configs;

namespace NHSOnline.Backend.PfsApi.UserInfo
{
    public class UserInfoHttpClient
    {
        public HttpClient Client { get; }
        
        public UserInfoHttpClient(HttpClient client, IApiConfig config)
        {   
            client.BaseAddress = config.ApiBaseUrl;
            Client = client;
        }
    }
}