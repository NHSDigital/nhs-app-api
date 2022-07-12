using System;
using System.ComponentModel.DataAnnotations.Schema;
using NHSOnline.MetricLogFunctionApp.Etl.Load;

namespace NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.NominatedPharmacy.Create
{
    [Table("NominatedPharmacyCreateMetric", Schema = "events")]
    public class NominatedPharmacyCreateMetric : IEventRepositoryRow
    {
        public DateTimeOffset Timestamp { get; set; }
        public string SessionId { get; set; }
        public string AuditId { get; set; }
    }
}
