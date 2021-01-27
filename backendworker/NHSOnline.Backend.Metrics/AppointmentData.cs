using System.Collections.Generic;
using System.Globalization;
namespace NHSOnline.Backend.Metrics
{
    public sealed class AppointmentData : IMetricData
    {
        private readonly string _sessionId;

        public AppointmentData(string sessionId)
        {
            _sessionId = sessionId;
        }

        public IEnumerable<KeyValuePair<string, string>> ToKeyValuePairs()
        {
            yield return new KeyValuePair<string, string>(MetricLogBuilder.SessionId, _sessionId);
        }
    }
}