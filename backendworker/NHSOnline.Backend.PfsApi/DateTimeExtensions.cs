using System;
using System.Globalization;

namespace NHSOnline.Backend.PfsApi
{
    public static class DateTimeExtensions
    {
        public static string FormatToYYYYMMDD(this DateTime? dateTimeValue)
        {
            return dateTimeValue?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        }
    }
}