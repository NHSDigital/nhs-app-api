using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.PfsApi.Resources;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.Messages
{
    public class MessagesServiceConfig : IMessagesServiceConfig
    {
        public bool SendIntroductoryMessage { get; }

        public string Body { get; }
        public string CampaignId { get; }

        public MessagesServiceConfig(IConfiguration configuration, ILogger<MessagesServiceConfig> logger)
        {
            SendIntroductoryMessage = configuration.GetBoolOrThrow("SEND_INTRODUCTORY_MESSAGE", logger);
            CampaignId = configuration.GetOrWarn("INTRODUCTORY_MESSAGE_CAMPAIGN_ID", logger);
            Body = EmbeddedResources.GetEmbeddedResource(EmbeddedResources.IntroductoryMessage);
        }
    }
}
