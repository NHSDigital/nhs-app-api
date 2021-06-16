using System.ComponentModel;

namespace NHSOnline.Backend.AspNet.HealthChecks.PerformanceCounter.DataPoints
{
    public enum DataPointType
    {
        [Description("numberOfRequests")]
        RequestCount = 0,

        [Description("averageResponseTimeInMs")]
        ResponseTime = 1
    }
}