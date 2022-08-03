using System;
using System.ComponentModel.DataAnnotations.Schema;
using NHSOnline.MetricLogFunctionApp.Etl.Load;

namespace NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.RegistrationAndLogin.LastLoginPatientIdentifier;

[Table("LastLoginPatientIdentifier", Schema = "events")]
public class LastLoginPatientIdentifier : IEventRepositoryRow
{
    public DateTimeOffset Timestamp { get; set; }
    public string AuditId { get; set; }
    public string LoginId { get; set; }
    public string NhsNumber { get; set; }
}
