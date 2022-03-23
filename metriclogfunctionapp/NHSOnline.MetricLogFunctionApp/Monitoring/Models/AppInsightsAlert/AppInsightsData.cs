namespace NHSOnline.MetricLogFunctionApp.Monitoring.Models.AppInsightsAlert
{
    public class AppInsightsData
    {
        public AppInsightsEssentials Essentials { get; set; }

        public AppInsightsAlertContext AlertContext { get; set; }
    }
}