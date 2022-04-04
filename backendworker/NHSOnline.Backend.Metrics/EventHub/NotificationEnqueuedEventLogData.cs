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
        private readonly bool _scheduled;
        private readonly string _hubPath;
        private readonly SenderContextEventLogData _senderContextEventLogData;

        public NotificationEnqueuedEventLogData(
            string nhsLoginId,
            string notificationId,
            bool scheduled,
            string hubPath,
            SenderContextEventLogData senderContextEventLogData
            )
        {
            _nhsLoginId = nhsLoginId;
            _notificationId = notificationId;
            _scheduled = scheduled;
            _hubPath = hubPath;
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
            yield return new KeyValuePair<string, string>("Scheduled", _scheduled.ToString(CultureInfo.InvariantCulture));
            yield return new KeyValuePair<string, string>("HubPath", _hubPath);
        }
    }
}