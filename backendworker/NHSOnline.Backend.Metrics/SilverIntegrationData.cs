using System.Collections.Generic;

namespace NHSOnline.Backend.Metrics
{
    public sealed class SilverIntegrationData : IMetricData
    {
        private readonly string _sessionId;
        private readonly string _providerId;
        private readonly string _providerName;
        private readonly string _jumpOffId;

        public SilverIntegrationData(
            string sessionId,
            string providerId,
            string providerName,
            string jumpOffId)
        {
            _sessionId = sessionId;
            _providerId = providerId;
            _providerName = providerName;
            _jumpOffId = jumpOffId;
        }

        public IEnumerable<KeyValuePair<string, string>> ToKeyValuePairs()
        {
            yield return new KeyValuePair<string, string>(MetricLogBuilder.SessionId, _sessionId);
            yield return new KeyValuePair<string, string>("ProviderId", _providerId);
            yield return new KeyValuePair<string, string>("ProviderName", _providerName);
            yield return new KeyValuePair<string, string>("JumpOffId", _jumpOffId);
        }
    }
}