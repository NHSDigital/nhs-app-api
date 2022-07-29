namespace NHSOnline.Backend.PfsApi.Areas.MetricLogging.Models
{
    public class OperationAuditData
    {
        public string Operation { get; }
        public string Details { get; }

        public OperationAuditData(string operation, string details)
        {
            Operation = operation;
            Details = details;
        }
    }
}
