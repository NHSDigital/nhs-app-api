using System.Collections.Generic;

namespace NHSOnline.Backend.Metrics.EventHub
{
    public class MessageReadEventLogData : IEventLogData
    {
        private readonly string _messageId;
        private readonly MessageSenderContextEventLogData _senderContextEventLogData;

        public MessageReadEventLogData(string messageId, MessageSenderContextEventLogData senderContextEventLogData)
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