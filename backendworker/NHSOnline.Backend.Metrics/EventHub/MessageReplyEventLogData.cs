using System.Collections.Generic;

namespace NHSOnline.Backend.Metrics.EventHub
{
    public class MessageReplySentEventLogData : IEventLogData
    {
        public string MessageId { get; }
        public string Response { get; }
        public SenderContextEventLogData SenderContextEventLogData { get; }

        public MessageReplySentEventLogData(string messageId, string response,
            SenderContextEventLogData senderContextEventLogData)
        {
            MessageId = messageId;
            Response = response;
            SenderContextEventLogData = senderContextEventLogData;
        }

        public IEnumerable<KeyValuePair<string, string>> ToKeyValuePairs(bool pidAllowed)
        {
            foreach (var kvp in SenderContextEventLogData.ToKeyValuePairs(pidAllowed))
            {
                yield return kvp;
            }

            yield return new KeyValuePair<string, string>("MessageId", MessageId);
            yield return new KeyValuePair<string, string>("Response", Response);
        }
    }
}