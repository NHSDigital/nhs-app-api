using System.Collections.Generic;

namespace NHSOnline.Backend.Metrics
{
    public sealed class NominatedPharmacyData : IMetricData
    {
        private readonly string _sessionId;

        public NominatedPharmacyData(string sessionId)
        {
            _sessionId = sessionId;
        }

        public IEnumerable<KeyValuePair<string, string>> ToKeyValuePairs()
        {
            yield return new KeyValuePair<string, string>(MetricLogBuilder.SessionId, _sessionId);
        }
    }
}