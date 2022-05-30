using System;

namespace NHSOnline.MetricLogFunctionApp.Compute.QueueRequests
{
    public class DailyDeviceReferralUsageReportRequest
    {
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
    }
}