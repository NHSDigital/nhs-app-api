using System.Collections.Generic;

namespace NHSOnline.Backend.Metrics
{
    public sealed class RepeatPrescriptionData : IMetricData
    {
        private readonly string _sessionId;

        public RepeatPrescriptionData(string sessionId)
        {
            _sessionId = sessionId;
        }

        public IEnumerable<KeyValuePair<string, string>> ToKeyValuePairs()
        {
            yield return new KeyValuePair<string, string>(MetricLogBuilder.SessionId, _sessionId);
        }
    }
}