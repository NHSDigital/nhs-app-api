using System.Collections.Generic;

namespace NHSOnline.Backend.Metrics.EventHub
{
    public class MessageReadEventLogData : IEventLogData
    {
        private readonly MessageSenderContextEventLogData _senderContextEventLogData;

        public MessageReadEventLogData(MessageSenderContextEventLogData senderContextEventLogData)
        {
            _senderContextEventLogData = senderContextEventLogData;
        }

        public IEnumerable<KeyValuePair<string, string>> ToKeyValuePairs(bool pidAllowed)
            => _senderContextEventLogData.ToKeyValuePairs(pidAllowed);
    }
}