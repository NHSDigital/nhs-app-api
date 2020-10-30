using System.Collections.Generic;

namespace NHSOnline.Backend.Metrics
{
    public class LoginData : IMetricData
    {
        public string RequestId { get; }
        public string SessionId { get; }
        public string UserAgent { get; }

        public LoginData(string requestId, string sessionId, string userAgent)
        {
            RequestId = requestId;
            SessionId = sessionId;
            UserAgent = userAgent;
        }

        public IEnumerable<KeyValuePair<string, string>> ToKeyValuePairs()
        {
            yield return new KeyValuePair<string, string>("RequestId", RequestId);
            yield return new KeyValuePair<string, string>("SessionId", SessionId);
            yield return new KeyValuePair<string, string>("UserAgent", UserAgent);
        }
    }
}