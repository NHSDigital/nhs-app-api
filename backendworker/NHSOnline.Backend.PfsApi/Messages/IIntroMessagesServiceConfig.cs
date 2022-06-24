namespace NHSOnline.Backend.PfsApi.Messages
{
    public interface IIntroMessagesServiceConfig
    {
        public bool SendIntroductoryMessage { get; }

        public string Body { get; }
        public string CampaignId { get; }
        public string SenderId { get; }
    }
}
