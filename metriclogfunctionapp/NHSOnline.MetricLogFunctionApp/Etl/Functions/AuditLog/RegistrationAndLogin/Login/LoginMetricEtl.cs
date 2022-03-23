using NHSOnline.MetricLogFunctionApp.Etl.Load;

namespace NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.RegistrationAndLogin.Login
{
    public class LoginMetricEtl : AuditLogEtl<LoginMetric>
    {
        public LoginMetricEtl(IEventsRepository repo, IAuditLogParser<LoginMetric> parser)
            : base(repo, parser)
        {
        }

        protected override string StoredProcedureName =>
            "CALL events.LoginMetricInsert({0},{1},{2},{3},{4},{5},{6});";
        protected override object[] ReturnParams(LoginMetric metric)
        {
            return new object[]
            {
                metric.Timestamp,
                metric.OdsCode,
                metric.LoginId,
                metric.ProofLevel,
                metric.Referrer,
                metric.SessionId,
                metric.AuditId
            };
        }
    }
}
