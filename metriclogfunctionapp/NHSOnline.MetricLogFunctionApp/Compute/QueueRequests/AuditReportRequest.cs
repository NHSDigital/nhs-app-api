using System;

namespace NHSOnline.MetricLogFunctionApp.Compute.QueueRequests
{
    public class AuditReportRequest
    {
        public string LoginId { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
    }
}