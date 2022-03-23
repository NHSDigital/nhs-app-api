using System;

namespace NHSOnline.MetricLogFunctionApp.Etl.Load
{
    public interface IEventRepositoryRow
    {
        DateTimeOffset Timestamp { get; set; }
    }
}