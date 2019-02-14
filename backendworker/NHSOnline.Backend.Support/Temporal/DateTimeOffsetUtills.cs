using System;

namespace NHSOnline.Backend.Support.Temporal
{
    public static class DateTimeOffsetUtills
    {   
        public static DateTimeOffset SetTimeToMidnight(this DateTimeOffset dateTimeOffset)
        {
            return new DateTimeOffset(dateTimeOffset.Date, dateTimeOffset.Offset);
        }

        public static DateTimeOffset SubDays(this DateTimeOffset dateTimeOffset, int days)
        {
            return dateTimeOffset.AddDays(-days);
        }
    }
}
