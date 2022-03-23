using System.Collections.Generic;
using System.Text;
using NHSOnline.MetricLogFunctionApp.Monitoring.Models.AppInsightsAlert;

namespace NHSOnline.MetricLogFunctionApp.Monitoring.AlertFormatters
{
    public class UnknownAlertFormatter : IAlertFormatter
    {
        public bool CanFormat(AppInsightAlert alert)
        {
            return true;
        }

        public string CreateMessageRow(Dictionary<string, object> row)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("*UNKNOWN ALERT!*\nData as follows: \n");

            foreach (var item in row)
            {
                stringBuilder.Append($"{item.Key} : {item.Value}\n");
            }
            return stringBuilder.ToString();
        }
    }
}