using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.PfsApi.Resources;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.Messages
{
    public class IntroMessagesServiceConfig : IIntroMessagesServiceConfig
    {
        public bool SendIntroductoryMessage { get; }

        public string Body { get; }
        public string CampaignId { get; }
        public string SenderId { get; }
        public string SupplierId { get; }

        public IntroMessagesServiceConfig(IConfiguration configuration, ILogger<IntroMessagesServiceConfig> logger)
        {
            SendIntroductoryMessage = configuration.GetBoolOrThrow("SEND_INTRODUCTORY_MESSAGE", logger);
            CampaignId = configuration.GetOrWarn("INTRODUCTORY_MESSAGE_CAMPAIGN_ID", logger);
            SenderId = configuration.GetOrThrow("MESSAGES_SENDER_ID_NHS_APP", logger);
            SupplierId = configuration.GetOrThrow("MESSAGES_SUPPLIER_ID_NHS_APP", logger);
            Body = EmbeddedResources.GetEmbeddedResource(EmbeddedResources.IntroductoryMessage);
        }
    }
}
