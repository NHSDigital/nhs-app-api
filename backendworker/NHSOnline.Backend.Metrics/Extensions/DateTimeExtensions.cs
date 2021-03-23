using System;
using System.Globalization;

namespace NHSOnline.Backend.Metrics.Extensions
{
    internal static class DateTimeExtensions
    {
        private const string TimestampSplunkFormat = "yyyy-MM-ddTHH:mm:ss:fff";

        public static string ToSplunkString(this DateTime dateTime) =>
            dateTime.ToString(TimestampSplunkFormat, CultureInfo.InvariantCulture);

        public static string ToSplunkString(this DateTimeOffset dateTimeOffset) =>
            dateTimeOffset.ToString(TimestampSplunkFormat, CultureInfo.InvariantCulture);
    }
}