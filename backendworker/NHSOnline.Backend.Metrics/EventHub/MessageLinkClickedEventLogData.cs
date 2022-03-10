using System.Collections.Generic;

namespace NHSOnline.Backend.Metrics.EventHub
{
    public class MessageLinkClickedEventLogData : IEventLogData
    {
        public string MessageId { get; }
        public string Link { get; }
        public SenderContextEventLogData SenderContextEventLogData { get; }

        public MessageLinkClickedEventLogData(
            string messageId,
            string link,
            SenderContextEventLogData senderContextEventLogData)
        {
            MessageId = messageId;
            Link = link;
            SenderContextEventLogData = senderContextEventLogData;
        }

        public IEnumerable<KeyValuePair<string, string>> ToKeyValuePairs(bool pidAllowed)
        {
            foreach (var kvp in SenderContextEventLogData.ToKeyValuePairs(pidAllowed))
            {
                yield return kvp;
            }

            yield return new KeyValuePair<string, string>("MessageId", MessageId);
            yield return new KeyValuePair<string, string>("Link", Link);
        }
    }
}