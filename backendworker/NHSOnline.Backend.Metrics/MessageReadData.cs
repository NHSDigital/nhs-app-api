using System.Collections.Generic;

namespace NHSOnline.Backend.Metrics
{
    public class MessageReadData : IMetricData
    {
        public string MessageId { get; }
        public string CommunicationId { get; }
        public string TransmissionId { get; }

        public MessageReadData(string messageId, string communicationId, string transmissionId)
        {
            MessageId = messageId;
            CommunicationId = communicationId;
            TransmissionId = transmissionId;
        }

        public IEnumerable<KeyValuePair<string, string>> ToKeyValuePairs()
        {
            yield return new KeyValuePair<string, string>("MessageId", MessageId);
            yield return new KeyValuePair<string, string>("CommunicationId", CommunicationId);
            yield return new KeyValuePair<string, string>("TransmissionId", TransmissionId);
        }
    }
}