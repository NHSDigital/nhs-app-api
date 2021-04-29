using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.Messages
{
    public class MessagesApiConfig : IMessagesApiConfig
    {
        public string ApiKey { get; set; }
        public Uri BaseUrl { get; set; }
        public string ResourceUrl { get; set; }

        public MessagesApiConfig(IConfiguration configuration, ILogger<MessagesApiConfig> logger)
        {
            ApiKey = configuration.GetOrThrow("NHSAPP_API_KEY", logger);
            BaseUrl = new Uri(configuration.GetOrThrow("MESSAGES_API_BASE_URL", logger), UriKind.Absolute);
            ResourceUrl = configuration.GetOrThrow("MESSAGES_API_RESOURCE_URL", logger);
        }
    }
}
