using System.Collections.Generic;
using System.Globalization;

namespace NHSOnline.Backend.Metrics.EventHub
{
    public class NotificationEnqueuedEventLogData : IEventLogData
    {
        private readonly string _nhsLoginId;
        private readonly string _notificationId;
        private readonly string _trackingId;
        private readonly bool _scheduled;

        public NotificationEnqueuedEventLogData(
            string nhsLoginId,
            string notificationId,
            string trackingId,
            bool scheduled)
        {
            _nhsLoginId = nhsLoginId;
            _notificationId = notificationId;
            _trackingId = trackingId;
            _scheduled = scheduled;
        }

        public IEnumerable<KeyValuePair<string, string>> ToKeyValuePairs(bool pidAllowed)
        {
            yield return new KeyValuePair<string, string>("NhsLoginId", _nhsLoginId);
            yield return new KeyValuePair<string, string>("NotificationId", _notificationId);
            yield return new KeyValuePair<string, string>("TrackingId", _trackingId);
            yield return new KeyValuePair<string, string>("Scheduled", _scheduled.ToString(CultureInfo.InvariantCulture));
        }
    }
}