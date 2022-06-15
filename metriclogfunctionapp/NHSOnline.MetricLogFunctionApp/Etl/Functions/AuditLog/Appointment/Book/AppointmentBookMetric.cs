using System;
using System.ComponentModel.DataAnnotations.Schema;
using NHSOnline.MetricLogFunctionApp.Etl.Load;

namespace NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.Appointment.Book;

[Table("AppointmentBookMetric", Schema = "events")]
public class AppointmentBookMetric : IEventRepositoryRow
{
    public DateTimeOffset Timestamp { get; set; }
    public string SessionId { get; set; }
    public string AuditId { get; set; }
}