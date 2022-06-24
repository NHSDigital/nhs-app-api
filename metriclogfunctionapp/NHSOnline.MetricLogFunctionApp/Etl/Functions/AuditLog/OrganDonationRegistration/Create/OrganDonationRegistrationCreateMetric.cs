using System;
using System.ComponentModel.DataAnnotations.Schema;
using NHSOnline.MetricLogFunctionApp.Etl.Load;

namespace NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.OrganDonationRegistration.Create
{
    [Table("OrganDonationRegistrationCreateMetric", Schema = "events")]
    public class OrganDonationRegistrationCreateMetric : IEventRepositoryRow
    {
        public DateTimeOffset Timestamp { get; set; }
        public string SessionId { get; set; }
        public string AuditId { get; set; }
    }
}
