using System.Collections.Generic;

namespace NHSOnline.Backend.Metrics
{
    public sealed class UpliftStartedData : IMetricData
    {
        private readonly string _sessionId;

        public UpliftStartedData(string sessionId)
        {
            _sessionId = sessionId;
        }

        public IEnumerable<KeyValuePair<string, string>> ToKeyValuePairs()
        {
            yield return new KeyValuePair<string, string>("SessionId", _sessionId);
        }
    }
}