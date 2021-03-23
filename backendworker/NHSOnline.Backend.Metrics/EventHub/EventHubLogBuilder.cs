using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using NHSOnline.Backend.Metrics.Extensions;

namespace NHSOnline.Backend.Metrics.EventHub
{
    internal class EventHubLogBuilder
    {
        private const string ActionLabel = "Action";
        private const string EnvironmentNameLabel = "EnvironmentName";
        private const string TimestampLabel = "Timestamp";

        private IEnumerable<KeyValuePair<string, string>> _logData;

        private EventHubLogBuilder(IEnumerable<KeyValuePair<string, string>> initialData)
        {
            _logData = initialData;
        }

        public EventHubLogBuilder With(IEventLogData data, bool pidAllowed)
        {
            _logData = _logData.Concat(data.ToKeyValuePairs(pidAllowed));
            return this;
        }

        public string Build()
        {
            var metrics = new List<string> { $"{TimestampLabel}={Timestamp()}" };
            metrics.AddRange(_logData.Select(kvp => $"{kvp.Key}={HttpUtility.UrlEncode(kvp.Value)}"));
            var eventLog = string.Join(" ", metrics);

            return eventLog;
        }

        public static EventHubLogBuilder Create([CallerMemberName] string action = "", string environmentName = "")
        {
            return new EventHubLogBuilder(BaseLogData(action, environmentName));
        }

        private static IEnumerable<KeyValuePair<string, string>> BaseLogData(string action, string environmentName)
        {
            yield return new KeyValuePair<string, string>(ActionLabel, action);
            yield return new KeyValuePair<string, string>(EnvironmentNameLabel, environmentName);
        }

        private static string Timestamp()
        {
            return DateTimeOffset.UtcNow.ToSplunkString();
        }
    }
}