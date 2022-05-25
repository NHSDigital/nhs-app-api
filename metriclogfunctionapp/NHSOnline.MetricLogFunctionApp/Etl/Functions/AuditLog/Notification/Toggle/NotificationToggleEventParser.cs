using System;
using System.Text.RegularExpressions;

namespace NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.Notification.Toggle
{
    public class NotificationToggleEventParser : IAuditLogParser<NotificationToggleMetric>
    {
        private const string OperationFieldValue = "NotificationToggle_Response";
        private const string DetailsFieldValue = "Notification toggled.";

        public NotificationToggleMetric Parse(AuditRecord source)
        {
            if (!IsNotificationToggleMetric(source)) return null;

            return new NotificationToggleMetric
            {
                LoginId = source.NhsLoginSubject,
                Timestamp = source.Timestamp,
                NotificationToggle = GetOptedInValueFromAuditRecordDetails(source.Details),
                AuditId = source.AuditId
            };
        }

        private bool IsNotificationToggleMetric(AuditRecord source)
        {
            if (source == null) return false;

            return source.Operation == OperationFieldValue &&
                   source.Details != null && source.Details.Contains(DetailsFieldValue);
        }

        private string GetOptedInValueFromAuditRecordDetails(string details)
        {
            Regex optedInPattern = new Regex(@"optIn=(?<optedIn>.*)");
            Match optedInMatch = optedInPattern.Match(details);

            return optedInMatch.Groups["optedIn"].Value switch
            {
                "true" => "On",
                _ => "Off"
            };
        }
    }
}