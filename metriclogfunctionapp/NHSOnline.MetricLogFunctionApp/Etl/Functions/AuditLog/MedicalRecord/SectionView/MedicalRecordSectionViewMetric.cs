using System;
using System.ComponentModel.DataAnnotations.Schema;
using NHSOnline.MetricLogFunctionApp.Etl.Load;

namespace NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.MedicalRecord.SectionView;

[Table("MedicalRecordSectionViewMetric", Schema = "events")]
public class MedicalRecordSectionViewMetric : IEventRepositoryRow
{
    public DateTimeOffset Timestamp { get; set; }
    public string SessionId { get; set; }
    public string Supplier  { get; set; }
    public bool IsActingOnBehalfOfAnother { get; set; }
    public string Section { get; set; }
    public string AuditId { get; set; }
}
