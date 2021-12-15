using System.Collections.Generic;

namespace NHSOnline.Backend.Metrics.EventHub
{
    public class MessageCreatedEventLogData : IEventLogData
    {
        private readonly string _messageId;
        private readonly SenderContextEventLogData _senderContextEventLogData;

        public MessageCreatedEventLogData(
            string messageId,
            SenderContextEventLogData senderContextEventLogData)
        {
            _messageId = messageId;
            _senderContextEventLogData = senderContextEventLogData;
        }

        public IEnumerable<KeyValuePair<string, string>> ToKeyValuePairs(bool pidAllowed)
        {
            foreach (var kvp in _senderContextEventLogData.ToKeyValuePairs(pidAllowed))
            {
                yield return kvp;
            }

            yield return new KeyValuePair<string, string>("MessageId", _messageId);
        }
    }
}