using System;
using System.Globalization;

namespace NHSOnline.HttpMocks.GpMedicalRecord
{
    public static class GpMedicalRecordConstants
    {
        public const string FhirDateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'sszzz";

        public static readonly DateTime TenMonthsAgoDate = System.DateTime.UtcNow.Date
            .AddMonths(-10);

        public static readonly string TenMonthsAgoDateString = TenMonthsAgoDate
            .ToString(GpMedicalRecordConstants.FhirDateTimeFormat, CultureInfo.InvariantCulture);

    }
}