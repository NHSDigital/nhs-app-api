using NHSOnline.MetricLogFunctionApp.Compute.QueueRequests;
using NHSOnline.MetricLogFunctionApp.Etl.Load;
using NHSOnline.MetricLogFunctionApp.Resilience;

namespace NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.Device
{
    public class DeviceMetricEtl : AuditLogEtl<DeviceMetric>
    {
        public DeviceMetricEtl(IEventsRepository repo, IAuditLogParser<DeviceMetric> parser, IRequestQueueOrchestrator<AuditReportRequest> queueOrchestrator)
            : base(repo, parser, queueOrchestrator)
        {

        }

        protected override string StoredProcedureName =>
            "CALL events.DeviceInsert({0},{1},{2},{3},{4},{5},{6},{7},{8})";

        protected override object[] ReturnParams(DeviceMetric metric)
        {
            return new object[]
            {
                metric.Timestamp,
                metric.SessionId,
                metric.AppVersion,
                metric.DeviceManufacturer,
                metric.DeviceModel,
                metric.DeviceOS,
                metric.DeviceOSVersion,
                metric.UserAgent,
                metric.AuditId
            };
        }
    }
}
