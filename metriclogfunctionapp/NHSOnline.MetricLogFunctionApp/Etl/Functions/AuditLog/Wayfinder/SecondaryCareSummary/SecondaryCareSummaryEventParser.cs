using System;
using System.Text.RegularExpressions;

namespace NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.Wayfinder.SecondaryCareSummary
{
    public class SecondaryCareSummaryEventParser : IAuditLogParser<SecondaryCareSummaryMetric>
    {
        private const string OperationFieldValue = "SecondaryCare_GetSummary_Response";
        private const string DetailsFieldValue = "Secondary Care Summary successfully retrieved";

        public SecondaryCareSummaryMetric Parse(AuditRecord source)
        {
            if (!IsSecondaryCareSummaryMetric(source)) return null;

            return new SecondaryCareSummaryMetric
            {
                Timestamp = source.Timestamp,
                SessionId = source.SessionId,
                TotalReferrals = GetTotalReferralsFromAuditRecordDetails(source.Details),
                TotalUpcomingAppointments = GetTotalUpcomingAppointmentsFromAuditRecordDetails(source.Details),
                AuditId = source.AuditId
            };
        }

        private bool IsSecondaryCareSummaryMetric(AuditRecord source)
        {
            if (source == null) return false;

            return source.Operation == OperationFieldValue &&
                   source.Details != null && source.Details.Contains(DetailsFieldValue);
        }

        private int GetTotalReferralsFromAuditRecordDetails(string details)
        {
            Regex totalReferralsPattern = new Regex(@".*Total\sReferrals:\s*(?<totalReferrals>\d*)");
            Match totalReferralsMatch = totalReferralsPattern.Match(details);

            return String.IsNullOrEmpty(totalReferralsMatch.Groups["totalReferrals"].Value) ?
                0 : int.Parse(totalReferralsMatch.Groups["totalReferrals"].Value);
        }

        private int GetTotalUpcomingAppointmentsFromAuditRecordDetails(string details)
        {
            Regex upcomingAppointmentsPattern = new Regex(@".*Total\sUpcoming\sAppointments:\s*(?<totalUpcomingAppointments>\d*)");
            Match upcomingAppointmentsMatch = upcomingAppointmentsPattern.Match(details);

            return String.IsNullOrEmpty(upcomingAppointmentsMatch.Groups["totalUpcomingAppointments"].Value) ?
                0 : int.Parse(upcomingAppointmentsMatch.Groups["totalUpcomingAppointments"].Value);
        }
    }
}