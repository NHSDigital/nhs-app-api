namespace NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.Appointment.Cancel
{
    public class AppointmentCancelEventParser : IAuditLogParser<AppointmentCancelMetric>
    {
        private const string OperationFieldValue = "Appointments_Cancel_Response";
        private const string DetailsFieldValue = "Appointment successfully cancelled";

        public AppointmentCancelMetric Parse(AuditRecord source)
        {
            if (!IsAppointmentCancelMetric(source)) return null;

            return new AppointmentCancelMetric
            {
                Timestamp = source.Timestamp,
                SessionId = source.SessionId,
                AuditId = source.AuditId
            };
        }

        private bool IsAppointmentCancelMetric(AuditRecord source)
        {
            if (source == null) return false;

            return source.Operation == OperationFieldValue &&
                   source.Details != null && source.Details.Contains(DetailsFieldValue);
        }
    }
}