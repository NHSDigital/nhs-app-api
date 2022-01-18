using System.Collections.Generic;

namespace NHSOnline.Backend.Metrics
{
    public class MessageLinkClickedData : IMetricData
    {
        public string MessageId { get; }
        public string Link { get; }
        public string CampaignId { get; }
        public string CommunicationId { get; }
        public string TransmissionId { get; }

        public MessageLinkClickedData(string messageId, string link, string campaignId, string communicationId, string transmissionId)
        {
            MessageId = messageId;
            Link = link;
            CampaignId = campaignId;
            CommunicationId = communicationId;
            TransmissionId = transmissionId;
        }

        public IEnumerable<KeyValuePair<string, string>> ToKeyValuePairs()
        {
            yield return new KeyValuePair<string, string>("MessageId", MessageId);
            yield return new KeyValuePair<string, string>("Link", Link);
            yield return new KeyValuePair<string, string>("CampaignId", CampaignId);
            yield return new KeyValuePair<string, string>("CommunicationId", CommunicationId);
            yield return new KeyValuePair<string, string>("TransmissionId", TransmissionId);
        }
    }
}
