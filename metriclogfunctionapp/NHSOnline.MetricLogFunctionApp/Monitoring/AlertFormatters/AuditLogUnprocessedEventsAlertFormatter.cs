using System.Collections.Generic;
using System.Text;
using NHSOnline.MetricLogFunctionApp.Monitoring.Models.AppInsightsAlert;

namespace NHSOnline.MetricLogFunctionApp.Monitoring.AlertFormatters
{
    public class AuditLogUnprocessedEventsAlertFormatter : IAlertFormatter
    {
        public bool CanFormat(AppInsightAlert alert)
        {
            return alert.Data.Essentials.AlertRule.StartsWith("AuditLog ETL Unprocessed Events Alert");
        }

        public string CreateMessageRow(Dictionary<string, object> row)
        {
            return new StringBuilder()
                .Append($"Environment : {row["env"]}\n")
                .Append($"Message : {row["message"]}\n")
                .Append($"Event Hub events details : {row["eventhubevents"]}\n")
                .ToString();
        }
    }
}
