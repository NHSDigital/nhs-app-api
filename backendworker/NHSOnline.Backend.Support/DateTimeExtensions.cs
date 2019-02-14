using System;
using System.Globalization;

namespace NHSOnline.Backend.Support
{
    public static class DateTimeExtensions
    {
        public static string FormatToYYYYMMDD(this DateTime? dateTimeValue)
        {
            if (dateTimeValue == null) return null;

            return dateTimeValue.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        }
    }
}