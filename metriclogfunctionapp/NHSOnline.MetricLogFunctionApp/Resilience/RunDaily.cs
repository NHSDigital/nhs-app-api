using System;

namespace NHSOnline.MetricLogFunctionApp.Resilience
{
    internal static class RunDaily
    {
        internal const string CronExpressionEarlyMorning = "0 10 4 * * *";

        internal static (DateTime startDateTime, DateTime endDateTime) CalculateRunDateTimeRange()
        {
            var endDateTime = DateTime.UtcNow.Date;
            var startDateTime = endDateTime.AddDays(-1);

            return (startDateTime, endDateTime);
        }
    }
}