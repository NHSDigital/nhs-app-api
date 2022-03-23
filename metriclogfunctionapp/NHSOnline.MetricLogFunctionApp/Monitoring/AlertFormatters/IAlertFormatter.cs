using System.Collections.Generic;
using NHSOnline.MetricLogFunctionApp.Monitoring.Models.AppInsightsAlert;

namespace NHSOnline.MetricLogFunctionApp.Monitoring.AlertFormatters
{
    public interface IAlertFormatter
    {
        bool CanFormat(AppInsightAlert alert);
        string CreateMessageRow(Dictionary<string, object> row);
    }
}