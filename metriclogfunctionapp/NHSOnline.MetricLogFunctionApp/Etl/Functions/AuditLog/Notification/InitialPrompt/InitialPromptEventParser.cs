using System;
using System.Text.RegularExpressions;

namespace NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.Notification.InitialPrompt
{
    public class InitialPromptEventParser : IAuditLogParser<InitialPromptMetric>
    {
        private const string OperationFieldValue = "InitialNotificationPrompt_Decision";
        private const string DetailsFieldValue = "Initial notification prompt decision made.";

        public InitialPromptMetric Parse(AuditRecord source)
        {
            if (!IsInitialPromptMetric(source)) return null;

            return new InitialPromptMetric
            {
                LoginId = source.NhsLoginSubject,
                Timestamp = source.Timestamp,
                OptedIn = GetOptedInValueFromAuditRecordDetails(source.Details),
                AuditId = source.AuditId
            };
        }

        private bool IsInitialPromptMetric(AuditRecord source)
        {
            if (source == null) return false;

            return source.Operation == OperationFieldValue &&
                   source.Details != null && source.Details.Contains(DetailsFieldValue);
        }

        private String GetOptedInValueFromAuditRecordDetails(string details)
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