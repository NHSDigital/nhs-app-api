using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Web;

namespace NHSOnline.Backend.Metrics
{
    internal class MetricLogBuilder
    {
        private const string ActionLabel = "Action";
        private const string OdsCodeLabel = "OdsCode";
        private const string TimestampLabel = "Timestamp";

        private IEnumerable<KeyValuePair<string, string>> _metricData;

        private MetricLogBuilder(IEnumerable<KeyValuePair<string, string>> initialData)
        {
            _metricData = initialData;
        }

        public MetricLogBuilder With(IMetricData metricData)
        {
            return With(metricData.ToKeyValuePairs());
        }

        public MetricLogBuilder With(IEnumerable<KeyValuePair<string, string>> metricData)
        {
            _metricData = _metricData.Concat(metricData);
            return this;
        }
        
        public Task WriteMetricLog()
        {
            var metrics = new List<string> { $"{TimestampLabel}={Timestamp()}" };
            metrics.AddRange(_metricData.Select(kvp => $"{kvp.Key}={HttpUtility.UrlEncode(kvp.Value)}"));
            var metricLog = string.Join(" ", metrics);

            Console.Out.WriteLine(metricLog);
            return Task.CompletedTask;
        }

        internal static MetricLogBuilder Create(IMetricContext metricContext, [CallerMemberName] string action = "")
        {
            return new MetricLogBuilder(AnonymousMetricData(metricContext, action));
        }

        private static IEnumerable<KeyValuePair<string, string>> AnonymousMetricData(IMetricContext metricContext,
            string action)
        {
            yield return new KeyValuePair<string, string>(OdsCodeLabel, metricContext.OdsCode);
            yield return new KeyValuePair<string, string>(ActionLabel, action);
        }

        private static string Timestamp()
        {
            return DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss:fff", CultureInfo.InvariantCulture);
        }
    }
}