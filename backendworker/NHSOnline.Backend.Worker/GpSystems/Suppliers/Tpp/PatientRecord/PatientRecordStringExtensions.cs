using System;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.PatientRecord
{
    public static class PatientRecordStringExtensions
    {
        public static DateTimeOffset? SafeParseToNullableDateTimeOffset(this string dateTimeString)
        {
            if (string.IsNullOrWhiteSpace(dateTimeString))
            {
                return null;
            }

            if (!DateTimeOffset.TryParse(dateTimeString, out var parsedDateTimeOffset))
            {
                return null;
            }

            return parsedDateTimeOffset;
        }
    }
}