namespace NHSOnline.Backend.PfsApi.Messages
{
    public interface IMessagesServiceConfig
    {
        public bool SendIntroductoryMessage { get; }

        public string Body { get; }
        public string CampaignId { get; }
    }
}
