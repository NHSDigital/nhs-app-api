using System.Collections.Generic;

namespace NHSOnline.Backend.Metrics.EventHub
{
    public class MessageReplyCompletedEventLogData : IEventLogData
    {
        public string MessageId { get; }
        public SenderContextEventLogData SenderContextEventLogData { get; }

        public MessageReplyCompletedEventLogData(string messageId,
            SenderContextEventLogData senderContextEventLogData)
        {
            MessageId = messageId;
            SenderContextEventLogData = senderContextEventLogData;
        }

        public IEnumerable<KeyValuePair<string, string>> ToKeyValuePairs(bool pidAllowed)
        {
            foreach (var kvp in SenderContextEventLogData.ToKeyValuePairs(pidAllowed))
            {
                yield return kvp;
            }

            yield return new KeyValuePair<string, string>("MessageId", MessageId);
        }
    }
}