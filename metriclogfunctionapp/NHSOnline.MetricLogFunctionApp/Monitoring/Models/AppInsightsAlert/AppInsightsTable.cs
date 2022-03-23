using System.Collections.Generic;

namespace NHSOnline.MetricLogFunctionApp.Monitoring.Models.AppInsightsAlert
{
    public class AppInsightsTable
    {
        public List<List<object>> Rows { get; set; }

        public string Name { get; set; }

        public List<AppInsightColumn> Columns { get; set; }
    }
}