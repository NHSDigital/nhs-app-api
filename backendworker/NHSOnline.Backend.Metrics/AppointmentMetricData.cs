using System.Collections.Generic;
using System.Globalization;

namespace NHSOnline.Backend.Metrics
{
    public class AppointmentMetricData : IMetricData
    {
        private readonly string _sessionName;
        private readonly string _slotType;
        private readonly int _statusCode;

        public AppointmentMetricData(string sessionName, string slotType, int statusCode)
        {
            _sessionName = sessionName;
            _slotType = slotType;
            _statusCode = statusCode;
        }

        public IEnumerable<KeyValuePair<string, string>> ToKeyValuePairs()
        {
            yield return new KeyValuePair<string, string>("SessionName", _sessionName);
            yield return new KeyValuePair<string, string>("SlotType", _slotType);
            yield return new KeyValuePair<string, string>("StatusCode", _statusCode.ToString(CultureInfo.InvariantCulture));
        }
    }
}