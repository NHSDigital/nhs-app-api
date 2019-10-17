using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.UserInfo
{
    public class UserInfoApiConfig: IUserInfoApiConfig
    {
        public Uri UserInfoApiBaseUrl { get; set; }

        public UserInfoApiConfig(IConfiguration configuration, ILogger<UserInfoApiConfig> logger)
        {
            var apiBaseUrl = configuration.GetOrWarn("USERINFO_API_BASE_URL", logger);
            UserInfoApiBaseUrl = new Uri(apiBaseUrl, UriKind.Absolute);
        }
    }
}