using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace NHSOnline.Backend.Metrics.EventHub
{
    public class NotificationEnqueuedEventLogData : IEventLogData
    {
        private const string NhsLoginIdKey = "NhsLoginId";
        private readonly string _nhsLoginId;
        private readonly string _notificationId;
        private readonly string _trackingId;
        private readonly bool _scheduled;
        private readonly SenderContextEventLogData _senderContextEventLogData;

        public NotificationEnqueuedEventLogData(
            string nhsLoginId,
            string notificationId,
            string trackingId,
            bool scheduled,
            SenderContextEventLogData senderContextEventLogData
            )
        {
            _nhsLoginId = nhsLoginId;
            _notificationId = notificationId;
            _trackingId = trackingId;
            _scheduled = scheduled;
            _senderContextEventLogData = senderContextEventLogData;
        }

        public IEnumerable<KeyValuePair<string, string>> ToKeyValuePairs(bool pidAllowed)
        {

            var senderContextKvps = _senderContextEventLogData
                .ToKeyValuePairs(pidAllowed)
                .Where(x => x.Key != NhsLoginIdKey);

            foreach (var kvp in senderContextKvps)
            {
                yield return kvp;
            }

            yield return new KeyValuePair<string, string>(NhsLoginIdKey, _nhsLoginId);
            yield return new KeyValuePair<string, string>("NotificationId", _notificationId);
            yield return new KeyValuePair<string, string>("TrackingId", _trackingId);
            yield return new KeyValuePair<string, string>("Scheduled", _scheduled.ToString(CultureInfo.InvariantCulture));
        }
    }
}