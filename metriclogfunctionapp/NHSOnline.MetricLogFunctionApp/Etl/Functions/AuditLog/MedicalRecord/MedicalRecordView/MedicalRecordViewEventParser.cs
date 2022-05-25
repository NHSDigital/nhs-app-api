using System;
using System.Text.RegularExpressions;

namespace NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.MedicalRecord.MedicalRecordView
{
    public class MedicalRecordViewEventParser : IAuditLogParser<MedicalRecordViewMetric>
    {
        private const string OperationFieldValue = "PatientRecord_View_Response";
        private const string DetailsFieldValue = "Patient record successfully retrieved";

        public MedicalRecordViewMetric Parse(AuditRecord source)
        {
            if (!IsMedicalRecordViewMetric(source)) return null;

            return new MedicalRecordViewMetric
            {
                Timestamp = source.Timestamp,
                SessionId = source.SessionId,
                HasSummaryRecordAccess = GetHasSummaryRecordAccessFromDetails(source.Details),
                HasDetailedRecordAccess = GetHasDetailedRecordAccessFromDetails(source.Details),
                AuditId = source.AuditId
            };
        }

        private bool GetHasSummaryRecordAccessFromDetails(string details)
        {
            Regex hasSummaryRecordAccessPattern =
                new Regex(@".*hasSummaryRecordAccess=\s*(?<hasSummaryRecordAccess>False|True)",
                    RegexOptions.IgnoreCase);
            Match hasSummaryRecordAccessMatch = hasSummaryRecordAccessPattern.Match(details);

            if (String.IsNullOrEmpty(hasSummaryRecordAccessMatch.Groups["hasSummaryRecordAccess"].Value))
            {
                return false;
            }

            return bool.Parse(hasSummaryRecordAccessMatch.Groups["hasSummaryRecordAccess"].Value);
        }

        private bool GetHasDetailedRecordAccessFromDetails(string details)
        {
            Regex hasDetailedRecordAccessPattern =
                new Regex(@".*hasDetailedRecordAccess=\s*(?<hasDetailedRecordAccess>False|True)",
                    RegexOptions.IgnoreCase);
            Match hasDetailedRecordAccessMatch = hasDetailedRecordAccessPattern.Match(details);

            if (String.IsNullOrEmpty(hasDetailedRecordAccessMatch.Groups["hasDetailedRecordAccess"].Value))
            {
                return false;
            }

            return bool.Parse(hasDetailedRecordAccessMatch.Groups["hasDetailedRecordAccess"].Value);
        }

        private bool IsMedicalRecordViewMetric(AuditRecord source)
        {
            if (source == null) return false;

            return source.Operation == OperationFieldValue &&
                   source.Details != null && source.Details.Contains(DetailsFieldValue);
        }
    }
}