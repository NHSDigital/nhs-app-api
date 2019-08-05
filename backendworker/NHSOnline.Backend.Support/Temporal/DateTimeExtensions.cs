using System;
using System.Globalization;

namespace NHSOnline.Backend.Support.Temporal
{
    public static class DateTimeExtensions
    {
        public static string FormatToYYYYMMDD(this DateTime? dateTimeValue)
        {
            return dateTimeValue?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        }
    }
}