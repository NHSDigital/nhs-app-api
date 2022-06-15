namespace NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.Appointment.Book;

public class AppointmentBookEventParser : IAuditLogParser<AppointmentBookMetric>
{
    private const string OperationFieldValue = "Appointments_Book_Response";
    private const string DetailsFieldValue = "Appointment successfully booked";

    public AppointmentBookMetric Parse(AuditRecord source)
    {
        if (!IsAppointmentBookMetric(source)) return null;
        return new AppointmentBookMetric
        {
            Timestamp = source.Timestamp,
            SessionId = source.SessionId,
            AuditId = source.AuditId
        };
    }
    
    private bool IsAppointmentBookMetric(AuditRecord source)
    {
        if (source == null) return false;
        return source.Operation == OperationFieldValue &&
               source.Details != null && source.Details.Contains(DetailsFieldValue);
    }
}