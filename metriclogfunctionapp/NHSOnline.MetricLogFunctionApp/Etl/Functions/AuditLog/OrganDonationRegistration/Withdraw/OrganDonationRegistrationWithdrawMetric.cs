using System;
using System.ComponentModel.DataAnnotations.Schema;
using NHSOnline.MetricLogFunctionApp.Etl.Load;

namespace NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.OrganDonationRegistration.Withdraw
{
    [Table("OrganDonationRegistrationWithdrawMetric", Schema = "events")]
    public class OrganDonationRegistrationWithdrawMetric : IEventRepositoryRow
    {
        public DateTimeOffset Timestamp { get; set; }
        public string SessionId { get; set; }
        public string AuditId { get; set; }
    }
}
