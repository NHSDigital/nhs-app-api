using System;
using System.Globalization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.PatientRecord
{
    public static class PatientRecordStringExtensions
    {
        public static DateTimeOffset? SafeParseToNullableDateTimeOffset(this string dateTimeString)
        {
            if (string.IsNullOrWhiteSpace(dateTimeString))
            {
                return null;
            }

            if (!DateTimeOffset.TryParse(dateTimeString, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDateTimeOffset))
            {
                return null;
            }

            return parsedDateTimeOffset;
        }
    }
}