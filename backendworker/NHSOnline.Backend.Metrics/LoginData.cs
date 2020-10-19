using System.Collections.Generic;

namespace NHSOnline.Backend.Metrics
{
    public class LoginData : IMetricData
    {
        public string RequestId { get; }

        public LoginData(string requestId)
        {
            RequestId = requestId;
        }

        public IEnumerable<KeyValuePair<string, string>> ToKeyValuePairs()
        {
            yield return new KeyValuePair<string, string>("RequestId", RequestId);
        }
    }
}