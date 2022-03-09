using System.Collections.Generic;

namespace NHSOnline.Backend.Metrics
{
    public sealed class SilverIntegrationJumpOffBlockedData : IMetricData
    {
        public string ProviderId { get; }
        public string ProviderName { get; }
        public string JumpOffId { get; }
        public string Reason { get; }

        public SilverIntegrationJumpOffBlockedData(
            string providerId,
            string providerName,
            string jumpOffId,
            string reason)
        {
            ProviderId = providerId;
            ProviderName = providerName;
            JumpOffId = jumpOffId;
            Reason = reason;
        }

        public IEnumerable<KeyValuePair<string, string>> ToKeyValuePairs()
        {
            yield return new KeyValuePair<string, string>("ProviderId", ProviderId);
            yield return new KeyValuePair<string, string>("ProviderName", ProviderName);
            yield return new KeyValuePair<string, string>("JumpOffId", JumpOffId);
            yield return new KeyValuePair<string, string>("Reason", Reason);
        }
    }
}