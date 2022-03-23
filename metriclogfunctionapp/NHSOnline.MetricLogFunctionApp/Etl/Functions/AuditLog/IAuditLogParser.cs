namespace NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog
{
    public interface IAuditLogParser<out T>
    {
        T Parse(AuditRecord source);
    }
}
