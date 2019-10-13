using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.PfsApi.UserInfo;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.Configs
{
    public class ApiConfig: IApiConfig
    {
        public Uri ApiBaseUrl { get; set; }

        public ApiConfig(IConfiguration configuration, ILogger<ApiConfig> logger)
        {
            var apiBaseUrl = configuration.GetOrWarn("API_BASE_URL", logger);
            ApiBaseUrl = new Uri(apiBaseUrl, UriKind.Absolute);
        }
    }
}