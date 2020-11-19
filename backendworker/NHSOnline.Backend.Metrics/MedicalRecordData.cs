using System.Collections.Generic;
using System.Globalization;

namespace NHSOnline.Backend.Metrics
{
    public sealed class MedicalRecordData : IMetricData
    {
        private readonly string _sessionId;
        private readonly bool _hasSummaryRecordAccess;
        private readonly bool _hasDetailedRecordAccess;

        public MedicalRecordData(string sessionId, bool hasSummaryRecordAccess, bool hasDetailedRecordAccess)
        {
            _sessionId = sessionId;
            _hasSummaryRecordAccess = hasSummaryRecordAccess;
            _hasDetailedRecordAccess = hasDetailedRecordAccess;
        }

        public IEnumerable<KeyValuePair<string, string>> ToKeyValuePairs()
        {
            yield return new KeyValuePair<string, string>(MetricLogBuilder.SessionId, _sessionId);
            yield return new KeyValuePair<string, string>("HasSummaryRecordAccess", _hasSummaryRecordAccess.ToString(CultureInfo.InvariantCulture));
            yield return new KeyValuePair<string, string>("HasDetailedRecordAccess", _hasDetailedRecordAccess.ToString(CultureInfo.InvariantCulture));
        }
    }
}