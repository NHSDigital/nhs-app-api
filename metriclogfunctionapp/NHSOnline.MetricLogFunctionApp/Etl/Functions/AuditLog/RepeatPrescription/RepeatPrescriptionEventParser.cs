using System;
using System.Text.RegularExpressions;

namespace NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.RepeatPrescription
{
    public class RepeatPrescriptionEventParser : IAuditLogParser<RepeatPrescriptionMetric>
    {
        private const string OperationFieldValue = "RepeatPrescriptions_OrderRepeatMedications_Response";
        private const string DetailsFieldValue = "Repeat prescription request successfully created";

        public RepeatPrescriptionMetric Parse(AuditRecord source)
        {
            if (!IsRepeatPrescriptionMetric(source)) return null;

            return new RepeatPrescriptionMetric
            {
                Timestamp = source.Timestamp,
                SessionId = source.SessionId,
                AuditId = source.AuditId
            };
        }

        private bool IsRepeatPrescriptionMetric(AuditRecord source)
        {
            if (source == null) return false;

            return source.Operation == OperationFieldValue &&
                   source.Details != null && source.Details.Contains(DetailsFieldValue);
        }
    }
}
