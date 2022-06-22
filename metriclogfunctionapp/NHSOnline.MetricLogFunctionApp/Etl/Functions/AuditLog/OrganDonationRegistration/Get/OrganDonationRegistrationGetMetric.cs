using System;
using System.ComponentModel.DataAnnotations.Schema;
using NHSOnline.MetricLogFunctionApp.Etl.Load;

namespace NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.OrganDonationRegistration.Get;

[Table("OrganDonationRegistrationGetMetric", Schema = "events")]
public class OrganDonationRegistrationGetMetric : IEventRepositoryRow
{
    public DateTimeOffset Timestamp { get; set; }
    public string SessionId { get; set; }
    public string AuditId { get; set; }
}
