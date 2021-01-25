using System.Collections.Generic;

namespace NHSOnline.Backend.Metrics
{
    public class AppointmentData : IMetricData
    {
        private readonly string _sessionId;

        public AppointmentData(string sessionId)
        {
            _sessionId = sessionId;
        }

        public IEnumerable<KeyValuePair<string, string>> ToKeyValuePairs()
        {
            yield return new KeyValuePair<string, string>("SessionId", _sessionId);
        }
    }
}